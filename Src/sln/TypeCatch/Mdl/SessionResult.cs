using System;

namespace TypingWpf.Mdl
{
    public class SessionResult
	{
		public string UserId { get; set; }
		public string Note { get; set; }
		public string ExcerciseName { get; set; }
		public int PokedIn { get; set; }
		public DateTimeOffset DoneAt { get; set; }
		public TimeSpan Duration { get; set; }
		public int CpM { get { return (int)(Duration.TotalMinutes == 0 ? 0d : PokedIn / Duration.TotalMinutes); } }
	}
}
