using HtmlAgilityPack;
using Mugs.Models;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Mugs.ViewModels
{
    public class InmatesViewModel : BaseViewModel
    {
        public ObservableCollection<Inmate> Inmates { get; set; }
        public Command LoadInmatesCommand { get; set; }
        public HtmlDocument Document { get; set; }
        public string FormData { get; set; }

        public string URL = "http://www.mugshotsocala.com/";

        public InmatesViewModel()
        {
            Title = "Browse";
            Inmates = new ObservableCollection<Inmate>();
            LoadInmatesCommand = new Command(async () => await ExecuteLoadInmatesCommand());
        }

        public InmatesViewModel(string url)
        {
            URL = url;
            Title = "Browse";
            Inmates = new ObservableCollection<Inmate>();
            LoadInmatesCommand = new Command(async () => await ExecuteLoadInmatesCommand());
        }

        async Task ExecuteLoadInmatesCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                Inmates.Clear();
                Document = HtmlParser.GetHtmlDocumentFromUrl(URL);
                foreach (var inmate in HtmlParser.PartialParseInmatesOnPage(URL, Document))
                    Inmates.Add(inmate);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
