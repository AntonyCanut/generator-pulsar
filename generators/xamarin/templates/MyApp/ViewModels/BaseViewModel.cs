using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Toolkit.ErrorManagement;
using Toolkit.Exceptions;
using Toolkit.Utilities;
using Toolkit.ViewServices;

namespace <%= name %>.ViewModels
{
    public abstract class BaseViewModel : GalaSoft.MvvmLight.ViewModelBase
    {
        private readonly object _locker = new object();
        private int _busyCounter;
        private TimeSpan _defaultTimeout = TimeSpan.FromSeconds(30);
        private readonly Dictionary<Guid, CancellationTokenSource> _cts = new Dictionary<Guid, CancellationTokenSource>();
        private readonly List<Guid> _pendingManualCancellation = new List<Guid>();
        
        protected object Parameter { get; private set; }

		public int BusyCounter => _busyCounter;
		public bool IsBusy => _busyCounter != 0;
        public IErrorHandler ErrorHandler { get; }
        public IDialogService DialogService { get; }
  
        protected BaseViewModel(IDialogService dialogService, IErrorHandler errorHandler)
        {
            DialogService = dialogService;
            ErrorHandler = errorHandler;
        }

        protected TimeSpan DefaultTimeout
        {
            get => _defaultTimeout;
            set => Set(ref _defaultTimeout, value);
        }

        private bool IsAManuallyCanceledTask(Guid guid)
        {
            lock (_locker)
            {
                return _pendingManualCancellation.Contains(guid);
            }
        }

        private void ManuallyCancelTask(Guid guid)
        {
            lock (_locker)
            {
                if (_cts.TryGetValue(guid, out CancellationTokenSource cts))
                {
                    _pendingManualCancellation.Add(guid);
                    cts?.Cancel();
                }
            }
        }

        private void RemoveFromPendingManualCancellation(Guid guid)
        {
            lock (_locker)
            {
                if (_pendingManualCancellation.Contains(guid))
                    _pendingManualCancellation.Remove(guid);
            }
        }

        protected Task ExecuteWithTimeout(Func<CancellationToken, Task> func, TimeSpan? timeout = null)
        {
            return ExecuteWithTimeout(ct => WrapIntoGenericTask<object, CancellationToken>(func, ct), timeout);
        }

        protected Task ExecuteWithTimeoutSafe(Func<CancellationToken, Task> func, TimeSpan? timeout = null)
        {
            return ExecuteWithTimeoutSafe(ct => WrapIntoGenericTask<object, CancellationToken>(func, ct), timeout);
        }

        protected Task ExecuteWithCancellationToken(Func<CancellationToken, Task> func)
        {
            return ExecuteWithCancellationToken(ct => WrapIntoGenericTask<object, CancellationToken>(func, ct));
        }

        protected Task ExecuteWithCancellationTokenSafe(Func<CancellationToken, Task> func)
        {
            return ExecuteWithCancellationTokenSafe(ct => WrapIntoGenericTask<object, CancellationToken>(func, ct));
        }

        protected Task ExecuteWithCancellationTokenSource(Func<CancellationTokenSource, Task> func)
        {
            return ExecuteWithCancellationTokenSource(ct => WrapIntoGenericTask<object, CancellationTokenSource>(func, ct));
        }

        protected Task ExecuteWithCancellationTokenSourceSafe(Func<CancellationTokenSource, Task> func)
        {
            return ExecuteWithCancellationTokenSourceSafe(ct => WrapIntoGenericTask<object, CancellationTokenSource>(func, ct));
        }

        protected Task<T> ExecuteWithCancellationTokenSource<T>(Func<CancellationTokenSource, Task<T>> func)
        {
            return ExecuteWithCancellationTokenSourceFactory(
            func,
            () => new CancellationTokenSource(),
            cts => cts);
        }

        protected Task<T> ExecuteWithCancellationToken<T>(Func<CancellationToken, Task<T>> func)
        {
            return ExecuteWithCancellationTokenSourceFactory(
            func,
            () => new CancellationTokenSource(),
            cts => cts.Token);
        }

        protected Task<T> ExecuteWithCancellationTokenSafe<T>(Func<CancellationToken, Task<T>> func)
        {
            return ExecuteWithCancellationTokenSourceFactory(
            func,
            () => new CancellationTokenSource(),
            cts => cts.Token,
            true);
        }

        protected Task<T> ExecuteWithCancellationTokenSourceSafe<T>(Func<CancellationTokenSource, Task<T>> func)
        {
            return ExecuteWithCancellationTokenSourceFactory(
            func,
            () => new CancellationTokenSource(),
            cts => cts,
            true);
        }

        protected Task<T> ExecuteWithTimeout<T>(Func<CancellationToken, Task<T>> func, TimeSpan? timeout = null)
        {
            return ExecuteWithCancellationTokenSourceFactory(
            func,
            () => new CancellationTokenSource(timeout ?? DefaultTimeout),
            cts => cts.Token);
        }

        protected Task<T> ExecuteWithTimeoutSafe<T>(Func<CancellationToken, Task<T>> func, TimeSpan? timeout = null)
        {
            return ExecuteWithCancellationTokenSourceFactory(
            func,
            () => new CancellationTokenSource(timeout ?? DefaultTimeout),
            cts => cts.Token,
            true);
        }

        private async Task<T> WrapIntoGenericTask<T, TToken>(Func<TToken, Task> func, TToken ct)
        {
            var invoke = func?.Invoke(ct);
            if (invoke != null)
                await invoke;
            return default(T);
        }

        private async Task<T> ExecuteWithCancellationTokenSourceFactory<T, TToken>(
        Func<TToken, Task<T>> func,
        Func<CancellationTokenSource> cancellationTokenSourceBuilder,
        Func<CancellationTokenSource, TToken> tokenExtractor,
        bool isSafe = false)
        {
            var guid = Guid.NewGuid();
            var cts = cancellationTokenSourceBuilder();

            AddCancellationTokenSource(guid, cts);

            T result;
            try
            {
                var token = tokenExtractor(cts);
                result = await ExecuteWithCancellationToken(func, token);
                RemoveCancellationTokenSource(guid);
            }
            catch (ServiceException ex)
            {
                if (IsAManuallyCanceledTask(guid))
                {
                    RemoveFromPendingManualCancellation(guid);
                    if (!isSafe)
                        throw new TaskCanceledByUserException(ex.Message, ex);
                }
                else
                {
                    if (!isSafe)
                        throw;
                    else
                        await ErrorHandler.HandleErrorAsync(ex, cts.Token);
                }

                result = default(T);
            }
            catch (TaskCanceledException ex)
            {
                if (IsAManuallyCanceledTask(guid))
                {
                    RemoveFromPendingManualCancellation(guid);

                    if (!isSafe)
                        throw new TaskCanceledByUserException(ex.Message, ex);
                }
                else
                {
                    if (!isSafe)
                        throw;
                    else
                        await ErrorHandler.HandleErrorAsync(ex, cts.Token);
                }

                result = default(T);
            }
            catch (Exception ex)
            {

                if (!isSafe)
                    throw;
                else
                    await ErrorHandler.HandleErrorAsync(ex, cts.Token);

                result = default(T);
            }
            finally
            {
                RemoveCancellationTokenSource(guid);
            }

            return result;
        }

        private void AddCancellationTokenSource(Guid guid, CancellationTokenSource cts)
        {
            lock (_locker)
            {
                _cts.Add(guid, cts);
            }
        }

        private void RemoveCancellationTokenSource(Guid guid)
        {
            lock (_locker)
            {
                try
                {
                    CancellationTokenSource cts;
                    if (_cts.TryGetValue(guid, out cts))
                    {
                        _cts.Remove(guid);
                        cts?.Cancel();
                        cts?.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    ExecuteWithCancellationTokenSafe(ct => ErrorHandler.HandleErrorAsync(ex, ct))
                    .FireAndForgetSafeAsync(null, CancellationToken.None);
                }
            }
        }

        protected async Task<T> ExecuteWithCancellationToken<T, TToken>(Func<TToken, Task<T>> func, TToken ct)
        {
            var result = default(T);
            var invoke = func?.Invoke(ct);
            if (invoke != null)
                result = await invoke;
            return result;
        }

        protected void IncrementBusyCounter()
        {
            Interlocked.Increment(ref _busyCounter);
            // ReSharper disable ExplicitCallerInfoArgument
            RaisePropertyChanged(nameof(BusyCounter));
            RaisePropertyChanged(nameof(IsBusy));
            // ReSharper restore ExplicitCallerInfoArgument
            RaiseCanExecuteChanged();
        }

        protected void DecrementBusyCounter()
        {
            Interlocked.Decrement(ref _busyCounter);
            // ReSharper disable ExplicitCallerInfoArgument
            RaisePropertyChanged(nameof(BusyCounter));
            RaisePropertyChanged(nameof(IsBusy));
            // ReSharper restore ExplicitCallerInfoArgument
            RaiseCanExecuteChanged();
        }


        public void Start(object parameter = null)
        {
            if (IsBusy)
                return;

            ExecuteWithCancellationTokenSafe(ct => StartAsync(parameter))
            .FireAndForgetSafeAsync(ErrorHandler, CancellationToken.None);
        }

        public async Task StartAsync(object parameter = null)
        {
            if (IsBusy)
                return;

            try
            {
                Parameter = parameter;
                IncrementBusyCounter();
                await ExecuteWithTimeout(StartAsync);
            }
            finally
            {
                DecrementBusyCounter();
            }
        }

        protected abstract Task StartAsync(CancellationToken ct);

        public override void Cleanup()
        {
            base.Cleanup();

            Parameter = null;
            _busyCounter = 0;

            lock (_locker)
            {
                var arr = _cts.Keys.ToList();
                if (arr.Count > 0)
                {
                    for (var i = arr.Count - 1; i >= 0; i--)
                    {
                        var guid = arr[i];
                        ManuallyCancelTask(guid);
                        RemoveCancellationTokenSource(guid);
                        arr.Remove(guid);
                    }
                }

                _cts.Clear();
            }
        }

        public virtual bool DefaultCanExecute()
        {
            return !IsBusy;
        }

        public virtual bool DefaultCanExecute<T>(T parameter)
        {
            return DefaultCanExecute();
        }

        protected virtual void RaiseCanExecuteChanged()
        {
        }
    }
}
