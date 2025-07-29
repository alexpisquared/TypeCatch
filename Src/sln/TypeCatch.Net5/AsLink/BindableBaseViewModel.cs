using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Input;
namespace TypeCatch.Net5.AsLink;
public abstract class BindableBaseViewModel : ObservableObject
{
  static readonly TraceSwitch appTraceLevel_Lcl = new("Display nanme", "Descr-n") { Level = TraceLevel.Error };
  protected BindableBaseViewModel() => CloseAppCmd = new RelayCommand(x => OnRequestClose(), param => CanClose())
  {
    //#if __DEBUG
    //      GestureKey = Key.Escape
    //#else
    //    GestureKey = Key.F4,
    //    GestureModifier = ModifierKeys.Alt
    //#endif
  };

  protected virtual void AutoExec() => Trace.WriteLineIf(appTraceLevel_Lcl.TraceVerbose, "AutoExec()");
  protected virtual void AutoExecSynch() => Trace.WriteLineIf(appTraceLevel_Lcl.TraceVerbose, "AutoExecSynch()");
  protected virtual async Task AutoExecAsync() { Trace.WriteLineIf(appTraceLevel_Lcl.TraceVerbose, "AutoExecAsync()"); await Task.Delay(1); }
  public ICommand CloseAppCmd { get; }

  public event EventHandler RequestClose;
  protected virtual void OnRequestClose() => RequestClose?.Invoke(this, EventArgs.Empty);
  protected virtual bool CanClose() => true;
  protected virtual async Task ClosingVM() => await Task.Delay(0);  // public abstract void Closing();

  protected static bool _cancelClosing = false;
  public static void CloseEvent(Window view, BindableBaseViewModel vwMdl)
  {
    async void handler(object? sender, EventArgs e)
    {
      await vwMdl.ClosingVM();
      if (_cancelClosing) return;

      vwMdl.RequestClose -= handler;
      try
      {
        if (view != null)
          if (System.Windows.Application.Current.Dispatcher.CheckAccess()) // if on UI thread							
            view.Close();
          else
            await System.Windows.Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(view.Close));
      }
      catch (Exception ex) { Trace.WriteLine(ex.Message, MethodBase.GetCurrentMethod()?.Name); if (Debugger.IsAttached) Debugger.Break(); }
    } // When the ViewModel asks to be closed, close the window.

    vwMdl.RequestClose += handler;

    ////org: replaced by code cleanup on Jan 2019.
    //EventHandler handler = null; // When the ViewModel asks to be closed, close the window.
    //handler = async delegate
    //{
    //  await vwMdl.ClosingVM();
    //  if (_cancelClosing) return;

    //  vwMdl.RequestClose -= handler;
    //  try
    //  {
    //    if (view != null)
    //    {
    //      if (Application.Current.Dispatcher.CheckAccess()) // if on UI thread							
    //        view.Close();
    //      else
    //        await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => view.Close()));
    //    }
    //  }
    //  catch (Exception ex) { Trace.WriteLine(ex.Message, System.Reflection.MethodInfo.GetCurrentMethod().Name); if (Debugger.IsAttached) Debugger.Break(); }
    //};
    //vwMdl.RequestClose += handler;

  }

  public static void ShowMvvm(BindableBaseViewModel vMdl, Window view)
  {
    view.DataContext = vMdl;
    CloseEvent(view, vMdl);
    _ = Task.Run(vMdl.AutoExec); //			await vMdl.AutoExec();
    view.Show();
  }
  public static bool? ShowModalMvvm(BindableBaseViewModel vMdl, Window view, Window? owner = null)
  {
    if (owner != null)
    {
      view.Owner = owner;
      view.WindowStartupLocation = WindowStartupLocation.CenterOwner;
    }

    view.DataContext = vMdl;
    CloseEvent(view, vMdl);

    autoexecSafe(vMdl);

    return view.ShowDialog();
  }

  public static async Task<bool?> ShowModalMvvmAsync(BindableBaseViewModel vMdl, Window view, Window? owner = null) // public static async Task<bool?> ShowModalMvvmAsync(BindableBaseViewModel vMdl, Window view, Window owner = null)
  {
    if (owner != null)
    {
      view.Owner = owner;
      view.WindowStartupLocation = WindowStartupLocation.CenterOwner;
    }

    view.DataContext = vMdl;
    CloseEvent(view, vMdl);

#if brave
    Task.Run(async () => await autoexecAsyncSafe(vMdl)); //todo: 
#else
    await vMdl.AutoExecAsync(); //todo:             
#endif

    return view.ShowDialog();
  }

  static void autoexecSafe(BindableBaseViewModel vMdl)
  {
    try
    {
      if (System.Windows.Application.Current.Dispatcher.CheckAccess()) // if on UI thread
        vMdl.AutoExec();
      else
        _ = System.Windows.Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(vMdl.AutoExec));
    }
    catch (Exception ex) { Trace.WriteLine(ex.Message, MethodBase.GetCurrentMethod().Name); if (Debugger.IsAttached) Debugger.Break(); }
  }
  protected static async Task refreshUi() => await System.Windows.Application.Current.Dispatcher.BeginInvoke(new ThreadStart(refreshUiSynch));
  protected static void refreshUiSynch() => CommandManager.InvalidateRequerySuggested();  //tu: Sticky UI state fix for MVVM (May2015)
}