using System.IO;
using System.Text;
using Ionic.Zlib;
using CompressionMode = System.IO.Compression.CompressionMode;
using DeflateStream = System.IO.Compression.DeflateStream;

namespace Delugional.Utility
{
    public static class Zlib
    {
        public static byte[] Inflate(byte[] bytes, int offset, int length)
        {
            using (var ms = new MemoryStream())
            {
                using (var inflateStream = new ZlibStream(ms, Ionic.Zlib.CompressionMode.Decompress))
                {
                    inflateStream.Write(bytes, 0, length);
                }

                return ms.ToArray();
            }
        }

        public static byte[] Inflate(byte[] bytes)
        {
            return Inflate(bytes, 0, bytes.Length);
        }

        public static byte[] Deflate(string s)
        {
            return Deflate(s, Encoding.UTF8);
        }

        public static byte[] Deflate(string s, Encoding encoding)
        {
            byte[] bytes = encoding.GetBytes(s);
            return Deflate(bytes);
        }

        public static byte[] Deflate(byte[] bytes)
        {
            return Deflate(bytes, 0, bytes.Length);
        }

        public static byte[] Deflate(byte[] bytes, int offset, int length)
        {
            using (var ms = new MemoryStream())
            {
                using (var deflateStream = new ZlibStream(ms, Ionic.Zlib.CompressionMode.Compress))
                {
                    deflateStream.Write(bytes, offset, length);
                }

                return ms.ToArray();
            }
        }
    }
}
