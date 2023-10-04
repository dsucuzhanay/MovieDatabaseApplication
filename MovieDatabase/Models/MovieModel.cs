using System.ComponentModel.DataAnnotations;

namespace MovieDatabase.Models
{
    public class MovieModel
    {
        public string? Title { get; set; }

        [DisplayFormat(DataFormatString = "{0:0.0#}", ApplyFormatInEditMode = true)]
        public decimal Rating { get; set; }

        public string? Genres { get; set; }

        [Display(Name = "IMDB")]
        public string ImdbId { get; set; } = null!;

        [Display(Name = "TMDB")]
        public string TmdbId { get; set; } = null!;
    }
}
