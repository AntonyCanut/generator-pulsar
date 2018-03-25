using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using CommonServiceLocator;
using Foundation;
using <%= name %>.Services;
using <%= name %>.ViewModels;
using UIKit;

namespace <%= name %>.ViewControllers.iOS
{
    public abstract class BaseViewController : UIViewController
    {
        private int _animations = 1;
        private SemaphoreSlim _semaphore = new SemaphoreSlim(1);
        private TaskCompletionSource<UIStoryboardSegue> _navigationTask;

        public virtual bool IsModalViewController => false;

        protected BaseViewController(IntPtr handle) : base(handle)
        {
        }

        protected virtual void SetupControls()
        {
        }

        protected virtual void SubscribeToViewEvents()
        {
            UnsubscribeFromViewEvents();
        }

        protected virtual void UnsubscribeFromViewEvents()
        {
        }

        protected virtual void EnableControls()
        {

        }

        protected virtual void DisableControls()
        {

        }

        protected virtual void ShowLoader()
        {

        }

        protected virtual void HideLoader()
        {

        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            SetupControls();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            SubscribeToViewEvents();
        }


        public override void ViewWillDisappear(bool animated)
        {
            UnsubscribeFromViewEvents();
            AttemptRotationToDeviceOrientation();
            base.ViewWillDisappear(animated);
        }

        public override void ViewWillUnload()
        {
            base.ViewWillUnload();

            UnsubscribeFromViewEvents();
        }

        protected virtual void ReleaseControls()
        {
        }

        public virtual void Cleanup()
        {
            UnsubscribeFromViewEvents();
            ReleaseControls();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                Cleanup();

            base.Dispose(disposing);
        }

        public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations()
        {
            return UIInterfaceOrientationMask.Portrait;
        }


        protected virtual Task<UIStoryboardSegue> PerformSegueAsync(string identifier, NSObject sender)
        {
            if (_navigationTask != null)
                _navigationTask.TrySetCanceled();

            _navigationTask = new TaskCompletionSource<UIStoryboardSegue>();

            PerformSegue(identifier, sender);

            return _navigationTask.Task;
        }

        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            base.PrepareForSegue(segue, sender);

            _navigationTask?.TrySetResult(segue);
        }

        protected virtual async Task ExecuteAnimationSafeAsync(Task animationTask)
        {
            try
            {
                if (_animations == 0)
                    return;

                await _semaphore.WaitAsync();

                if (_animations == 0)
                    return;

                Interlocked.Decrement(ref _animations);

                Console.WriteLine("ExecuteAnimationSafeAsync : " + DateTime.UtcNow.ToFileTimeUtc());
                await animationTask;
            }
            finally
            {
                Interlocked.Increment(ref _animations);
                _semaphore.Release();
            }
        }


    }

    public abstract class BaseViewController<T> : BaseViewController
        where T : BaseViewModel
    {
        private T _vm;

        public T ViewModel => _vm;

        public object Parameter { get; set; }

        protected BaseViewController(IntPtr handle) : base(handle)
        {
        }

        protected virtual void SubscribeToViewModelEvents()
        {
            UnsubscribeFromViewModelEvents();

            _vm.PropertyChanged += ViewModelPropertyChanged;
        }

        protected virtual void UnsubscribeFromViewModelEvents()
        {
            _vm.PropertyChanged -= ViewModelPropertyChanged;
        }

        public override void ViewDidLoad()
        {
            _vm = ServiceLocator.Current.GetInstance<T>();
            Cleanup();

            base.ViewDidLoad();

            SubscribeToViewModelEvents();

            _vm.Start(Parameter);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
        }

        public override void ViewWillUnload()
        {
            base.ViewWillUnload();

            UnsubscribeFromViewModelEvents();
        }

        protected virtual void OnViewModelPropertyChanged(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(ViewModel.IsBusy):
                    UIApplication.SharedApplication.NetworkActivityIndicatorVisible = ViewModel.IsBusy;
                    if (ViewModel.IsBusy)
                    {
                        DisableControls();
                        ShowLoader();
                    }
                    else
                    {
                        EnableControls();
                        HideLoader();
                    }
                    break;
            }
        }

        private void ViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnViewModelPropertyChanged(e.PropertyName);
        }

        public override void Cleanup()
        {
            base.Cleanup();
            UnsubscribeFromViewModelEvents();
            _vm?.Cleanup();
        }

        public override bool PrefersStatusBarHidden()
        {
            return false;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                Cleanup();

            base.Dispose(disposing);
        }
    }
}
