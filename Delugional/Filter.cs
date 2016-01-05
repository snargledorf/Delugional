using System.Collections.Generic;

namespace Delugional
{
    public sealed class Filter
    {
        public HashSet<string> Ids { get; set; } = new HashSet<string>();
        public HashSet<States> States { get; set; } = new HashSet<States>();
        public HashSet<string> Keywords { get; set; } = new HashSet<string>();
        public HashSet<string> TrackerHosts { get; set; } = new HashSet<string>();
    }
}