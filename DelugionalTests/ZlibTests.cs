using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Delugional.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using rencodesharp;

namespace DelugionalTests
{
    [TestClass]
    public class ZlibTests
    {
        [TestMethod]
        public void Deflate()
        {
            string hello = "hello world";
            byte[] bytes = Zlib.Deflate(hello);
            string base64 = Base64.Encode(bytes);
        }

        [TestMethod]
        public void DeflateRencode()
        {
            var list = "hello_world";

            string encode = Rencode.Encode(list);
            byte[] encodedBytes = encode.Select(Convert.ToByte).ToArray();

            byte[] bytes = Zlib.Deflate(encodedBytes);
            string base64 = Base64.Encode(bytes);
        }
    }
}
