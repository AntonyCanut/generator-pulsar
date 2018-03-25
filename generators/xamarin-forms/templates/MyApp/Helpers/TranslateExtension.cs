using System;
using System.Globalization;
using System.Reflection;
using System.Resources;
using <%= name %>.Resources;
using <%= name %>.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace <%= name %>.Helpers
{
    [ContentProperty("Text")]
    public class TranslateExtension : IMarkupExtension
    {
        private readonly CultureInfo _ci;
        private static readonly string ResourceId = typeof(Strings).FullName;

        private static readonly Lazy<ResourceManager> ResMgr = new Lazy<ResourceManager>(() => new ResourceManager(ResourceId
            , typeof(TranslateExtension).GetTypeInfo().Assembly));

        public TranslateExtension()
        {
            _ci = DependencyService.Get<ILocalizationService>().GetCurrentCultureInfo();
        }

        public string Text { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Text == null)
                return string.Empty;

            var translation = ResMgr.Value.GetString(Text, _ci);

            if (translation == null)
            {
#if DEBUG
                throw new ArgumentException(
                    $"Key '{Text}' was not found in resources '{ResourceId}' for culture '{_ci.Name}'.", Text);
#else
                translation = Text; // returns the key, which GETS DISPLAYED TO THE USER
#endif
            }
            return translation;
        }
    }
}
