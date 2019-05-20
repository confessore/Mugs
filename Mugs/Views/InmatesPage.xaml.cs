using Mugs.Items;
using Mugs.ViewModels;
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

        public InmatesPage(string url)
        {
            InitializeComponent();

            BindingContext = viewModel = new InmatesViewModel(url);
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

        void OnItemAppearing(object sender, ItemVisibilityEventArgs args)
        {
            if (args.ItemIndex == viewModel.Inmates.Count - 1)
            {
                var tmp = viewModel.HtmlParser.PartialParseInmatesOnNextPage(viewModel.URL, viewModel.Document);
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
