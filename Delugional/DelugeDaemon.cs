using System;
using System.IO;

namespace Delugional
{
    public class DelugeDaemon : IDelugeDaemon
    {
        public static IDelugeDaemon Default { get; } = new DelugeDaemon();

        public DelugeDaemon(string pathToDaemonExe = null)
        {
            if (pathToDaemonExe == null)
                pathToDaemonExe = FindDefaultDaemon();
        }

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }

        public bool Running { get; }

        public void Start()
        {
            throw new System.NotImplementedException();
        }

        public void Stop()
        {
            throw new System.NotImplementedException();
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
