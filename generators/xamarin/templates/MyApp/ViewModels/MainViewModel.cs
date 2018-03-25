using System;
using System.Threading;
using System.Threading.Tasks;
using Toolkit.ErrorManagement;
using Toolkit.ViewServices;

namespace <%= name %>.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public MainViewModel(IDialogService dialogService,
                                IErrorHandler errorHandler) : base(dialogService, errorHandler)
        {
        }

        protected override async Task StartAsync(CancellationToken ct)
        {

        }
    }
}
