using System.Collections.Generic;

namespace Mugs.Models
{
    public class Inmate
    {
        public Inmate()
        {
            Charges = new List<Charge>();
        }

        public uint BookingNumber { get; set; }
        public string Name { get; set; }
        public string DateOfBooking { get; set; }
        public string County { get; set; }
        public string DateOfBirth { get; set; }
        public uint Age { get; set; }
        public string Gender { get; set; }
        public string Race { get; set; }
        public List<Charge> Charges { get; set; }
        public string ImageUrl { get; set; }
        public string BookingUrl { get; set; }
    }
}
