using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using The_Post.Services;

namespace The_Post.Areas.Identity.Pages.Account.Manage
{
    public class SubscriptionEmailSentModel : BaseCookiesPageModel
    {
        public SubscriptionEmailSentModel(IArticleService articleService)
            : base(articleService)
        {
            
        }
        public void OnGet()
        {
        }
    }
}
