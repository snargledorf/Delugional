using System;
using System.Diagnostics;
using System.Linq;
using System.Management;

namespace Delugional.Utility
{
    public static class ProcessHelper
    {
        public static bool IsExecutableRunning(string path)
        {
            return GetProcessForExecutable(path) != null;
        }

        public static Process GetProcessForExecutable(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException(@"Argument is null or whitespace", nameof(path));

            return WindowsManagementProcessUtils.GetProcessForPath(path);
        }

        private static class WindowsManagementProcessUtils
        {
            public static Process GetProcessForPath(string path)
            {
                const string wmiQueryString = "SELECT ProcessId, ExecutablePath, CommandLine FROM Win32_Process";
                using (var searcher = new ManagementObjectSearcher(wmiQueryString))
                {
                    using (ManagementObjectCollection results = searcher.Get())
                    {
                        return results.Cast<ManagementObject>()
                            .Where(mo => string.Equals(mo["ExecutablePath"], path))
                            .Join(Process.GetProcesses(), mo => (int)(uint)mo["ProcessId"], p => p.Id, (mo, p) => p)
                            .FirstOrDefault();
                    }
                }
            }
        }
    }
}