using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Delugional.Rpc;
using Delugional.Utility;

namespace Delugional.Daemon
{
    public class DelugeDaemon : IDelugeDaemon
    {
        public static IDelugeDaemon Default { get; } = new DelugeDaemon();

        private readonly string pathToDaemon;

        private bool disposed;

        public DelugeDaemon(string pathToDaemon = null)
        {
            if (pathToDaemon == null)
                pathToDaemon = FindDefaultDaemon();

            if (string.IsNullOrEmpty(pathToDaemon) || !File.Exists(pathToDaemon))
                throw new FileNotFoundException("Deluge daemon executable not found", pathToDaemon);

            this.pathToDaemon = pathToDaemon;

            Process = ProcessHelper.GetProcessForExecutable(pathToDaemon);
        }

        public Process Process { get; private set; }

        public bool Running => ProcessHelper.IsExecutableRunning(pathToDaemon);

        public bool Start()
        {
            if (Running)
            {
                Process = ProcessHelper.GetProcessForExecutable(pathToDaemon);
                return true;
            }

            var process = new Process { StartInfo = { FileName = pathToDaemon } };
            if (!process.Start())
                return false;

            Process = process;
            return true;
        }

        public void Stop()
        {
            if (!Running)
                return;

            if (Process == null)
                Process = ProcessHelper.GetProcessForExecutable(pathToDaemon);

            Process.Kill();
            Process.WaitForExit();
        }

        public IDelugeRpc OpenRpc()
        {
            return OpenRpcAsync().Result;
        }

        public async Task<IDelugeRpc> OpenRpcAsync()
        {
            var connection = new DelugeRpcConnectionV3();
            await connection.OpenAsync();
            return new DelugeRpc(connection);
        }

        private static string FindDefaultDaemon()
        {
            return GetProgramFilesDaemon();
        }

        private static string GetProgramFilesDaemon()
        {
            string daemonPath = DelugePaths.DaemonProgramFilesX86;

            if (File.Exists(daemonPath))
                return daemonPath;

            daemonPath = DelugePaths.DaemonProgramFiles;

            return File.Exists(daemonPath) ? daemonPath : null;
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

            Process?.Close();
            Process = null;
        }

        ~DelugeDaemon()
        {
            Dispose(false);
        }
    }
}
