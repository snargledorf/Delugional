using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using rencodesharp;

namespace DelugionalTests
{
    [TestClass]
    public class RencodeTests
    {
        [TestMethod]
        public void EncodeList()
        {
            var list = new List<object>
            {
                1,
                "hello_world",
                new object[] {"foo", "bar"}
            };

            string encode = Rencode.Encode(list);
        }

        [TestMethod]
        public void DecodeList()
        {
            var list = new List<object>
            {
                1,
                "hello_world",
                new object[] {"foo", "bar"}
            };

            string encode = Rencode.Encode(list);

            object decode = Rencode.Decode(encode);
        }
    }
}
