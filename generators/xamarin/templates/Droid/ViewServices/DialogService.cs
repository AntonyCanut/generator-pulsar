using System;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Toolkit.ViewServices;

namespace <%= name %>.Droid.ViewServices
{
    public class DialogService : IDialogService
    {
        public Context Context { get; set; }

        public Task<bool> ShowAsync(string title, string content, string retryContent, string cancelContent, CancellationToken ct)
        {
            if (Context == null)
            {
                return Task.FromResult(false);
            }

            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

            var builder = new AlertDialog.Builder(Context)
                                         .SetTitle(title)
                                         .SetMessage(content)
                                         .SetPositiveButton(retryContent, (sender, e) => tcs.TrySetResult(true))
                                         .SetNegativeButton(cancelContent, (sender, e) => tcs.TrySetResult(false));
            var dialog = builder.Create();
            dialog.Show();

            return tcs.Task;
        }

        public Task ShowAsync(string title, string content, string cancelContent, CancellationToken ct)
        {
            if (Context == null)
            {
                return Task.FromResult(false);
            }

            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

            var builder = new AlertDialog.Builder(Context)
                                         .SetTitle(title)
                                         .SetMessage(content)
                                         .SetNegativeButton(cancelContent, (sender, e) => tcs.TrySetResult(false));
            var dialog = builder.Create();
            dialog.Show();

            return tcs.Task;
        }
    }
}