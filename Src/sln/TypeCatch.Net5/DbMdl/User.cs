namespace TypingWpf.DbMdl
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel.DataAnnotations;
  using System.ComponentModel.DataAnnotations.Schema;

  [Table("TCh.User")]

  public partial class User
  {
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public User() => SessionResults = new HashSet<SessionResult>();

    [StringLength(16)] public string UserId { get; set; }

    [StringLength(50)] public string FullName { get; set; }

    [Column(TypeName = "text")] public string Note { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? ModifiedAt { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
    public virtual ICollection<SessionResult> SessionResults { get; set; }
  }
}
