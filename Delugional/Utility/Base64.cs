using System;
using System.Text;

namespace Delugional.Utility
{
    internal static class Base64
    {
        internal static string Encode(string s)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(s);
            return Convert.ToBase64String(bytes);
        }

        internal static string Decode(string s)
        {
            byte[] bytes = Convert.FromBase64String(s);
            return Encoding.UTF8.GetString(bytes);
        }
    }
}
