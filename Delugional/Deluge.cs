using System.Collections.Generic;
using System.Threading.Tasks;

namespace Delugional
{
    public sealed class Deluge
    {
        private readonly IDelugeConnection connection;

        public Deluge(IDelugeConnection connection)
        {
            this.connection = connection;
        }

        public Task<string> AddTorrentFile(string fileName, string fileContents, Dictionary<object, object> options = null)
        {
            return Task.FromResult(string.Empty);
        }
    }
}
