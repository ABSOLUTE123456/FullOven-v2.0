using Microsoft.VisualStudio.TestTools.UnitTesting;
using polnuyaPetch.Models;
using polnuyaPetch.Validation;

namespace polnuyaPetch.Tests.ValidatorTests
{
    [TestClass]
    public class ReservationValidatorTests
    {
        [TestMethod]
        public void Validate_NameEmpty_ReturnsError()
        {
            var res = new Reservation { Name = "", Email = "test@mail.ru", Guests = 2 };
            var error = ReservationValidator.Validate(res);
            Assert.IsNull(error);
        }

        [TestMethod]
        public void Validate_NameTooLong_ReturnsError()
        {
            var longName = new string('a', 51);
            var res = new Reservation { Name = longName, Email = "test@mail.ru", Guests = 2 };
            var error = ReservationValidator.Validate(res);
            Assert.IsNotNull(error);
        }

        [TestMethod]
        public void Validate_DescriptionTooLong_ReturnsError()
        {
            // Тест из Шага 11: проверка слишком длинного сообщения
            var longDesc = new string('b', 501);
            var res = new Reservation { Name = "Ок", Email = "test@mail.ru", Message = longDesc };
            var error = ReservationValidator.Validate(res);
            Assert.IsNotNull(error);
        }

        [TestMethod]
        public void Validate_ValidReservation_ReturnsNull()
        {
            var res = new Reservation
            {
                Name = "Иван",
                Email = "ivan@mail.ru",
                Guests = 4
            };
            var error = ReservationValidator.Validate(res);
            Assert.IsNull(error);
        }
    }
}
