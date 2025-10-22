namespace The_Post.Models.VM
{
    public class ArticleQueriesVM
    {
        public List<Article> GetFiveMostPopularArticles { get; set; } = new List<Article>();

        public List<Article> TenLatestArticles { get; set; } = new List<Article>();

        public List<Article> GetEditorsChoiceArticles { get; set; } = new List<Article>();
    }
}
