using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using The_Post.Models;
using The_Post.Models.API;
using The_Post.Models.VM;

namespace The_Post.Services
{
    public interface IArticleService
    {
        public void CreateArticle(Article article);
        public void UpdateArticle(Article updatedArticle);
        public Article GetArticleById(int articleID);
        public List<Article> GetAllArticles(); // Pagination
        public List<Article> GetFiveMostPopularArticles();
        public List<Article> GetEditorsChoiceArticles();
        public List<SelectListItem> GetAllCategoriesSelectList();
        public List<Category> GetSelectedCategories(List<int> categoryIDs);
        public List<Article> GetSearchResults(string searchTerm);
        public string GetProcessedArticleContent(string content);
        public string GetUnprocessedArticleContent(string content);
        public List<Article> TenLatestArticles();
        public Article GetMostPopularArticleByCategory(int categoryID);
        public List<Article> GetAllArticlesByCategoryID(int categoryID);
        public List<Article> GetAllArticlesByCategoryName(string categoryName);
        public void DeleteArticle(int articleID);
        public Task AddRemoveLikeAsync(int articleID, string userID);
        public Task<int> GetLikeCountAsync(int articleID);
        public Task<string> UploadFileToContainer(AddArticleVM model);

        public bool IsCookiesAccepted();

        public void AcceptCookies();

        public Task<SpotNow> GetData();
    }
}
