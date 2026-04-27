using System.ComponentModel.DataAnnotations;

namespace polnuyaPetch.Models
{
    public class Reservation
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Введите имя")]
        public string Name { get; set; } = string.Empty;

        public string? LastName { get; set; }

        [Required(ErrorMessage = "Введите Email")]
        [EmailAddress(ErrorMessage = "Некорректный формат Email")]
        public string Email { get; set; } = string.Empty;

        [Range(1, 20, ErrorMessage = "Кол-во гостей должно быть от 1 до 20")]
        public int Guests { get; set; }

        [Required(ErrorMessage = "Выберите дату")]
        public string Date { get; set; } = string.Empty;

        [Required(ErrorMessage = "Выберите время")]
        public string Time { get; set; } = string.Empty;

        public string? Message { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
