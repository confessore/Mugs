using HtmlAgilityPack;
using Mugs.Models;
using Mugs.Services;
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

        async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            var inmate = (Inmate)args.SelectedItem;
            if (inmate == null)
                return;
            inmate = viewModel.HtmlParser.IndividualInmateParse(inmate);

            await Navigation.PushAsync(new InmateDetailPage(new InmateDetailViewModel(inmate)));

            // Manually deselect item.
            ItemsListView.SelectedItem = null;
        }

        async void OnItemAppearing(object sender, ItemVisibilityEventArgs args)
        {
            if (args.ItemIndex + 1 == viewModel.Inmates.Count)
            {
                var tmp = viewModel.HtmlParser.PartialParseInmatesOnNextPage(viewModel.url, viewModel.Document);
                foreach (var inmate in tmp.Item1)
                    viewModel.Inmates.Add(inmate);
                viewModel.Document = tmp.Item2;
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (viewModel.Inmates.Count == 0)
                viewModel.LoadInmatesCommand.Execute(null);
        }
    }
}
