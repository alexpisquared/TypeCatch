using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypingWpf.Resources.Simplified
{

    public class SsnResultsOnly
    {
        public Snrt[] SnRts { get; set; }
    }

    public class Snrt
    {
        public Doneat DoneAt { get; set; }
        public string Duration { get; set; }
        public string ExcerciseName { get; set; }
        public string Notes { get; set; }
        public int PokedIn { get; set; }
        public string UserId { get; set; }
    }

    public class Doneat
    {
        public DateTime DateTime { get; set; }
        public int OffsetMinutes { get; set; }
    }
}
