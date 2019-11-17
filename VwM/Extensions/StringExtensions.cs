using System;
using System.IO;
using System.Linq;
using System.Text;

namespace VwM.Extensions
{
    public static class StringExtensions
    {
        public static bool? ParseBool(this string value)
        {
            if (value.Length == 1)
            {
                if (value == "0") return false;
                if (value == "1") return true;
            }

            if (bool.TryParse(value, out bool boolean))
                return boolean;

            return null;
        }


        public static bool ParseBool(this string value, bool defaultValue)
        {
            var result = ParseBool(value);
            return result.HasValue ? (bool)result : defaultValue;
        }


        public static string ToUpperFirst(this string value)
        {
            if (value == null || value.Length == 0)
                return value;

            char[] a = value.ToCharArray();
            a[0] = char.ToUpper(a[0]);

            return new string(a);
        }
    }
}
