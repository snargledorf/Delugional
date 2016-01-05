using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Delugional
{
    public interface IDeluge : IDisposable
    {
        Task<string> AddTorrentAsync(string fileName, byte[] fileDump, IDictionary<string, object> options = null);
        Task<string> AddMagnetAsync(string url, IDictionary<string, object> options = null);

        Task<bool> RemoveTorrentAsync(string torrentId, bool removeData = false);
        Task<object[]> RemoveTorrentsAsync(string[] torrentIds, bool removeData = false);

        Task<IDictionary<string, object>> GetTorrentStatusAsync(string torrentId, string[] statusKeys = null, bool diff = false);
        Task<IDictionary<string, IDictionary<string, object>>> GetTorrentsStatusAsync(Filter filter = null, string[] statusKeys = null, bool diff = false);
    }

    public abstract class Deluge : IDeluge
    {
        private bool disposed;

        public virtual Task<string> AddTorrentAsync(string fileName, byte[] fileDump, IDictionary<string, object> options = null)
        {
            throw new NotImplementedException();
        }

        public virtual Task<string> AddMagnetAsync(string url, IDictionary<string, object> dictionary = null)
        {
            throw new NotImplementedException();
        }

        public virtual Task<bool> RemoveTorrentAsync(string torrentId, bool removeData = false)
        {
            throw new NotImplementedException();
        }

        public virtual async Task<object[]> RemoveTorrentsAsync(string[] torrentIds, bool removeData = false)
        {
            var errors = new List<string[]>();

            foreach (string torrentId in torrentIds)
            {
                bool removed = await RemoveTorrentAsync(torrentId, removeData);
                if (!removed)
                    errors.Add(new[] { torrentId, "Failed to remove torrent"});
            }

            return errors.ToArray();
        }

        public virtual async Task<IDictionary<string, object>> GetTorrentStatusAsync(string torrentId, string[] statusKeys = null, bool diff = false)
        {
            // Create a filter for the torrent id
            var filter = new Filter
            {
                Ids = new HashSet<string>
                {
                    torrentId
                }
            };

            IDictionary<string, IDictionary<string, object>> torrents = await GetTorrentsStatusAsync(filter, statusKeys, diff);

            return torrents?.FirstOrDefault().Value;
        }

        public virtual Task<IDictionary<string, IDictionary<string, object>>> GetTorrentsStatusAsync(Filter filter = null, string[] statusKeys = null, bool diff = false)
        {
            throw new NotImplementedException();
        }

        public virtual void Close()
        {
        }

        public void Dispose()
        {
            if (disposed)
                return;

            Dispose(true);
            GC.SuppressFinalize(this);

            disposed = true;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            Close();
        }

        protected void CheckDisposed()
        {
            if (disposed)
                throw new ObjectDisposedException(GetType().Name);
        }

        ~Deluge()
        {
            Dispose(false);
        }
    }
}