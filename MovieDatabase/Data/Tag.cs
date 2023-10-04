using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieDatabase.Data;

public partial class Tag
{
    [Key]
    public int Id { get; set; }

    public int UserId { get; set; }

    public int MovieId { get; set; }

    [Column("Tag")]
    [StringLength(300)]
    public string Tag1 { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime Date { get; set; }

    [ForeignKey("MovieId")]
    [InverseProperty("Tags")]
    public virtual Movie Movie { get; set; } = null!;
}
