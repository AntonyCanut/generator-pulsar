using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace <%= name %>.Resources
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Colors : ResourceDictionary
    {
        public Colors()
        {
            InitializeComponent();
        }

        public static Color <%= name %>Yellow => (Color)Application.Current.Resources[nameof(<%= name %>Yellow)];
    }
}
