namespace TypingWpf.Mdl
{
    public class VeloMeasure
    {
        public int SppedCpm { get; set; }
        public int PrevRcrd { get; set; }
        public int UpperLim { get { return PrevRcrd * 2; } }
        public double Percentg { get { return 210.0 * SppedCpm / UpperLim; } }
        public bool IsRecord { get { return SppedCpm > PrevRcrd; } }
    }
}
