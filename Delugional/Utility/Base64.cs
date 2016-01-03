using System;
using System.Text;

namespace Delugional.Utility
{
    public static class Base64
    {
        public static string Encode(string s)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(s);
            return Encode(bytes);
        }

        public static string Encode(byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }

        public static string Decode(string s)
        {
            byte[] bytes = Convert.FromBase64String(s);
            return Encoding.UTF8.GetString(bytes);
        }
    }
}
