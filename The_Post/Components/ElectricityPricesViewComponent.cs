using Microsoft.AspNetCore.Mvc;
using The_Post.Services;

namespace The_Post.Components
{
    [ViewComponent]
    public class ElectricityPrices : ViewComponent
    {
        private readonly IArticleService _articleService;

        public ElectricityPrices(IArticleService articleService)
        {
            _articleService = articleService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var spotpricesnow = await _articleService.GetData();
            return View(spotpricesnow);
        }
    }
}
