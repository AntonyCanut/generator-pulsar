using Android.App;
using Android.Widget;
using Android.OS;
using <%= name %>.Resources;
using <%= name %>.Droid.Activities;
using <%= name %>.ViewModels;

namespace <%= name %>.Droid
{
    [Activity(Label = "<%= name %>", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MainActivity : BaseActivity<MainViewModel>
    {
        protected override int Layout => Resource.Layout.Main;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        protected override void SetupControls()
        {
            base.SetupControls();

            TextView button = FindViewById<TextView>(Resource.Id.helloWorld);
			
			button.Text = Strings.HelloWorld;
        }
	}
}

