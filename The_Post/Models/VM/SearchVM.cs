using X.PagedList;

namespace The_Post.Models.VM
{
    public class SearchVM
    {
        public IPagedList<Article> Articles { get; set; } // For pagination
        public string SearchTerm { get; set; }
    }
}
