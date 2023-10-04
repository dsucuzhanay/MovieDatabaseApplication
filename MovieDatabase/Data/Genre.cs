using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieDatabase.Data;

public partial class Genre
{
    [Key]
    public int GenreId { get; set; }

    [StringLength(20)]
    public string Name { get; set; } = null!;

    [InverseProperty("Genre")]
    public virtual ICollection<MoviesGenre> MoviesGenres { get; } = new List<MoviesGenre>();
}
