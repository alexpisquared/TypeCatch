using AAV.Sys.Ext;
using AsLink;
using System;
using System.Diagnostics;
using System.Linq;
using TypeCatch.Net5.DbMdl;
using TypingWpf.DbMdl;

namespace TypingWpf.DbDataTran
{
  public class DataTransfer
  {
    readonly DateTime _now = DateTime.Now;
    public (int newRows, string report, Stopwatch swstopwatch) CopyChunkyAzureSynch(A0DbMdl src, A0DbMdl trg)
    {
      var rowsAdded = 0;
      var sw = Stopwatch.StartNew();

      var srcSvrDb = src.ServerDatabase();
      var trgSvrDb = trg.ServerDatabase();

      foreach (var u in src.Users)
      {
        if (!trg.Users.Any(r => u.UserId.Equals(r.UserId, StringComparison.OrdinalIgnoreCase)))
        {
          trg.Users.Add(new User { UserId = u.UserId, FullName = u.UserId, CreatedAt = _now, Note = $"data transfer from  '{srcSvrDb}'." });
          rowsAdded++;
        }
      }

      //trg.SessionResults.Load(); // chunky vs chattee

      foreach (var s in src.SessionResults)
      {
        if (trg.SessionResults.Any(r => s.DoneAt == r.DoneAt && s.UserId.Equals(r.UserId, StringComparison.OrdinalIgnoreCase)))
          Trace.WriteLineIf(ExnLogr.AppTraceLevelCfg.TraceVerbose, $"  **> skipping match:  {s.DoneAt}   {s.UserId} ");
        else
        {
          trg.SessionResults.Add(new SessionResult
          {
            Duration = s.Duration,
            ExcerciseName = s.ExcerciseName,
            IsRecord = s.IsRecord,
            PokedIn = s.PokedIn,
            TotalRunCount = s.TotalRunCount,
            UserId = s.UserId,
            DoneAt = s.DoneAt,
            Note = s.Note
          });
          rowsAdded++;
        }
      }

      var rowsSaved = trg.SaveChanges();

      Debug.Assert(rowsAdded == rowsSaved, "ap: rowsChanged != rowsSaved");

      return (rowsSaved, $"Success: {rowsAdded} rows copied from \n {srcSvrDb} to \n {trgSvrDb} in {sw.Elapsed:mm\\:ss}.", sw);
    }
  }
}
