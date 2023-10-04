using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MovieDatabase.Data;
using MovieDatabase.Models;

namespace MovieDatabase.Controllers
{
    public class MovieByGenreController : Controller
    {
        private readonly MovieLensContext _context;
        private readonly IConfiguration _config;

        public MovieByGenreController(MovieLensContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public IActionResult Index(GenreModel genreModel)
        {
            var dateTimeI = DateTime.Now;
            var movies = new List<Movie>();
            var dictionary = new Dictionary<Movie, decimal>();

            if (genreModel.GenreId == 0)
                genreModel.GenreId = _config.GetValue<int>("MovieParameters:DefaultGenre");

            movies = (from m in _context.Movies
                      join mg in _context.MoviesGenres on m.MovieId equals mg.MovieId
                      join g in _context.Genres on mg.GenreId equals g.GenreId
                      where g.GenreId == genreModel.GenreId
                      select m).ToList();

            foreach (var movie in movies)
            {
                var rating = GetRating(movie.MovieId);
                dictionary.Add(movie, rating);
            }

            var dateTimeF = DateTime.Now;
            Console.WriteLine((dateTimeF - dateTimeI).TotalSeconds);

            var top = _config.GetValue<int>("MovieParameters:TopMoviesByGenre");

            var topDictionary = dictionary.OrderByDescending(pair => pair.Value)
                .Take(top)
                .ToDictionary(pair => pair.Key, pair => pair.Value);

            var movieModels = GetMovieModels(topDictionary);

            #region drop down list

            var genres = _context.Genres.ToList();
            genreModel.GenreList = new List<SelectListItem>();

            foreach (var genre in genres)
            {
                genreModel.GenreList.Add(
                    new SelectListItem
                    {
                        Text = genre.Name,
                        Value = Convert.ToString(genre.GenreId)
                    });
            }

            genreModel.Controller = "MovieByGenre";
            genreModel.Action = "Index";
            ViewBag.GenreModel = genreModel;
            ViewBag.Top = top;

            #endregion

            return View(movieModels);
        }

        private List<MovieModel> GetMovieModels(Dictionary<Movie, decimal> dictionary)
        {
            var moviesModel = new List<MovieModel>();

            foreach (KeyValuePair<Movie, decimal> entry in dictionary)
            {
                var movie = entry.Key;
                var link = _context.Links.Where(l => l.MovieId == movie.MovieId).FirstOrDefault();

                var imdbId = string.Empty;
                var tmdbId = string.Empty;
                if (link != null)
                {
                    if (link.ImdbId != null)
                        imdbId = link.ImdbId;
                    if (link.TmdbId != null)
                        tmdbId = link.TmdbId;
                }

                var movieModel = new MovieModel
                {
                    Title = movie.Title,
                    Rating = entry.Value,
                    ImdbId = imdbId,
                    TmdbId = tmdbId
                };

                moviesModel.Add(movieModel);
            }

            return moviesModel;
        }

        private decimal GetRating(int movieId)
        {
            var ratings = _context.Ratings
                    .Where(r => r.MovieId == movieId)
                    .Select(r => new { r.Ratng })
                    .ToList();

            var rating = decimal.Zero;

            if (ratings.Any())
            {
                var sumRating = ratings.Sum(r => r.Ratng);
                var countRating = ratings.Count;
                rating = Math.Round(sumRating / countRating, 1);
            }

            return rating;
        }
    }
}
