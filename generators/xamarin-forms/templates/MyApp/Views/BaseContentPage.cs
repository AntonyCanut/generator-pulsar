using System;
using System.ComponentModel;
using CommonServiceLocator;
using <%= name %>.ViewModels;
using Xamarin.Forms;

namespace <%= name %>.Views
{
    public abstract class BaseContentPage : ContentPage
    {

    }

    public abstract class BaseContentPage<T> : BaseContentPage
        where T : BaseViewModel
    {
        protected T ViewModel { get; set; }

        protected BaseContentPage()
        {
            ViewModel = ServiceLocator.Current.GetInstance<T>();
            BindingContext = ViewModel;
            ViewModel.PropertyChanged += ViewModelOnPropertyChanged;
        }

        private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ViewModel_OnPropertyChanged(e.PropertyName);
        }

        protected virtual void ViewModel_OnPropertyChanged(string propertyName)
        {

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ViewModel.Start();
        }
    }
}
