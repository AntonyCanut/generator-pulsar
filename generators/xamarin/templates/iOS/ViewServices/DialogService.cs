using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Toolkit.ViewServices;
using UIKit;

namespace <%= name %>.iOS.Services
{
    public class DialogService : IDialogService
    {
        public Task ShowAsync(string title, string content, string cancelContent, CancellationToken ct)
        {
            UIAlertController alertNoPhone = UIAlertController.Create(title, content, UIAlertControllerStyle.Alert);
            alertNoPhone.AddAction(UIAlertAction.Create(cancelContent, UIAlertActionStyle.Default, null));

            var window = UIApplication.SharedApplication.KeyWindow;
            var vc = window.RootViewController;
            while (vc.PresentedViewController != null)
            {
                vc = vc.PresentedViewController;
                vc.PresentViewController(alertNoPhone, true, null);
            }

            var navController = vc as UINavigationController;
            if (navController != null)
            {
                vc = navController.ViewControllers.Last();
                vc.PresentViewController(alertNoPhone, true, null);
            }

            return Task.FromResult(0);
        }

        public Task<bool> ShowAsync(string title, string content, string retryContent, string cancelContent, CancellationToken ct)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();
            UIAlertController alertNoPhone = UIAlertController.Create(title, content, UIAlertControllerStyle.Alert);
            alertNoPhone.AddAction(UIAlertAction.Create(cancelContent, UIAlertActionStyle.Default, a => taskCompletionSource.SetResult(false)));
            alertNoPhone.AddAction(UIAlertAction.Create(retryContent, UIAlertActionStyle.Default, a => taskCompletionSource.SetResult(false)));

            var window = UIApplication.SharedApplication.KeyWindow;
            var vc = window.RootViewController;
            while (vc.PresentedViewController != null)
            {
                vc = vc.PresentedViewController;
                vc.PresentViewController(alertNoPhone, true, null);
            }

            var navController = vc as UINavigationController;
            if (navController != null)
            {
                vc = navController.ViewControllers.Last();
                vc.PresentViewController(alertNoPhone, true, null);
            }

            return Task.FromResult(taskCompletionSource.Task.Result);
        }
    }
}
