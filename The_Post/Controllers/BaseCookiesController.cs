using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using The_Post.Services;

namespace The_Post.Controllers
{
    public class BaseCookiesController : Controller
    {
        private readonly IArticleService _articleService;

        public BaseCookiesController(IArticleService articleService)
        {
            _articleService = articleService;
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            // Set IsCookiesAccepted for all actions in controllers that inherit from this controller
            // This way, the view can display the cookies notice if the user has not accepted cookies and hide it if they have
            ViewData["IsCookiesAccepted"] = _articleService.IsCookiesAccepted();
            base.OnActionExecuted(context);
        }

        // This action is called when the user accepts cookies from the cookies notice
        [HttpPost]
        public IActionResult AcceptCookies()
        {
            // Set the cookie to mark that the user has accepted cookies
            _articleService.AcceptCookies();

            return NoContent();
        }
    }
}
