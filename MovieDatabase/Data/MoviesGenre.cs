using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieDatabase.Data;

[Index("GenreId", Name = "IX_MoviesGenres_GenreId")]
public partial class MoviesGenre
{
    [Key]
    public int Id { get; set; }

    public int MovieId { get; set; }

    public int GenreId { get; set; }

    [ForeignKey("GenreId")]
    [InverseProperty("MoviesGenres")]
    public virtual Genre Genre { get; set; } = null!;

    [ForeignKey("MovieId")]
    [InverseProperty("MoviesGenres")]
    public virtual Movie Movie { get; set; } = null!;
}
