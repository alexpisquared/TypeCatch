namespace TypingWpf.DbMdl
{
  using System;
  using System.ComponentModel.DataAnnotations;
  using System.ComponentModel.DataAnnotations.Schema;

  [Table("TCh.AppStng")]
  public partial class AppStng
  {
    public int Id { get; set; }

    [StringLength(50)] public string FullName { get; set; }

    [Column(TypeName = "text")] public string Note { get; set; }

    public LessonType LesnTyp { get; set; }

    [StringLength(8)] public string SubLesnId { get; set; }
    [StringLength(16)] public string UserId { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public bool ProLTgl { get; set; }
    public bool Audible { get; set; }
  }
}
