using X.PagedList;

namespace The_Post.Models.VM
{
    public class AdminAllArticlesVM
    {
        public IPagedList<Article> Articles { get; set; } // For pagination
        public string? SortOrder { get; set; }
        public bool IncludeArchived { get; set; }
    }
}
