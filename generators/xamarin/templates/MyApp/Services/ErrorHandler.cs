using System;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using Toolkit.ErrorManagement;
using Toolkit.ViewServices;

namespace <%= name %>.Services
{
    public class ErrorHandler : IErrorHandler
    {
        private readonly IDialogService _dialogService;

        public ErrorHandler(IDialogService dialogService)
        {
            _dialogService = dialogService;
        }

        public Task HandleErrorAsync(Exception ex, CancellationToken ct)
        {
            //Analytics.TrackEvent("Exception : " + ex.Message);
            return Task.FromResult(0);
        }

        public async Task<bool> HandleErrorWithRetryAsync(Exception ex, CancellationToken ct)
        {
            // TODO : Localize
            var result = await _dialogService.ShowAsync("Title Error", "Something went wrong", "Retry", "Cancel", ct);
            //var result = await _dialogService.ShowAsync(Strings.Error, Strings.Oops, Strings.Retry, Strings.Cancel, ct);
            if (result)
                return true;

            //Analytics.TrackEvent("Exception : " + ex.Message);
            return false;
        }
    }
}
