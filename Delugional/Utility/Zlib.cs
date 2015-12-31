using System;
using System.IO;
using System.Text;
using Ionic.Zlib;

namespace Delugional.Utility
{
    internal static class Zlib
    {
        public static byte[] Decompress(byte[] bytes, int offset, int length)
        {
            using (var ms = new MemoryStream())
            {
                using (var zlib = new ZlibStream(ms, CompressionMode.Decompress))
                {
                    zlib.Write(bytes, offset, length);
                }

                return ms.ToArray();
            }
        }

        public static byte[] Decompress(byte[] bytes)
        {
            return Decompress(bytes, 0, bytes.Length);
        }

        public static byte[] Compress(string s)
        {
            return ZlibStream.CompressString(s);
        }
    }
}
