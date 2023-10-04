using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieDatabase.Data;

public partial class Link
{
    [Key]
    public int MovieId { get; set; }

    [StringLength(10)]
    public string ImdbId { get; set; } = null!;

    [StringLength(10)]
    public string? TmdbId { get; set; }

    [ForeignKey("MovieId")]
    [InverseProperty("Link")]
    public virtual Movie Movie { get; set; } = null!;
}
