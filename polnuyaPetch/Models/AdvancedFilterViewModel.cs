namespace FullOven.Models
{
    public class AdvancedFilterViewModel
    {
        public string? SearchText { get; set; }
        public string? Status { get; set; } // "Any", "New", "InProgress", "Done"
    }
}
