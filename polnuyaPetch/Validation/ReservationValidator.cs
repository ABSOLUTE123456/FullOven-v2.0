using polnuyaPetch.Models;

namespace polnuyaPetch.Validation
{
    public static class ReservationValidator
    {
        public static string? Validate(Reservation res)
        {
            if (string.IsNullOrWhiteSpace(res.Name))
                return "Имя не может быть пустым";

            if (res.Name.Length > 50)
                return "Имя слишком длинное";

            if (res.Guests < 1 || res.Guests > 20)
                return "Количество гостей должно быть от 1 до 20";

            return null; // Ошибок нет
        }
    }
}
