namespace Delugional
{
    public interface IAuthFileWriter
    {
        void Write(AuthFile authFile);
        void Write(User user);
        void Write(string username, string password, AuthLevels authLevel);
    }
}