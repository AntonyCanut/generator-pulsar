using System;
using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;
using <%= name %>.IOC;

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
            base.Setup(simpleIoc);
        }
    }
}
