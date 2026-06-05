using System;

namespace polnuyaPetch.Security
{
    public static class AccessControl
    {
        public static bool IsAdmin(string? role)
        {
            return string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase);
        }

        public static void RequireAdmin(string? role)
        {
            if (!IsAdmin(role))
            {
                throw new ArgumentException("Недостаточно прав. Требуется роль: Admin");
            }
        }
    }
}
