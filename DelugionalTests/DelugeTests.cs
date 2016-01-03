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
            using (var deluge = new Deluge())
            {
                await deluge.OpenAsync();

                AuthLevels b = await deluge.LoginAsync("localclient", "8a9bc4597feb7e2b8669104aeb462fc204d31901");
                string torrentId = await deluge.AddTorrentFileAsync("torrent_file", new byte[]{1,2,3,4});

                Assert.IsNotNull(torrentId, "torrentId != null");
            }
        }

        [TestMethod]
        public async Task AddMagnetLink()
        {
            using (var deluge = new Deluge())
            {
                await deluge.OpenAsync();

                await deluge.LoginAsync("localclient", "8a9bc4597feb7e2b8669104aeb462fc204d31901");

                string torrentId = await deluge.AddMagnetAsync("magnet:?xt=urn:btih:3F19B149F53A50E14FC0B79926A391896EABAB6F&dn=ubuntu+15+10+desktop+64+bit&tr=udp%3A%2F%2Ftracker.publicbt.com%2Fannounce&tr=udp%3A%2F%2Fglotorrents.pw%3A6969%2Fannounce");
            }
        }
    }
}
