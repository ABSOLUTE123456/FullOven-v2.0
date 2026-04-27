using System.ComponentModel.DataAnnotations;

namespace polnuyaPetch.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required] public string Login { get; set; } = string.Empty;
        [Required] public string Password { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? AvatarPath { get; set; } = "/images/default-avatar.png";

    }
}
