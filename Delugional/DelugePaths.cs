using System;
using System.IO;

namespace Delugional
{
    public static class DelugePaths
    {
        public static string DaemonExecutableName { get; } = "deluged.exe";

        public static string ProgramFilesFolderName { get; } = "Deluge";

        public static string AppDataFolderName { get; } = "deluge";

        public static string DefaultAuthFileName { get; } = "auth";

        public static string DefaultAuthFile { get; } = Path.Combine(AppDataFolder, DefaultAuthFileName);

        public static string AppDataFolder => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppDataFolderName);

        public static string DaemonProgramFilesX86 { get; } = Path.Combine(CreateProgramFilesFolderPath(), DaemonExecutableName);
        public static string DaemonProgramFiles { get; } = Path.Combine(CreateProgramFilesFolderPath(false), DaemonExecutableName);

        public static string ProgramFilesX86Folder { get; }  = CreateProgramFilesFolderPath();
        public static string ProgramFilesFolder { get; } = CreateProgramFilesFolderPath(false);

        private static string CreateProgramFilesFolderPath(bool x86 = true)
        {
            Environment.SpecialFolder folder = x86
                ? Environment.SpecialFolder.ProgramFilesX86
                : Environment.SpecialFolder.ProgramFiles;

            string programFiles = Environment.GetFolderPath(folder);
            return Path.Combine(programFiles, ProgramFilesFolderName);
        }
    }
}