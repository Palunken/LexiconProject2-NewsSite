using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using The_Post.Models;
using The_Post.Models.API;
using The_Post.Models.VM;
using The_Post.Services;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using X.PagedList.Extensions;
using System.Drawing.Printing;

namespace The_Post.Controllers
{
    public class ArticleController : BaseCookiesController
    {
        private readonly IArticleService _articleService;
        private readonly IRequestService _requestService;
        private readonly UserManager<User> _userManager;

        private readonly IEmployeeService _employeeService;
        public ArticleController(IArticleService articleService, IRequestService requestService, UserManager<User> userManager, IEmployeeService employeeService)
            : base(articleService)
        {
            _articleService = articleService;
            _requestService = requestService;
            _userManager = userManager;
            _employeeService = employeeService;
        }
               
        public IActionResult Index()
        {           
            return View();
        }

        public IActionResult ViewArticle(int articleID)
        {
            var article = _articleService.GetArticleById(articleID);
            article.Views++;
            _articleService.UpdateArticle(article);

            return View(article);
        }

        [HttpGet]
        [Authorize(Roles ="Writer,Admin")]
        public IActionResult AddArticle()
        {
            var viewModel = new AddArticleVM()
            {
                AvailableCategories = _articleService.GetAllCategoriesSelectList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Writer,Admin")]
        public async Task<IActionResult> AddArticle(AddArticleVM model)
        {
            model.AvailableCategories = _articleService.GetAllCategoriesSelectList();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                if (model.ImageLink == null || model.ImageLink.Length == 0)
                {
                    return Content("File not selected");
                }
                else if (!model.ImageLink.ContentType.StartsWith("image/"))
                {
                    throw new ArgumentException("File is not an image");
                }
                string imageUrl = await _articleService.UploadFileToContainer(model);

                var article = new Article
                {
                    HeadLine = model.HeadLine,
                    LinkText = model.LinkText,
                    ContentSummary = model.ContentSummary,
                    Content = _articleService.GetProcessedArticleContent(model.Content),
                    ImageOriginalLink = imageUrl,
                    Categories = _articleService.GetSelectedCategories(model.SelectedCategoryIds)
                };

                _articleService.CreateArticle(article);

                TempData["ArticleMessage"] = "The article has been added successfully.";
                return RedirectToAction("ArticleAdded");
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("ImageLink", ex.Message);
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        public IActionResult ArticleAdded()
        {
            return View();
        }

        [Authorize(Roles = "Writer,Admin")]
        public IActionResult DeleteArticle(int articleID)
        {
            _articleService.DeleteArticle(articleID);
            return RedirectToAction("AllArticles", "Admin");
        }

        [HttpGet]
        [Authorize(Roles ="Writer,Editor,Admin")]
        public IActionResult EditArticle(int articleID)
        {
            var article = _articleService.GetArticleById(articleID);

            if(article == null)
            {
                return NotFound();
            }

            var vm = new EditArticleVM()
            {
                Id = article.Id,
                HeadLine = article.HeadLine,
                LinkText = article.LinkText,
                ContentSummary = article.ContentSummary,
                Content = _articleService.GetUnprocessedArticleContent(article.Content),
                SelectedCategoryIds = article.Categories.Select(c => c.Id).ToList(),
                AvailableCategories = _articleService.GetAllCategoriesSelectList()
            };

            ViewBag.CurrentImage = article.ImageOriginalLink;
            TempData["ID"] = article.Id;
            return View(vm);
        }

        [HttpPost]
        [Authorize(Roles = "Writer,Editor,Admin")]
        public async Task<IActionResult> EditArticle(EditArticleVM vm)
        {
            if (!ModelState.IsValid)
            {
                vm.AvailableCategories = _articleService.GetAllCategoriesSelectList();               
                return View(vm);
            }
            try
            {
                var article = _articleService.GetArticleById(vm.Id);
                if (article == null)
                {
                    return NotFound("Can't find the article!");
                }

                article.HeadLine = vm.HeadLine;
                article.LinkText = vm.LinkText;
                article.ContentSummary = vm.ContentSummary;
                article.Content = _articleService.GetProcessedArticleContent(vm.Content);

                if (vm.ImageLink != null)
                {
                    if (!vm.ImageLink.ContentType.StartsWith("image/"))
                    {
                        ModelState.AddModelError("ImageLink", "File is not an image");
                        vm.AvailableCategories = _articleService.GetAllCategoriesSelectList();
                        return View(vm);
                    }

                    var imageUrl = await _articleService.UploadFileToContainer(new AddArticleVM { ImageLink = vm.ImageLink });
                    article.ImageOriginalLink = imageUrl;
                }

                article.Categories = _articleService.GetSelectedCategories(vm.SelectedCategoryIds);
                _articleService.UpdateArticle(article);

                TempData["Success"] = "Article updated successfully";
                return RedirectToAction("AllArticles", "Admin");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                vm.AvailableCategories = _articleService.GetAllCategoriesSelectList();
                return View(vm);
            }           
        }
              
        // Also removes a like if the user has already liked the article.
        [HttpPost]
        public async Task<IActionResult> LikeArticle(int articleId)
        {
            // Gets the logged in user
            var loggedInUser = await _userManager.GetUserAsync(User);

            // If the user isn't logged in the return value is -1 (which triggers an error message in the view)
            if (loggedInUser == null)
                return Json(-1);

            // Add or remove like to the datbase, depending on whether or not the user has liked
            // the article before.
            await _articleService.AddRemoveLikeAsync(articleId, loggedInUser.Id);

            // Get the updated like count
            var updatedLikes = await _articleService.GetLikeCountAsync(articleId);

            return Json(updatedLikes);
        }

        public async Task<IActionResult> Weather(CategoryPageVM categoryPageVM)
        {
            List<WeatherForecast> foreCasts = new List<WeatherForecast>();
            var loggedInUser = await _userManager.GetUserAsync(User);

            // If a user is logged in
            if (loggedInUser != null)
            {

                // Gets logged-in user's "weather cities"
                var currentCities = loggedInUser.WeatherCities?.Split(',').Where(city => !string.IsNullOrEmpty(city)).ToList() ?? new List<string>();

                // Adds the user's local city if not already included
                if (!currentCities.Contains(loggedInUser.City))
                {
                    currentCities.Insert(0, loggedInUser.City);
                }

                // If no current cities (or local city) an empty list is returned
                if (!currentCities.Any())
                {
                    return View(new List<WeatherForecast>());
                }

                // Adds forecast to the foreCasts-list, for each city in currentCities
                foreach (string city in currentCities)
                {
                    try
                    {
                        var foreCast = await _requestService.GetForecastAsync(city);
                        if (foreCast != null)
                        {
                            foreCasts.Add(foreCast);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Needs to be changed
                        foreCasts.Add(new WeatherForecast() { City = city, Summary = "Error fetching data" });
                    }
                }
            }

            
            // Gets the articles relating to weather
            var articles = _articleService.GetAllArticlesByCategoryName("Weather");

            // Creates a WeatherVM object that holds the forecasts and articles
            WeatherVM vM = new WeatherVM()
            {
                Forecasts = foreCasts,
                Articles = articles
            };

            return View(vM);
        }


        // Adds a city to the user's string of cities, and returns a partial view with the new weather card 
        [HttpPost]
        public async Task<IActionResult> AddCity(string city)
        {
            if (string.IsNullOrWhiteSpace(city))
            {
                return BadRequest("No such city name.");
            }

            var user = await _userManager.GetUserAsync(User);

            // If the user isn't logged in
            if (user != null)
            {
                // Gets the logged-in user's weather cities and puts them in a list
                var cityList = user.WeatherCities?.Split(',').Select(c => c.Trim()).ToList() ?? new List<string>();

                if (cityList.Contains(city, StringComparer.OrdinalIgnoreCase))
                {
                    return BadRequest("The city is already displayed.");
                }
                // The new city is added
                cityList.Add(city);

                // Updated the user's Cities-string
                user.WeatherCities = string.Join(",", cityList);
                await _userManager.UpdateAsync(user);
            }

            // Gets weather data for the city
            var weatherData = await _requestService.GetForecastAsync(city); 

            return PartialView("_WeatherPartial", weatherData);
        }

        [HttpPost]
        public async Task<IActionResult> AddCityNotLoggedIn(string city)
        {
            if (string.IsNullOrWhiteSpace(city))
            {
                return BadRequest("No such city name.");
            }

            var weatherData = await _requestService.GetForecastAsync(city); // Gets weather data for the city

            return PartialView("_WeatherPartial", weatherData);
        }


        // Removes a city from the logged-in user's Cities-string and returns the new updated list of forecasts
        [HttpPost]
        public async Task<IActionResult> RemoveCity(string city)
        {

            // Removes
            await _requestService.RemoveCity(city);

            // Gets the updated list of forecast-objects
            var weatherData = await _requestService.GetForecastsUserAsync();

            return PartialView("_WeatherListPartial", weatherData);
        }

        public IActionResult SearchResults(string searchTerm, int? page)
        {
            // current page number, default is 1
            int pageNumber = page ?? 1;

            // number of articles per page
            int pageSize = 10; 

            var articles = _articleService.GetSearchResults(searchTerm);
            var onePageOfArticles = articles.ToPagedList(pageNumber, pageSize);

            SearchVM searchVM = new SearchVM()
            {
                Articles = onePageOfArticles,
                SearchTerm = searchTerm
            };

            return View(searchVM);
        }
    }
}
