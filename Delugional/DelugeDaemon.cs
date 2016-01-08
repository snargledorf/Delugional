using System;
using System.Diagnostics;
using System.IO;
using Delugional.Rpc;
using Delugional.Utility;

namespace Delugional
{
    public class DelugeDaemon : IDelugeDaemon
    {
        public static IDelugeDaemon Default { get; } = new DelugeDaemon();

        private readonly string pathToDaemon;

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

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }

        public bool Running => ProcessHelper.IsExecutableRunning(pathToDaemon);

        public bool Start()
        {
            if (Running)
            {
                Process = ProcessHelper.GetProcessForExecutable(pathToDaemon);
                return true;
            }

            var process = new Process { StartInfo = { FileName = pathToDaemon } };
            if (process.Start())
            {
                Process = process;
                return true;
            }

            return false;
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

        public IDelugeRpc GetRpc()
        {
            var connection = new DelugeRpcConnectionV3();
            connection.OpenAsync().Wait();
            return new DelugeRpc(connection);
        }

        private string FindDefaultDaemon()
        {
            return GetProgramFilesDaemon();

        }

        private string GetProgramFilesDaemon()
        {
            string daemonPath = CreateProgramFilesFolderPath();

            if (File.Exists(daemonPath))
                return daemonPath;

            daemonPath = CreateProgramFilesFolderPath(true);

            return File.Exists(daemonPath) ? daemonPath : null;
        }

        private string CreateProgramFilesFolderPath(bool x64 = false)
        {
            Environment.SpecialFolder folder = x64
                ? Environment.SpecialFolder.ProgramFiles
                : Environment.SpecialFolder.ProgramFilesX86;

            string programFiles = Environment.GetFolderPath(folder);
            string delugeFolder = Path.Combine(programFiles, DaemonResources.DelugeProgramFilesFolder);
            return Path.Combine(delugeFolder, DaemonResources.DaemonExecutableName);
        }
    }
}
