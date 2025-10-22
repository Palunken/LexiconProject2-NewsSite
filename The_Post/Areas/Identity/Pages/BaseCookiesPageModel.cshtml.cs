using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using The_Post.Services;

namespace The_Post.Areas.Identity.Pages
{
    public class BaseCookiesPageModel : PageModel
    {
        private readonly IArticleService _articleService;

        public BaseCookiesPageModel(IArticleService articleService)
        {
            _articleService = articleService;
        }

        public override void OnPageHandlerExecuted(PageHandlerExecutedContext context)
        {
            // Before rendering the page, set IsCookiesAccepted for all models that inherit from this model
            // This way, the page can display the cookies notice if the user has not accepted cookies and hide it if they have
            ViewData["IsCookiesAccepted"] = _articleService.IsCookiesAccepted();
            base.OnPageHandlerExecuted(context);
        }
    }
}
