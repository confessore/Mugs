using Mugs.Models;

namespace Mugs.ViewModels
{
    public class InmateDetailViewModel : BaseViewModel
    {
        public Inmate Inmate { get; set; }
        public InmateDetailViewModel(Inmate inmate = null)
        {
            Title = inmate?.Name;
            Inmate = inmate;
        }
    }
}
