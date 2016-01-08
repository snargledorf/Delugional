using System;
using System.IO;

namespace Delugional
{
    public class AuthFileWriter : IAuthFileWriter, IDisposable
    {
        private bool disposed;
        private TextWriter writer;

        public AuthFileWriter(string path)
            : this(File.OpenWrite(path))
        {
        }

        public AuthFileWriter(Stream stream)
            : this(new StreamWriter(stream))
        {
        }

        private AuthFileWriter(TextWriter writer)
        {
            this.writer = writer;
        }

        private void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            writer?.Close();
            writer?.Dispose();
            writer = null;
        }

        public virtual void Write(AuthFile authFile)
        {
            foreach (User user in authFile)
            {
                Write(user);
            }
        }

        public virtual void Write(User user)
        {
            Write(user.Username, user.Password, user.AuthLevel);
        }

        public virtual void Write(string username, string password, AuthLevels authLevel)
        {
            writer.WriteLine(string.Join(":", username, password, (int)authLevel));
        }

        public void Dispose()
        {
            if (disposed)
                return;

            Dispose(true);
            GC.SuppressFinalize(this);

            disposed = true;
        }

        ~AuthFileWriter()
        {
            Dispose(false);
        }
    }
}