using Microsoft.AspNetCore.Mvc;
using MovieDatabase.Data;
using MovieDatabase.Models;
using MovieDatabase.Views.Shared.Components.SearchBar;

namespace MovieDatabase.Controllers
{
    public class MovieController : Controller
    {
        private readonly MovieLensContext _context;
        private readonly IConfiguration _config;

        public MovieController(MovieLensContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public IActionResult Index(string SearchText = "", int pg = 1)
        {
            SearchText ??= "";
            var movies = new List<Movie>();

            if (SearchText != "")
            {
                movies = _context.Movies
                    .Where(m => m.Title.Contains(SearchText))
                    .ToList();
            }
            else
            {
                movies = _context.Movies.ToList();
            }

            if (pg < 1)
                pg = 1;

            var pageSize = _config.GetValue<int>("MovieParameters:MoviePageSize");
            var totalItems = movies.Count;
            var skip = (pg - 1) * pageSize;

            movies = movies
                .OrderBy(m => m.Title)
                .Skip(skip)
                .Take(pageSize)
                .ToList();

            var movieModels = GetMovieModels(movies);

            SearchPager searchPager = new("Index", "Movie", SearchText, totalItems, pg, pageSize);
            ViewBag.SearchPager = searchPager;

            return View(movieModels);
        }

        private List<MovieModel> GetMovieModels(List<Movie> movies)
        {
            var moviesModel = new List<MovieModel>();

            foreach (var movie in movies)
            {
                var rating = GetRating(movie.MovieId);
                var genres = GetGenres(movie.MovieId);
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
                    Rating = rating,
                    Genres = genres,
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

        private string GetGenres(int movieId)
        {
            var moviesGenres = _context.MoviesGenres
                .Where(m => m.MovieId == movieId)
                .ToList();

            var genres = string.Empty;
            var removeLastChar = false;

            foreach (var movieGenre in moviesGenres)
            {
                var genre = _context.Genres.Where(g => g.GenreId == movieGenre.GenreId).First();
                genres += genre.Name + " - ";
                removeLastChar = true;
            }

            if (removeLastChar)
                genres = genres.Remove(genres.Length - 3);

            return genres;
        }
    }
}
