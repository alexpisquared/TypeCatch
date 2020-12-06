using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
///  https://msdn.microsoft.com/en-us/magazine/jj991977.aspx
/// </summary>
namespace TypingWpf.Misc
{
    public static class DeadlockDemo
    {
        static async Task DelayAsync() => await Task.Delay(1000);

        public static void Causes_A_Deadlock() => DelayAsync().Wait(); // This method causes a deadlock when called in a GUI or ASP.NET context.

        public static async void NoDeadlock() => await DelayAsync(); // This method is fine.

    }
}
