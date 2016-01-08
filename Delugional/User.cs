namespace Delugional
{
    public sealed class User
    {
        public User(string username, string password, AuthLevels authLevel)
        {
            Username = username;
            Password = password;
            AuthLevel = authLevel;
        }

        public string Username { get; }

        public string Password { get; set; }

        public AuthLevels AuthLevel { get; set; }
    }
}