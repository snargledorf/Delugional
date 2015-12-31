using System.Text;
using Ionic.Zlib;

namespace Delugional.Utility
{
    internal static class Zlib
    {
        public static string Decompress(string s)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(s);
            return ZlibStream.UncompressString(bytes);
        }

        public static string Compress(string s)
        {
            byte[] bytes = ZlibStream.CompressString(s);
            return Encoding.UTF8.GetString(bytes);
        }
    }
}
