using System.Threading.Tasks;
using Delugional.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DelugionalTests
{
    [TestClass]
    public class WhenListTests
    {
        [TestMethod]
        public async Task GetWhen()
        {
            var collection = new WhenList<int>();

            Task.Run(() =>
            {
                for (int i = 0; i < 20; i++)
                    collection.Add(i);
            });

            int result = await collection.GetWhen(i => i == 9);

            Assert.AreEqual(9, result);
        }
    }
}
