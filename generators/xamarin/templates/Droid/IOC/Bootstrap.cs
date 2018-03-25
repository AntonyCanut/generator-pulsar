using System;
using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;
using <%= name %>.Droid.ViewServices;
using <%= name %>.IOC;
using Toolkit.ViewServices;

namespace <%= name %>.Droid.IOC
{
    public class Bootstrap : BootstrapBase
    {
        private readonly ISimpleIoc _simpleIoc;

        public Bootstrap(ISimpleIoc simpleIoc)
        {
            _simpleIoc = simpleIoc;
            ServiceLocator.SetLocatorProvider(() => simpleIoc);
        }

        public override void Start()
        {
            Setup(_simpleIoc);
        }

        protected override void Setup(ISimpleIoc simpleIoc)
        {
            simpleIoc.Register<IDialogService, DialogService>();

            base.Setup(simpleIoc);
        }
    }
}
