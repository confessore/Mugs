using Mugs.Models;
using Mugs.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace Mugs.Views
{
    [DesignTimeVisible(true)]
    public partial class InmateDetailPage : ContentPage
    {
        InmateDetailViewModel viewModel;

        public InmateDetailPage(InmateDetailViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = this.viewModel = viewModel;
        }

        public InmateDetailPage()
        {
            InitializeComponent();

            var inmate = new Inmate
            {
                Name = "Inmate"
            };

            viewModel = new InmateDetailViewModel(inmate);
            BindingContext = viewModel;
        }
    }
}
