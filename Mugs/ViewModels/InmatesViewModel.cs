using HtmlAgilityPack;
using Mugs.Items;
using Mugs.Views;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Mugs.ViewModels
{
    public class InmatesViewModel : BaseViewModel
    {
        public INavigation Navigation { get; set; }
        public ObservableCollection<Inmate> Inmates { get; set; }
        public Command LoadInmatesCommand { get; set; }
        public Command SearchInmatesCommand { get; set; }
        public HtmlDocument Document { get; set; }
        public string FormData { get; set; }

        public string URL = "http://www.mugshotsocala.com/";
        public string URI { get; set; }
        readonly string searchUri = "Default.aspx?searchString=";
        readonly string daysUri = "&days=90";

        public InmatesViewModel()
        {
            Title = "Browse";
            Inmates = new ObservableCollection<Inmate>();
            LoadInmatesCommand = new Command(async () => await ExecuteLoadInmatesCommand());
            SearchInmatesCommand = new Command<string>(async x => await ExecuteSearchInmatesCommand(x));
        }

        public InmatesViewModel(string url, string uri = "")
        {
            URL = url;
            URI = uri;
            Title = "Browse";
            Inmates = new ObservableCollection<Inmate>();
            LoadInmatesCommand = new Command(async () => await ExecuteLoadInmatesCommand());
            SearchInmatesCommand = new Command<string>(async x => await ExecuteSearchInmatesCommand(x));
        }

        async Task ExecuteLoadInmatesCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                Inmates.Clear();
                Document = string.IsNullOrEmpty(URI) ?
                    HtmlParser.GetHtmlDocumentFromUrl(URL) :
                    HtmlParser.GetHtmlDocumentFromUrl(URL + URI);
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

        async Task ExecuteSearchInmatesCommand(string name)
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                await Navigation.PushAsync(new InmatesPage(URL, searchUri + name.Replace(" ", "+") + daysUri));
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
