using Mugs.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Mugs.ViewModels
{
    public class InmatesViewModel : BaseViewModel
    {
        public ObservableCollection<Inmate> Inmates { get; set; }
        public Command LoadInmatesCommand { get; set; }

        public InmatesViewModel()
        {
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
                var inmates = new List<Inmate>();
                string json = string.Empty;
                using (var client = new HttpClient())
                {
                    json = await client.GetStringAsync("http://10.0.2.2/api/values/");
                    inmates = JsonConvert.DeserializeObject<List<Inmate>>(json);
                }
                foreach (var inmate in inmates)
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
