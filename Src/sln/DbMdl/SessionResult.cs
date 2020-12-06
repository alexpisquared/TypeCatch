namespace TypingWpf.DbMdl
{
  using System;
  using System.ComponentModel.DataAnnotations;
  using System.ComponentModel.DataAnnotations.Schema;

  [Table("TCh.SessionResult")]
  public partial class SessionResult
  {
    public int Id { get; set; }

    [Required] [StringLength(16)] public string UserId { get; set; }

    [Required] [StringLength(50)] public string ExcerciseName { get; set; }

    public int PokedIn { get; set; }

    public bool? IsRecord { get; set; } = null;
    public int? TotalRunCount { get; set; } = null;

    public DateTime DoneAt { get; set; }

    public TimeSpan Duration { get; set; }

    [Column(TypeName = "text")] public string Note { get; set; }

    public virtual User User { get; set; }
  }
}
