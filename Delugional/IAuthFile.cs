using System.Collections.Generic;

namespace Delugional
{
    public interface IAuthFile : ICollection<User>
    {
        void Add(string username, string password, AuthLevels authLevel);
        void Remove(string username);
        bool Contains(string username);

        User this[string username] { get; set; }

        void Load(string path);
        void Save(string path);
    }
}