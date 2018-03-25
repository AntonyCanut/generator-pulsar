using System;
using System.Globalization;
using <%= name %>.iOS.Services;
using <%= name %>.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(Localize))]

namespace <%= name %>.iOS.Services
{
    public class Localize : ILocalizationService
    {
        public void SetLocale(CultureInfo ci) { }
        public System.Globalization.CultureInfo GetCurrentCultureInfo()
        {
            return CultureInfo.CurrentUICulture;
        }
    }
}
