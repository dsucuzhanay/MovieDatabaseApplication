using Microsoft.AspNetCore.Mvc.Rendering;

namespace MovieDatabase.Models
{
    public class GenreModel
    {
        public int GenreId { get; set; }
        public List<SelectListItem>? GenreList { get; set; }
        public string? Controller { get; set;}
        public string? Action { get; set; }
    }
}
