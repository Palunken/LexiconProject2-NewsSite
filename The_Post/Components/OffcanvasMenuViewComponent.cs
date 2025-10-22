using Microsoft.AspNetCore.Mvc;

namespace The_Post.Components
{
    public class OffcanvasMenuViewComponent :ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View();
        }
    }
}
