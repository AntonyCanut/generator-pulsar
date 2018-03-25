using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using <%= name %>.iOS.IOC;
using UIKit;

namespace <%= name %>.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();

            LoadApplication(new App(new Bootstrap(GalaSoft.MvvmLight.Ioc.SimpleIoc.Default)));

            return base.FinishedLaunching(app, options);
        }
    }
}
