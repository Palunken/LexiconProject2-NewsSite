using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using The_Post.Services;

namespace The_Post.Areas.Identity.Pages.Account.Manage
{
    public class SubscriptionSuccessModel : BaseCookiesPageModel
    {
        public SubscriptionSuccessModel(IArticleService articleService)
            : base(articleService)
        {
            
        }
        public DateTime ExpireDate { get; set; }

        public void OnGet(string expireDate)
        {
            if (DateTime.TryParse(expireDate, out var expire))
            {
                ExpireDate = expire;
            }
        }
    }
}
