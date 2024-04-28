namespace TypeCatch.Net5.Mdl;

public class VeloMeasure
{
  public int SppedCpm { get; set; }
  public int PrevRcrd { get; set; }
  public int UpperLim => PrevRcrd * 2;
  public double Percentg => Math.Abs(210.0 * SppedCpm / UpperLim);
  public bool IsRecord => SppedCpm > PrevRcrd;
}
