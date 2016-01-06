using System.Threading.Tasks;
using Delugional;
using Delugional.Rpc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DelugionalTests
{
    [TestClass]
    public class DelugeConnectionV3Tests
    {
        [TestMethod]
        public async Task Connect()
        {
            using (var connection = new DelugeRpcConnectionV3())
            {
                await connection.OpenAsync();
                
                Assert.IsTrue(connection.IsOpen, "connection.IsOpen");

                connection.Close();

                Assert.IsFalse(connection.IsOpen, "connection.IsOpen");
            }
        }

        [TestMethod]
        public async Task Login()
        {
            using (var connection = new DelugeRpcConnectionV3())
            {
                await connection.OpenAsync();
                
                await connection.Send("daemon.login", Resources.Username, Resources.Password);

                object[] result = await connection.Receive();

                Assert.IsNotNull(result, "result != null");
                Assert.AreEqual(3, result.Length, "result.Length == 3");
                Assert.AreEqual(1, result[0], "Message type should be 1 (Response)");
                Assert.AreEqual(10, result[2], "Auth level should be 10 (Admin)");
            }
        }
    }
}
