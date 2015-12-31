using System;
using System.Net;
using System.Threading.Tasks;
using Delugional;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DelugionalTests
{
    [TestClass]
    public class DelugeConnectionTests
    {
        [TestMethod]
        public async Task Connect()
        {
            using (var connection = new DelugeConnection())
            {
                await connection.Open();
            }
        }
    }
}
