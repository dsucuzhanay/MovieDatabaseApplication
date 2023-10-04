using Microsoft.AspNetCore.Mvc;
using MovieDatabase.Data;
using MovieDatabase.Models;

namespace MovieDatabase.Controllers
{
    public class UserController : Controller
    {
        private readonly MovieLensContext _context;
        private readonly IConfiguration _config;

        public UserController(MovieLensContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public IActionResult Index(string SearchText = "")
        {
            var userModels = new List<UserModel>();

            if (SearchText == string.Empty)
                return View(userModels);

            var userId = Convert.ToInt32(SearchText);

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

            var numberSimilarUsers = _config.GetValue<int>("MovieParameters:NumberSimilarUsers");

            var topDictionary = usersDictionary
                .Where(pair => pair.Value > _config.GetValue<int>("MovieParameters:NumberMoviesRatedSimilarly"))
                .OrderByDescending(pair => pair.Value)
                .Take(numberSimilarUsers)
                .ToDictionary(pair => pair.Key, pair => pair.Value);

            foreach (KeyValuePair<int, int> entry in topDictionary)
                userModels.Add(new UserModel { UserId = entry.Key });

            @ViewBag.SearchText = SearchText;
            @ViewBag.NumberSimilarUsers = numberSimilarUsers;

            return View(userModels);
        }
    }
}
