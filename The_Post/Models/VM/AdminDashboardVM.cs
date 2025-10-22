namespace The_Post.Models.VM
{
    public class AdminDashboardVM
    {
        public int TotalArticles { get; set; }
        public int ArchivedArticles { get; set; }
        public string? MostLikedArticle { get; set; }
        public string? MostLikedImage { get; set; }
        public int MostLikedArticleLikes { get; set; }
        public int TotalEmployees { get; set; }
        public int TotalAdmin { get; set; }
        public int TotalEditors { get; set; }
        public int TotalWriters { get; set; }
        public int TotalUsers { get; set; }
        public int TotalSubscribers { get; set; }
        public int ActiveSubscriptions { get; set; }
        public int ExpiredSubscriptions { get; set; }
        public int TotalViews { get; set; }
        public List<int> UserAges { get; set; } = new List<int>();
    }
}
