#define MyOneDrive //  ISO
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using AAV.Sys.Helpers;
using LiveCharts.Wpf;
using MVVM.Common;

namespace TypingWpf.VMs
{
  public partial class MainVM : BindableBaseViewModel
  {
    const string _ext = ".json";
#if ISO
    static string _fnm => typeof(MainVM).Name + _ext;
#else
    static string _fnm2 => OneDrive.Folder($@"Public\AppData\TypeCatch\{TypeCatch.Net5.Properties.Settings.Default.LastUser}{_ext}");
    public static string MainVmJsonFile => OneDrive.Folder($@"Public\AppData\TypeCatch\{typeof(MainVM).Name}{_ext}");
#endif
    public static MainVM Create(CartesianChart cChart1)
    {
      MainVM vm;

      // I think this is old / not relevant any more:
      //if (!A0DbMdl.GetCreateA0DbMdl().Database.Exists()) // takes 4[-8] sec [migration check]
      //{
      //    if (File.Exists(App.Dbfn)) { MessageBox.Show($"File exists\r\n\n{App.Dbfn}\r\n\n...but appears to have no DB data.\n\nExiting", "RARE but...", MessageBoxButton.OK, MessageBoxImage.Stop); return null; }

      //    vm = JsonFileSerializer.Load<MainVM>(MainVmJsonFile) as MainVM;
      //    if (vm == null)
      //        vm = new MainVM { Audible = true, SelectUser = "Old" };
      //    else
      //        vm.onJsonToDb_Suspended();
      //}
      //else
      vm = new MainVM
      {
        _chartUC = cChart1
      };

      var ax = vm._chartUC.AxisX.FirstOrDefault();
      if (ax != null)
        ax.LabelFormatter = value => DateTime.FromOADate(value).ToString("MMM-dd"); //vm.XFormatter_ProperWay = value => DateTime.FromOADate(value).ToString("MMM-dd HH"); //tu: dates with formats

      return vm;
    }


    static void deleteOldBackUps_NOTUSED_DEMO_ONLY()
    {
      return;
#if DEBUG
      var keepCnt = 64;
#else
            int keepCnt = 32;
#endif

#if ISO
            var dir = IsoHelper.GetIsoFolder();
#else
      var dir = Path.GetDirectoryName(MainVmJsonFile);
#endif
      var all = new DirectoryInfo(dir).GetFiles($@"{typeof(MainVM).Name}.{DateTime.Now:yy.}*{_ext}");
      var ttl = all.Count();
      if (ttl <= keepCnt)
        return;

      var del = all.OrderBy(r => r.LastWriteTime).Take(ttl - keepCnt);

      Debug.WriteLine($@">>Deleting: {dir}    {typeof(MainVM).Name}.{DateTime.Now:yy.}*{_ext}     { del.Count()} / {all.Count()}");

      foreach (var f in del) { Debug.WriteLine($">>Deleting: {f}"); f.Delete(); }
    }

    CartesianChart _chartUC;
  }
}
