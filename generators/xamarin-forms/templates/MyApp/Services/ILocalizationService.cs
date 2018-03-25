using System.Globalization;

namespace <%= name %>.Services
{
    public interface ILocalizationService
    {
        CultureInfo GetCurrentCultureInfo();
        void SetLocale(CultureInfo ci);
    }
}
