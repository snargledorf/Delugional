using System;
using System.Threading.Tasks;
using Delugional;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DelugionalTests
{
    [TestClass]
    public class DelugeTests
    {
        [TestMethod]
        public async Task AddTorrentFile()
        {
            using (var connection = new MockDelugeConnection())
            {
                var deluge = new Deluge(connection);
                string torrentId = await deluge.AddTorrentFile("torrent_file", "torrent_file_contents");

                Assert.IsNotNull(torrentId, "torrentId != null");
            }
        }
    }
}
