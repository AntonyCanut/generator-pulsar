using System;
using System.Threading;
using System.Threading.Tasks;
using Toolkit.ViewServices;
using Xamarin.Forms;

namespace <%= name %>.ViewServices
{
    public class DialogService : IDialogService
    {
        public Task ShowAsync(string title, string content, string cancelContent, CancellationToken ct)
        {
            return Application.Current.MainPage.DisplayAlert(title, content, cancelContent);
        }

        public Task<bool> ShowAsync(string title, string content, string okContent, string cancelContent, CancellationToken ct)
        {
            return Application.Current.MainPage.DisplayAlert(title, content, okContent, cancelContent);
        }
    }
}
