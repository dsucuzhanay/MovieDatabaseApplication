using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieDatabase.Data;

public partial class Movie
{
    [Key]
    public int MovieId { get; set; }

    [StringLength(200)]
    public string Title { get; set; } = null!;

    [InverseProperty("Movie")]
    public virtual Link? Link { get; set; }

    [InverseProperty("Movie")]
    public virtual ICollection<MoviesGenre> MoviesGenres { get; } = new List<MoviesGenre>();

    [InverseProperty("Movie")]
    public virtual ICollection<Rating> Ratings { get; } = new List<Rating>();

    [InverseProperty("Movie")]
    public virtual ICollection<Tag> Tags { get; } = new List<Tag>();
}
