using System;
using polnuyaPetch.Models;

namespace polnuyaPetch.DataMigrations
{
    public static class MenuNormalizer
    {
        public static bool Normalize(MenuItem item)
        {
            bool changed = false;

            if (item.Name == null)
            {
                item.Name = "";
                changed = true;
            }
            if (item.Description == null)
            {
                item.Description = "";
                changed = true;
            }

            var nameTrim = item.Name.Trim();
            if (nameTrim != item.Name)
            {
                item.Name = nameTrim;
                changed = true;
            }

            var descTrim = item.Description.Trim();
            if (descTrim != item.Description)
            {
                item.Description = descTrim;
                changed = true;
            }

            if (item.Name.Length > 50)
            {
                item.Name = item.Name.Substring(0, 50);
                changed = true;
            }

            if (item.Description.Length > 200)
            {
                item.Description = item.Description.Substring(0, 200);
                changed = true;
            }

            return changed;
        }
    }
}
