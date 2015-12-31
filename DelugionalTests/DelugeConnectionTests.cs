using System;
using System.Net;
using System.Threading.Tasks;
using Delugional;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DelugionalTests
{
    [TestClass]
    public class DelugeConnectionTests
    {
        [TestMethod]
        public async Task Connect()
        {
            using (var connection = new DelugeConnection())
            {
                await connection.Open();
            }
        }

        [TestMethod]
        public async Task GetMethodList()
        {
            using (var connection = new DelugeConnection())
            {
                await connection.Open();

                RpcCall rpcCall = new RpcCall.Builder("daemon.get_method_list").Build();

                object[] result = await connection.Call(rpcCall);
            }
        }

        [TestMethod]
        public async Task Login()
        {
            using (var connection = new DelugeConnection())
            {
                await connection.Open();

                RpcCall rpcCall = new RpcCall.Builder("daemon.login")
                    .AddArg("localclient")
                    .AddArg("8a9bc4597feb7e2b8669104aeb462fc204d31901")
                    .Build();

                object[] result = await connection.Call(rpcCall);
            }
        }
    }
}
