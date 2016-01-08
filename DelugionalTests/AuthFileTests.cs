using Delugional;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DelugionalTests
{
    [TestClass]
    public class AuthFileTests
    {
        [TestMethod]
        public void OpenDefault()
        {
            AuthFile authFile = AuthFile.OpenDefault();
            Assert.IsNotNull(authFile);
            Assert.IsTrue(authFile.Count != 0);
        }

        [TestMethod]
        public void AddUser1()
        {
            var authFile = new AuthFile {{"testuser", "password", AuthLevels.Admin}};

            Assert.AreEqual(1, authFile.Count);
        }

        [TestMethod]
        public void AddUser2()
        {
            var authFile = new AuthFile {new User("testuser", "password", AuthLevels.Admin)};

            Assert.AreEqual(1, authFile.Count);
        }

        [TestMethod]
        public void RemoveUserByUsername()
        {
            var authFile = new AuthFile { { "testuser", "password", AuthLevels.Admin } };

            authFile.Remove("testuser");

            Assert.AreEqual(0, authFile.Count);
        }

        [TestMethod]
        public void RemoveUser()
        {
            var user = new User("testuser", "password", AuthLevels.Admin);
            var authFile = new AuthFile { user };

            authFile.Remove(user);

            Assert.AreEqual(0, authFile.Count);
        }
    }
}
