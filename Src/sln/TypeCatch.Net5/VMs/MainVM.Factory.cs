namespace TypingWpf.VMs;

public partial class MainVM //: BindableBaseViewModel
{
  CartesianChart _chartUC;

  public static MainVM Create(CartesianChart cChart1)
  {
    var vm = new MainVM { _chartUC = cChart1 };

    Axis? ax = vm._chartUC.AxisX.FirstOrDefault();
    if (ax != null)
      ax.LabelFormatter = value => DateTime.FromOADate(value).ToString("MMM-dd HH"); //vm.XFormatter_ProperWay = value => DateTime.FromOADate(value).ToString("MMM-dd HH"); //tu: dates with formats

    return vm;
  }
}
