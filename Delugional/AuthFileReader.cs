using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Delugional
{
    public class AuthFileReader : IAuthFileReader, IEnumerable<User>
    {
        private TextReader reader;

        private int lineNumber;

        private bool disposed;

        public AuthFileReader(string path)
            : this(File.OpenRead(path))
        {
        }

        public AuthFileReader(Stream stream)
            : this(new StreamReader(stream))
        {
        }

        public AuthFileReader(TextReader reader)
        {
            this.reader = reader;
        }

        public User Read()
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                lineNumber++;

                if (string.IsNullOrEmpty(line))
                    continue;

                string[] parts = line.Split(':');
                if (parts.Length != 3)
                    throw new InvalidDataException("Invalid user on line " + lineNumber);

                return new User(parts[0], parts[1], (AuthLevels)Enum.Parse(typeof(AuthLevels), parts[2]));
            }

            return null;
        }

        public User[] ReadAll()
        {
            var users = new List<User>();

            User user;
            while ((user = Read()) != null)
            {
                users.Add(user);
            }

            return users.ToArray();
        }

        public IEnumerator<User> GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public class Enumerator : IEnumerator<User>
        {
            private AuthFileReader reader;

            public Enumerator(AuthFileReader reader)
            {
                this.reader = reader;
            }

            public void Dispose()
            {
                reader?.Dispose();
                reader = null;
            }

            public bool MoveNext()
            {
                return (Current = reader.Read()) != null;
            }

            public void Reset()
            {
                throw new NotImplementedException();
            }

            public User Current { get; private set; }

            object IEnumerator.Current => Current;
        }

        public void Dispose()
        {
            if (disposed)
                return;

            Dispose(true);
            GC.SuppressFinalize(this);

            disposed = true;
        }

        private void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            reader?.Close();
            reader?.Dispose();
            reader = null;
        }
    }
}