using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieDatabase.Data;

[PrimaryKey("UserId", "MovieId")]
[Index("MovieId", Name = "IX_Ratings_MovieId")]
public partial class Rating
{
    [Key]
    public int UserId { get; set; }

    [Key]
    public int MovieId { get; set; }

    [Column(TypeName = "decimal(2, 1)")]
    public decimal Ratng { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime Date { get; set; }

    [ForeignKey("MovieId")]
    [InverseProperty("Ratings")]
    public virtual Movie Movie { get; set; } = null!;
}
