using System;

namespace polnuyaPetch.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateTime BookingDate { get; set; }
        public int GuestsCount { get; set; }
    }
}
