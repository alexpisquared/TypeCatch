namespace TypingWpf.DbMdl
{
    public partial class SessionResult
    {
        public int CpM { get { return (int)(Duration.TotalMinutes == 0 ? 0d : PokedIn / Duration.TotalMinutes); } }
    }

    public partial class User
    {
        public override string ToString()
        {
            return FullName ?? UserId;
        }
    }
}
