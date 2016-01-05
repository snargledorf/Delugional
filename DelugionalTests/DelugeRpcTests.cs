using System.Collections.Generic;
using System.Threading.Tasks;
using Delugional;
using Delugional.Rpc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DelugionalTests
{
    [TestClass]
    public class DelugeRpcTests
    {
        private IDelugeRpcConnection connection;

        [TestInitialize]
        public void Initialize()
        {
            connection = new DelugeRpcConnectionV3();
            connection.OpenAsync().Wait();
        }

        [TestCleanup]
        public void Cleanup()
        {
            connection?.Close();
            connection?.Dispose();
            connection = null;
        }

        [TestMethod]
        public async Task AddTorrentFile()
        {
            using (var deluge = new DelugeRpc(connection))
            {
                await deluge.LoginAsync(Resources.Username, Resources.Password);

                string torrentId = await deluge.AddTorrentAsync("torrent_file", new byte[] {1, 2, 3, 4});

                await deluge.RemoveTorrentAsync(torrentId);

                Assert.IsNotNull(torrentId, "torrentId != null");
            }
        }

        [TestMethod]
        public async Task AddMagnetLink()
        {
            using (var deluge = new DelugeRpc(connection))
            {
                await deluge.LoginAsync(Resources.Username, Resources.Password);

                string torrentId = await deluge.AddMagnetAsync(Resources.MagnetLink1);

                await deluge.RemoveTorrentAsync(torrentId, true);

                Assert.IsNotNull(torrentId, "torrentId != null");
            }
        }

        [TestMethod]
        public async Task GetTorrentStatus()
        {
            using (var deluge = new DelugeRpc(connection))
            {
                await deluge.LoginAsync(Resources.Username, Resources.Password);

                string torrentId = await deluge.AddMagnetAsync(Resources.MagnetLink1);

                try
                {
                    IDictionary<string, object> status = await deluge.GetTorrentStatusAsync(torrentId, new []{BuiltInStatuses.Name, BuiltInStatuses.ActiveTime});
                    
                    Assert.IsNotNull(status, "status != null");
                    Assert.AreEqual(2, status.Count, "status.Count == 2");
                    Assert.IsTrue(status.ContainsKey(BuiltInStatuses.Name), "status.ContainsKey(BuiltInStatuses.Name)");
                    Assert.IsTrue(status.ContainsKey(BuiltInStatuses.ActiveTime), "status.ContainsKey(BuiltInStatuses.ActiveTime)");
                }
                finally
                {
                    await deluge.RemoveTorrentAsync(torrentId, true);
                }
            }
        }
    }
}
