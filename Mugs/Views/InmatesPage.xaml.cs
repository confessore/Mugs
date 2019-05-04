using Mugs.Models;
using Mugs.ViewModels;
using System;
using System.ComponentModel;
using Xamarin.Forms;

namespace Mugs.Views
{
    [DesignTimeVisible(true)]
    public partial class InmatesPage : ContentPage
    {
        InmatesViewModel viewModel;

        public InmatesPage()
        {
            InitializeComponent();

            BindingContext = viewModel = new InmatesViewModel();
        }

        async void OnInmateSelected(object sender, SelectedItemChangedEventArgs args)
        {
            var inmate = args.SelectedItem as Inmate;
            if (inmate == null)
                return;

            await Navigation.PushAsync(new InmateDetailPage(new InmateDetailViewModel(inmate)));

            // Manually deselect item.
            ItemsListView.SelectedItem = null;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (viewModel.Inmates.Count == 0)
                viewModel.LoadInmatesCommand.Execute(null);
        }
    }
}
