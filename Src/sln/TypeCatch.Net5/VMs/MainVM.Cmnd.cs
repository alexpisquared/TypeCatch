using System.Windows.Input;
using TypeCatch.Net5.AsLink;
using TypingWpf.Vws;

namespace TypingWpf.VMs;

public partial class MainVM : BindableBaseViewModel
{
  ICommand _F1; public ICommand F1Cmd => _F1 ??= new RelayCommand(x => new DbExplorer2().ShowDialog() /*updateUserLessonLst(DashName, SelectUser)*/, x => true, Key.F1);
  ICommand _F2; public ICommand F2Cmd => _F2 ??= new RelayCommand(onF2, x => true, Key.F2);
  ICommand _F3; public ICommand F3Cmd => _F3 ??= new RelayCommand(onF3, x => IsInSsn, Key.F3);
  ICommand _F4; public ICommand F4Cmd => _F4 ??= new RelayCommand(onF4, x => IsInSsn, Key.F4);
  ICommand _F5; public ICommand F5Cmd => _F5 ??= new RelayCommand(onF5, x => !IsInSsn, Key.F5);
  ICommand _F6; public ICommand F6Cmd => _F6 ??= new RelayCommand(onF6, Key.F6);
  ICommand _F7; public ICommand F7Cmd => _F7 ??= new RelayCommand(onF7, Key.F7);
  ICommand _F8; public ICommand F8Cmd => _F8 ??= new RelayCommand(onF8, Key.F8);
  ICommand _F9; public ICommand F9Cmd => _F9 ??= new RelayCommand(onF9, x => true, Key.F9);
  ICommand _FA; public ICommand FACmd => _FA ??= new RelayCommand(onFA, x => true, Key.F10);
  ICommand _FB; public ICommand FBCmd => _FB ??= new RelayCommand(onFB, x => true, Key.F11);
  ICommand _FC; public ICommand FCCmd => _FC ??= new RelayCommand(onFC, x => true, Key.F12);
  ICommand _DeleteSR; public ICommand DeleteSR => _DeleteSR ??= new RelayCommand(onDeleteSR, x => SelectSnRt != null, Key.None);
  ICommand _ShowChart; public ICommand ShowChart => _ShowChart ??= new RelayCommand(x => onShowChart(x.ToString() ?? ""));
  ICommand _LT; public ICommand LTCmd => _LT ??= new RelayCommand(x => prepLessonType((string)x), x => true, Key.F11);
  ICommand _JsonToDb; public ICommand JsonToDb => _JsonToDb ??= new RelayCommand(x => onJsonToDb_Suspended(), x => !IsBusy);
}