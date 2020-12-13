using MVVM.Common;
using System.Windows.Input;
using TypingWpf.Vws;

namespace TypingWpf.VMs
{
    public partial class MainVM : BindableBaseViewModel
    {
        ICommand _F1; public ICommand F1Cmd => _F1 ?? (_F1 = new RelayCommand(x => new DbExplorer2().ShowDialog() /*updateUserLessonLst(DashName, SelectUser)*/, x => true, Key.F1));
        ICommand _F2; public ICommand F2Cmd => _F2 ?? (_F2 = new RelayCommand(x => onF2(x), x => true, Key.F2));
        ICommand _F3; public ICommand F3Cmd => _F3 ?? (_F3 = new RelayCommand(x => onF3(x), x => IsInSsn, Key.F3));
        ICommand _F4; public ICommand F4Cmd => _F4 ?? (_F4 = new RelayCommand(x => onF4(x), x => IsInSsn, Key.F4));
        ICommand _F5; public ICommand F5Cmd => _F5 ?? (_F5 = new RelayCommand(x => onF5(x), x => !IsInSsn, Key.F5));
        ICommand _F6; public ICommand F6Cmd => _F6 ?? (_F6 = new RelayCommand(x => onF6(x), Key.F6));
        ICommand _F7; public ICommand F7Cmd => _F7 ?? (_F7 = new RelayCommand(x => onF7(x), Key.F7));
        ICommand _F8; public ICommand F8Cmd => _F8 ?? (_F8 = new RelayCommand(x => onF8(x), Key.F8));
        ICommand _F9; public ICommand F9Cmd => _F9 ?? (_F9 = new RelayCommand(x => onF9(x), x => true, Key.F9));
        ICommand _FA; public ICommand FACmd => _FA ?? (_FA = new RelayCommand(x => onFA(x), x => true, Key.F10));
        ICommand _FB; public ICommand FBCmd => _FB ?? (_FB = new RelayCommand(x => onFB(x), x => true, Key.F11));
        ICommand _FC; public ICommand FCCmd => _FC ?? (_FC = new RelayCommand(x => onFC(x), x => true, Key.F12));
        ICommand _DeleteSR; public ICommand DeleteSR => _DeleteSR ?? (_DeleteSR = new RelayCommand(x => onDeleteSR(x), x => SelectSnRt != null, Key.None));
        ICommand _ShowChart; public ICommand ShowChart => _ShowChart ?? (_ShowChart = new RelayCommand(x => onShowChart(x)));

        ICommand _LT; public ICommand LTCmd => _LT ?? (_LT = new RelayCommand(x => prepLessonType((string)x), x => true, Key.F11));
        ICommand _JsonToDb; public ICommand JsonToDb => _JsonToDb ?? (_JsonToDb = new RelayCommand(x => onJsonToDb_Suspended(), x => !IsBusy));

    }
}