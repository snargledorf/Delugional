using System;
using Delugional;
using Delugional.Daemon;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DelugionalTests
{
    [TestClass]
    public class DelugeDaemonTests
    {
        [TestMethod]
        public void Default()
        {
            IDelugeDaemon daemon = DelugeDaemon.Default;

            Assert.IsNotNull(daemon);
        }

        [TestMethod]
        public void StartStopRunning()
        {
            bool running = DelugeDaemon.Default.Running;

            Assert.IsFalse(running);

            DelugeDaemon.Default.Start();

            running = DelugeDaemon.Default.Running;

            Assert.IsTrue(running);

            DelugeDaemon.Default.Stop();

            running = DelugeDaemon.Default.Running;

            Assert.IsFalse(running);
        }
    }
}
