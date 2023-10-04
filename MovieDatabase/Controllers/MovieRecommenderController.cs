using Microsoft.AspNetCore.Mvc;
using MovieDatabase.Data;
using MovieDatabase.Models;

namespace MovieDatabase.Controllers
{
    public class MovieRecommenderController : Controller
    {
        private readonly MovieLensContext _context;
        private readonly IConfiguration _config;
        private List<decimal> _bestRatings;

        public MovieRecommenderController(MovieLensContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
            _bestRatings = new List<decimal> { 5, 4.5m, 4 };
        }

        public IActionResult Index(string SearchText = "")
        {
            var movieModels = new List<MovieModel>();

            if (SearchText == string.Empty)
                return View(movieModels);

            var userId = Convert.ToInt32(SearchText);

            #region users with same taste

            var moviesRatedByUserId = _context.Ratings
                .Where(r => r.UserId == userId)
                .Select(r => new { r.MovieId, r.Ratng })
                .ToList();

            var usersDictionary = new Dictionary<int, int>();

            foreach (var movieRatedByUserId in moviesRatedByUserId)
            {
                var usersSameTaste = _context.Ratings
                    .Where(r => r.MovieId == movieRatedByUserId.MovieId && r.Ratng == movieRatedByUserId.Ratng && r.UserId != userId)
                    .Select(r => new { r.UserId })
                    .ToList();

                foreach (var item in usersSameTaste)
                {
                    if (usersDictionary.ContainsKey(item.UserId))
                        usersDictionary[item.UserId] += 1;
                    else
                        usersDictionary.Add(item.UserId, 1);
                }
            }

            usersDictionary = usersDictionary
                .Where(pair => pair.Value > _config.GetValue<int>("MovieParameters:NumberMoviesRatedSimilarly"))
                .OrderByDescending(pair => pair.Value)
                .Take(_config.GetValue<int>("MovieParameters:NumberSimilarUsers"))
                .ToDictionary(pair => pair.Key, pair => pair.Value);

            #endregion

            #region movies to recommend

            var numberMoviesRecommend = _config.GetValue<int>("MovieParameters:NumberMoviesRecommend");

            var moviesUser = new List<int>();
            foreach (var movieRatedByUserId in moviesRatedByUserId)
                moviesUser.Add(movieRatedByUserId.MovieId);

            var moviesDictionary = new Dictionary<int, int>();

            foreach (var rating in _bestRatings)
            {
                foreach (KeyValuePair<int, int> entry in usersDictionary)
                {
                    var moviesOtherUser = _context.Ratings
                        .Where(r => r.UserId == entry.Key && r.Ratng == rating)
                        .Select(r => new { r.MovieId })
                        .ToList();

                    foreach (var movieOtherUser in moviesOtherUser)
                    {
                        if (!moviesUser.Contains(movieOtherUser.MovieId))
                        {
                            if (moviesDictionary.ContainsKey(movieOtherUser.MovieId))
                                moviesDictionary[movieOtherUser.MovieId] += 1;
                            else
                                moviesDictionary.Add(movieOtherUser.MovieId, 1);
                        }
                    }
                }

                if (moviesDictionary.Count >= numberMoviesRecommend)
                    break;
            }

            var topMoviesDictionary = moviesDictionary
                .OrderByDescending(pair => pair.Value)
                .Take(numberMoviesRecommend)
                .ToDictionary(pair => pair.Key, pair => pair.Value);

            var movies = new List<int>();
            foreach (var movie in topMoviesDictionary)
                movies.Add(movie.Key);

            #endregion

            if (!movies.Any())
                return RedirectToAction("Index", "MovieByGenre");

            movieModels = GetMovieModels(movies);

            @ViewBag.SearchText = SearchText;

            return View(movieModels);
        }

        private List<MovieModel> GetMovieModels(List<int> movies)
        {
            var moviesModel = new List<MovieModel>();

            foreach (var movie in movies)
            {
                var rating = GetRating(movie);
                var genres = GetGenres(movie);
                var link = _context.Links.Where(l => l.MovieId == movie).FirstOrDefault();

                var imdbId = string.Empty;
                var tmdbId = string.Empty;
                if (link != null)
                {
                    if (link.ImdbId != null)
                        imdbId = link.ImdbId;
                    if (link.TmdbId != null)
                        tmdbId = link.TmdbId;
                }

                var movieAux = _context.Movies.Where(m => m.MovieId == movie).First();

                var movieModel = new MovieModel
                {
                    Title = movieAux.Title,
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

        //public IActionResult Index(string SearchText = "")
        //{
        //    var movieModels = new List<MovieModel>();

        //    if (SearchText == string.Empty)
        //        return View(movieModels);

        //    var userId = Convert.ToInt32(SearchText);

        //    #region users with same taste

        //    var moviesRatedByUserId = _context.Ratings
        //        .Where(r => r.UserId == userId)
        //        .Select(r => new { r.MovieId, r.Ratng })
        //        .ToList();

        //    var usersDictionary = new Dictionary<int, int>();

        //    foreach (var movieRatedByUserId in moviesRatedByUserId)
        //    {
        //        var usersSameTaste = _context.Ratings
        //            .Where(r => r.MovieId == movieRatedByUserId.MovieId && r.Ratng == movieRatedByUserId.Ratng && r.UserId != userId)
        //            .Select(r => new { r.UserId })
        //            .ToList();

        //        foreach (var item in usersSameTaste)
        //        {
        //            if (usersDictionary.ContainsKey(item.UserId))
        //                usersDictionary[item.UserId] += 1;
        //            else
        //                usersDictionary.Add(item.UserId, 1);
        //        }
        //    }

        //    usersDictionary = usersDictionary
        //        .Where(pair => pair.Value > MoviesRatedSimilarly)
        //        .OrderByDescending(pair => pair.Value)
        //        .ToDictionary(pair => pair.Key, pair => pair.Value);

        //    #endregion

        //    #region movies to recommend

        //    var moviesUser = new List<int>();
        //    foreach (var movieRatedByUserId in moviesRatedByUserId)
        //        moviesUser.Add(movieRatedByUserId.MovieId);

        //    var moviesToRecommend = new List<int>();

        //    foreach (var rating in _bestRatings)
        //    {
        //        foreach (KeyValuePair<int, int> entry in usersDictionary)
        //        {
        //            var moviesOtherUser = _context.Ratings
        //                .Where(r => r.UserId == entry.Key && r.Ratng == rating)
        //                .Select(r => new { r.MovieId })
        //                .ToList();

        //            foreach (var movieOtherUser in moviesOtherUser)
        //            {
        //                if (!moviesUser.Contains(movieOtherUser.MovieId))
        //                {
        //                    moviesToRecommend.Add(movieOtherUser.MovieId);

        //                    if (moviesToRecommend.Count >= NumberMovies)
        //                        break;
        //                }
        //            }

        //            if (moviesToRecommend.Count >= NumberMovies)
        //                break;
        //        }

        //        if (moviesToRecommend.Count >= NumberMovies)
        //            break;
        //    }

        //    #endregion

        //    movieModels = GetMovieModels(moviesToRecommend);

        //    @ViewBag.SearchText = SearchText;

        //    return View(movieModels);
        //}

    }
}
