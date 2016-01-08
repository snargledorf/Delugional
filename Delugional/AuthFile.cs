using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Delugional
{
    public sealed class AuthFile : IAuthFile
    {
        private readonly IDictionary<string, User> users;

        public AuthFile()
            : this(new Dictionary<string, User>())
        {
        }

        public AuthFile(string path)
            : this(new AuthFileReader(path))
        {
        }

        public AuthFile(IEnumerable<User> users)
            : this(users.ToDictionary(u => u.Username, u => u))
        {
        }

        private AuthFile(IDictionary<string, User> users)
        {
            this.users = users;
        }

        public User this[string username]
        {
            get { return users[username]; }
            set { users[username] = value; }
        }

        public int Count => users.Count;

        public bool IsReadOnly { get; } = false;

        public static AuthFile OpenDefault()
        {
            return Open(DelugePaths.DefaultAuthFile);
        }

        public static AuthFile Open(string path)
        {
            return new AuthFile(path);
        }

        public void Add(string username, string password, AuthLevels authLevel)
        {
            Add(new User(username, password, authLevel));
        }

        public bool Contains(string username)
        {
            return users.ContainsKey(username);
        }

        public void Remove(string username)
        {
            users.Remove(username);
        }

        public void Load(string path)
        {
            users.Clear();
            using (var reader = new AuthFileReader(path))
                AddRange(reader);
        }

        public void AddRange(IEnumerable<User> users)
        {
            foreach (User user in users)
            {
                Add(user);
            }
        }

        public void Save(string path)
        {
            using (var writer = new AuthFileWriter(path))
            {
                writer.Write(this);
            }
        }

        public void Add(User item)
        {
            users.Add(item.Username, item);
        }

        public void Clear()
        {
            users.Clear();
        }

        public bool Contains(User item)
        {
            return users.ContainsKey(item.Username);
        }

        public void CopyTo(User[] array, int arrayIndex)
        {
            users.Values.CopyTo(array, arrayIndex);
        }

        public bool Remove(User item)
        {
            return users.Remove(item.Username);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<User> GetEnumerator()
        {
            return users.Values.GetEnumerator();
        }
    }
}
