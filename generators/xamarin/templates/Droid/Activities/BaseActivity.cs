using System;
using System.ComponentModel;
using Android.App;
using CommonServiceLocator;
using <%= name %>.ViewModels;

namespace <%= name %>.Droid.Activities
{
    public abstract class BaseActivity<T> : Activity
        where T : BaseViewModel
    {
        protected T ViewModel { get; set; }
		protected abstract int Layout { get; }

        public BaseActivity()
        {
            ViewModel = ServiceLocator.Current.GetInstance<T>();
        }

        protected override void OnCreate(Android.OS.Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Layout);

            SetupControls();
            SubscribeToViewEvents();
            SubscribeToViewModelEvents();
            InitializeViewModel();
        }

        protected override void OnResume()
        {
            base.OnResume();

            SubscribeToViewEvents();
            SubscribeToViewModelEvents();
        }

        protected override void OnPause()
        {
            UnsubscribeFromViewEvents();
            UnsubscribeFromViewModelEvents();

            base.OnPause();
        }

        protected override void OnDestroy()
        {
            UnsubscribeFromViewEvents();
            UnsubscribeFromViewModelEvents();

            base.OnDestroy();
        }

        protected virtual void SubscribeToViewEvents()
        {
            UnsubscribeFromViewEvents();
        }

        protected virtual void UnsubscribeFromViewEvents()
        {

        }

        protected virtual void SubscribeToViewModelEvents()
        {
            UnsubscribeFromViewModelEvents();

            ViewModel.PropertyChanged += OnViewModelPropertyChanged;
        }

        protected virtual void UnsubscribeFromViewModelEvents()
        {
            ViewModel.PropertyChanged -= OnViewModelPropertyChanged;
        }

        protected virtual void OnViewModelPropertyChanged(string propertyName)
        {

        }

        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnViewModelPropertyChanged(e.PropertyName);
        }

        protected virtual void SetupControls()
        {

        }

        protected virtual void InitializeViewModel()
        {

        }
    }
}
