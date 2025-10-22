using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using The_Post.Data;
using The_Post.Models;
using The_Post.Models.VM;
using The_Post.Services;
using The_Post.Middleware;
using Microsoft.AspNetCore.Authorization;
using X.PagedList.Extensions;
using Microsoft.IdentityModel.Tokens;

namespace The_Post.Controllers
{
    
    public class AdminController : Controller
    {        
        private readonly IArticleService _articleService;
        private readonly IEmployeeService _employeeService;
        private readonly IRoleService _roleService;
        private readonly ISubscriptionService _subscriptionService;
        private readonly ApplicationDbContext _db;
        private const int MaxEditorsChoice = 5;

        public AdminController(IArticleService articleService, IEmployeeService employeeService, IRoleService roleService, ISubscriptionService subscriptionService,ApplicationDbContext db)
        {            
            _articleService = articleService;
            _employeeService = employeeService;
            _roleService = roleService;
            _subscriptionService = subscriptionService;
            _db = db;
        }

        public async Task<IActionResult> Index(AdminDashboardVM adminDashboard)
        {
            var totalArticles = _db.Articles.Count();
            var archived = _db.Articles.Where(a => a.IsArchived).Count();
            var allArticles = _articleService.GetAllArticles();
            var mostLiked = allArticles.OrderByDescending(a => a.Likes.Count).FirstOrDefault();
            var totalViews = allArticles.Sum(a => a.Views);

            string mostLikedArticle = mostLiked?.HeadLine ?? "No articles available";
            var mostLikedImage = mostLiked?.ImageSmallLink ?? "No image available";
            int mostLikedCount = mostLiked?.Likes.Count ?? 0;

            var allEmployees = await _employeeService.GetAllEmployeesWithRolesAsync();
            var totalEmployees = allEmployees.Count();
            var totalAdmins = allEmployees.Where(e => e.Role.Equals("Admin")).Count();
            var totalEditor = allEmployees.Where(e => e.Role.Equals("Editor")).Count();
            var totalWriter = allEmployees.Where(e => e.Role.Equals("Writer")).Count();

            var totalUsers = _db.Users.Where(u => u.IsEmployee == false).Count();
            var stats = await _subscriptionService.GetSubscriptionStats();

            var userAges = _db.Users
            .Where(u => !u.IsEmployee && u.DOB.HasValue)            
            .Select(u => DateTime.Now.Year - u.DOB.Value.Year -
                (DateTime.Now.DayOfYear < u.DOB.Value.DayOfYear ? 1 : 0))
            .ToList();


            var viewModel = new AdminDashboardVM
            {
                TotalArticles = totalArticles,
                ArchivedArticles = archived,
                MostLikedArticle = mostLikedArticle,
                MostLikedImage = mostLikedImage,
                MostLikedArticleLikes = mostLikedCount,
                TotalViews = totalViews,
                TotalEmployees = totalEmployees,
                TotalAdmin = totalAdmins,
                TotalEditors = totalEditor,
                TotalWriters = totalWriter,
                TotalUsers = totalUsers,
                TotalSubscribers = stats.TotalSubscribers,
                ActiveSubscriptions = stats.ActiveSubscriptions,
                ExpiredSubscriptions = stats.ExpiredSubscriptions,
                UserAges = userAges

            };

            return View(viewModel);
        }

        //------------------------- EMPLOYEE ACTIONS -------------------------

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EmployeeManagement()
        {
            var employeeList = await _employeeService.GetAllEmployeesWithRolesAsync();

            var viewModel = new EmployeeVM
            {
                Employees = employeeList,
                NewEmployee = new AddEmployeeVM()
            };

            return View(viewModel);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> EditEmployee(string userId)
        {
            var employee = await _employeeService.GetEmployeeById(userId);
            if (employee == null) return NotFound();

            return View(employee);
        }

        [HttpPost]
        public async Task<IActionResult> EditEmployee(EditEmployeeVM model)
        {
            if (!ModelState.IsValid) return View(model);

            var success = await _employeeService.EditEmployee(model);
            if (!success)
            {
                TempData["ErrorMessage"] = "Failed to update employee details.";
                return View(model);
            }

            TempData["SuccessMessage"] = "Employee details updated successfully.";
            return RedirectToAction("EmployeeManagement");
        }


        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult AddEmployee()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddEmployee([Bind(Prefix = "NewEmployee")] AddEmployeeVM model)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {                    
                    UserName = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    DOB = model.DOB,
                    Address = model.Address,
                    Zip = model.Zip,
                    City = model.City,
                    PhoneNumber = model.PhoneNumber,
                    Email = model.Email,
                    IsEmployee = true
                };

                var result = await _employeeService.AddEmployee(user, model.Password);

                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Employee successfully added.";
                    return RedirectToAction("EmployeeManagement");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                } 
            }

            // Set a flag so the view knows to stay on the "Register Employee" tab.
            ViewBag.ActiveTab = "addEmployee";

            // Repopulate the employee list and return the view with the errors.
            var employeeList = await _employeeService.GetAllEmployeesWithRolesAsync();
            var vM = new EmployeeVM
            {
                Employees = employeeList,
                NewEmployee = model
            };

            return View("EmployeeManagement", vM);
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteEmployee(string userId)
        {
            var result = await _employeeService.DeleteEmployee(userId);

            if (!result) 
            {
                TempData["ErrorMessage"] = "Failed to delete employee.";
                return RedirectToAction("EmployeeManagement"); 
            }

            TempData["SuccessMessage"] = "Employee deleted successfully.";
            return RedirectToAction("EmployeeManagement");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AllEmployees()
        {
            var employees = await _employeeService.GetAllEmployeesWithRolesAsync();
            return View(employees);
        }
        

        //------------------------- ARTICLE ACTIONS -------------------------

        [Authorize(Roles = "Editor,Admin")]
        public IActionResult AllArticles(int? page, string sortOrder, bool? includeArchived)
        {
            // Current page number
            int pageNumber = page ?? 1;
            // Number of articles per page
            int pageSize = 12;

            var articles = _articleService.GetAllArticles();

            if (articles.IsNullOrEmpty())
            {
                articles = new List<Article>();
            }

            // Excludes archived articles if the user has chosen to omit them
            if (includeArchived != true)
            {
                articles = articles.Where(a => !a.IsArchived).ToList();
            }

            // Sorts the articles based on the sortOrder parameter
            // If no sortOrder is provided, the articles are returned in their default order (newest first)
            articles = sortOrder switch
            {
                "title_asc" => articles.OrderBy(a => a.HeadLine).ToList(),
                "title_desc" => articles.OrderByDescending(a => a.HeadLine).ToList(),
                "date_asc" => articles.OrderBy(a => a.DateStamp).ToList(),
                "date_desc" => articles.OrderByDescending(a => a.DateStamp).ToList(),
                _ => articles
            };

            var onePageOfArticles = articles.ToPagedList(pageNumber, pageSize);

            var viewModel = new AdminAllArticlesVM()
            {
                Articles = onePageOfArticles,
                IncludeArchived = includeArchived ?? false,
                SortOrder = sortOrder
            };

            return View(viewModel);
        }

        [Authorize(Roles = "Editor,Admin")]
        public async Task<IActionResult> UpdateEditorsChoice(int articleId, bool isEditorsChoice)
        {
            try
            {
                var article = await _db.Articles.FindAsync(articleId);
                if (article == null)
                {
                    return NotFound("Article not found");
                }

                if (isEditorsChoice)
                {
                    var currentCount = await _db.Articles
                        .Where(a => a.EditorsChoice)
                        .CountAsync();

                    if(currentCount >= MaxEditorsChoice)
                    {
                        return Json(new
                        {
                            success = false,
                            message = $"Maximum number of Editor's Choices ({MaxEditorsChoice}) has been reached."
                        });
                    }
                }

                article.EditorsChoice = isEditorsChoice;
                await _db.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch(Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An unexpected error occurred" });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Editor,Admin")]
        public async Task<IActionResult> ArchiveArticle([FromBody] ArchiveArticleRequest request)
        {
            try
            {
                var article = await _db.Articles.FindAsync(request.ArticleId);
                if (article == null)
                {
                    return NotFound(new { success = false, message = "Article not found" });
                }
                article.IsArchived = request.IsArchived;
                await _db.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An unexpected error occurred" });
            }
        }


        [Authorize(Roles = "Admin,Editor")]
        // Gets the search results in a paginated list
        public IActionResult SearchResultsAdmin(string searchTerm, int? page)
        {
            // Sets the page number to 1 if the page-parameter is null
            int pageNumber = page ?? 1;
            int pageSize = 12;

            var articles = _articleService.GetSearchResults(searchTerm);
            var onePageOfArticles = articles.ToPagedList(pageNumber, pageSize);
            
            SearchVM searchVM = new SearchVM
            {
                Articles = onePageOfArticles,
                SearchTerm = searchTerm
            };

            return View(searchVM);
        }       


        //------------------------- SUBSCRIPTION ACTIONS -------------------------

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SubscriptionStats()
        {
            var stats = await _subscriptionService.GetSubscriptionStats();
            return View(stats); 
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SubscriptionStatsOverTime()
        {
            var stats = await _subscriptionService.GetSubscriptionStatsOverTime();
            var jsonData = stats.Select(s => new
            {
                Month = s.Month.ToString("yyyy-MM"), // Format for the X-axis
                TotalSubscribers = s.TotalSubscribers,
                ActiveSubscriptions = s.ActiveSubscriptions,
                ExpiredSubscriptions = s.ExpiredSubscriptions
            }).ToList();

            return Json(jsonData);
        }
    }
}


/*
basel @example.com
B_e123456

john.editor@email.com
John@1234

emily.admin@email.com
Emily@5678

ahmed.writer@email.com
Ahmed@9876
*/