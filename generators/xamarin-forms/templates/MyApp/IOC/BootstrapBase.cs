using GalaSoft.MvvmLight.Ioc;
using <%= name %>.Services;
using <%= name %>.ViewModels;
using <%= name %>.ViewServices;
using Toolkit.ErrorManagement;
using Toolkit.ViewServices;

namespace <%= name %>.IOC
{
    public abstract class BootstrapBase
    {
        public abstract void Start();

        protected virtual void Setup(ISimpleIoc simpleIoc)
        {
            simpleIoc.Register<IDialogService, DialogService>();
            simpleIoc.Register<IErrorHandler, ErrorHandler>();

            simpleIoc.Register<MainViewModel>();
        }
    }
}
