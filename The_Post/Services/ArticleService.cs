using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using The_Post.Data;
using The_Post.Models;
using Azure.Storage.Blobs;
using The_Post.Models.VM;
using System.ClientModel.Primitives;
using System.Text.RegularExpressions;
using The_Post.Models.API;
using Newtonsoft.Json;

namespace The_Post.Services
{
    public class ArticleService : IArticleService
    {
        private readonly IHttpContextAccessor _IHttpContextAccessor;
        private readonly ApplicationDbContext _applicationDBContext;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpclient;

        public ArticleService(IHttpContextAccessor iHttpContextAccessor,ApplicationDbContext applicationDbContext, IConfiguration configuration)
        {
            _IHttpContextAccessor = iHttpContextAccessor;
            _applicationDBContext = applicationDbContext;
            _configuration = configuration;
            _httpclient = new HttpClient();
        }
        public void CreateArticle(Article article)
        {
            _applicationDBContext.Articles.Add(article);
            _applicationDBContext.SaveChanges();
        }

        public void DeleteArticle(int articleID)
        {
            var article = _applicationDBContext.Articles.FirstOrDefault(a => a.Id == articleID);
            _applicationDBContext.Articles.Remove(article);
            _applicationDBContext.SaveChanges();
        }

        public void UpdateArticle(Article updatedArticle)
        { 
            _applicationDBContext.Articles.Update(updatedArticle);
            _applicationDBContext.SaveChanges();
        }
                
        public async Task<string> UploadFileToContainer(AddArticleVM model)
        {
            string connectionString = _configuration["AzureBlobStorage:ConnectionString"];
            string containerName = _configuration["AzureBlobStorage:ContainerName"];

            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            // Create the container if it does not exist

            await containerClient.CreateIfNotExistsAsync();

            string uniqueFileName = $"{Guid.NewGuid()}_{model.ImageLink.FileName}";
            BlobClient blobClient = containerClient.GetBlobClient(uniqueFileName);

            using (var stream = model.ImageLink.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, true);
            }

            return blobClient.Uri.ToString();
        }

        public List<Article> GetAllArticles()
        {
            // Order by date, newest first
            var articles = _applicationDBContext.Articles
                .Include(a => a.Categories)
                .Include(a => a.Likes)
                .OrderByDescending(a => a.DateStamp)
                .ToList();

            return articles;
        }

        public Article GetArticleById(int articleID)
        {
            var article = _applicationDBContext.Articles                
                .Include(a => a.Categories)
                .Include(a => a.Likes)
                .FirstOrDefault(c => c.Id == articleID);
            return article;
        }

        public List<Article> GetEditorsChoiceArticles()
        {
            var editorschoice = _applicationDBContext.Articles
                .Include(a => a.Categories)
                .Where(a => a.IsArchived == false)
                .Where(article => article.EditorsChoice).ToList();
            return editorschoice;
        }

        public List<Article> TenLatestArticles()
        {
            var tenlatest = _applicationDBContext.Articles
                .Where(a => a.IsArchived == false)
                .Include(a => a.Categories)
                .OrderByDescending(article => article.DateStamp).Take(10).ToList();
                
            return tenlatest;
        }

        public List<Article> GetFiveMostPopularArticles()
        {
            var mostPopular = _applicationDBContext.Articles
                .Where(a => a.IsArchived == false)
                .Include(a => a.Categories)
                .OrderByDescending(m => m.Views).Take(5).ToList();
            return mostPopular;
        }

        public Article GetMostPopularArticleByCategory(int categoryID)
        {
            var mostpopularbycategory = _applicationDBContext.Categories.Where(c => c.Id == categoryID)
                                          .SelectMany(c => c.Articles)
                                          .Include(a => a.Categories)
                                          .Where(a => a.IsArchived == false)
                                          .OrderByDescending(m => m.Views).FirstOrDefault();

            return (Article)mostpopularbycategory;
        }

        public List<Article> GetAllArticlesByCategoryID(int categoryID)
        {
            var articles = _applicationDBContext.Categories.Where(c => c.Id == categoryID)
                            .SelectMany(c => c.Articles)
                            .Include(a => a.Categories)
                            .Where(a => a.IsArchived == false).ToList();
            return articles;

        }

        public List<Article> GetAllArticlesByCategoryName(string categoryName)
        {
            var articles = _applicationDBContext.Categories.Where(c => c.Name == categoryName)
                            .SelectMany(c => c.Articles)
                            .Include(a => a.Categories)
                            .Where(a => a.IsArchived == false)
                            .OrderByDescending(a => a.DateStamp).ToList();
            return articles;

        }

        // For use when displaying the categories as checkboxes.
        public List<SelectListItem> GetAllCategoriesSelectList()
        {
            List<SelectListItem> categories = _applicationDBContext.Categories
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList();

            return categories;
        }

        public List<Category> GetSelectedCategories(List<int> categoryIDs)
        {
            List<Category> categories = _applicationDBContext.Categories
                                        .Where(c => categoryIDs.Contains(c.Id)).ToList();

            return categories;
        }

        public List<Article> GetSearchResults(string searchTerm)
        {
            // Splits into multiple strings if the search term contains multiple words (divided by an empty space)
           var words = searchTerm.Split(" ", StringSplitOptions.RemoveEmptyEntries);

            // Regular expression used to match only the exact word
            var wordRegex = new Func<string, Regex>((word) =>
                new Regex(@"\b" + Regex.Escape(word) + @"\b", RegexOptions.IgnoreCase));

            // Regular expression used to match the exact word + "s", for plural forms of words
            var pluralRegex = new Func<string, Regex>((word) =>
                new Regex(@"\b" + Regex.Escape(word + "s") + @"\b", RegexOptions.IgnoreCase));

            // Returns the top 20 articles that match the word(s)
            // The matches are given different weights/priority (3, 2 or 1) based on the element (Headline, LinkText etc.)
            var results = _applicationDBContext.Articles
                .Include(a => a.Categories)
                .Where(a => a.IsArchived == false)
                .ToList() // ToList() is needed otherwise the Regex won't work
                .Where(a => words.All(word =>
                    wordRegex(word).IsMatch(a.HeadLine) ||
                    wordRegex(word).IsMatch(a.LinkText) ||
                    wordRegex(word).IsMatch(a.ContentSummary) ||
                    wordRegex(word).IsMatch(a.Content) ||
                    pluralRegex(word).IsMatch(a.HeadLine) ||
                    pluralRegex(word).IsMatch(a.LinkText) ||
                    pluralRegex(word).IsMatch(a.ContentSummary) ||
                    pluralRegex(word).IsMatch(a.Content))) 
                .OrderByDescending(a => words.Any(word =>
                    wordRegex(word).IsMatch(a.HeadLine) || pluralRegex(word).IsMatch(a.HeadLine)) ? 3 : 0)
                .ThenByDescending(a => words.Any(word =>
                    wordRegex(word).IsMatch(a.LinkText) || pluralRegex(word).IsMatch(a.LinkText)) ? 2 : 0)  
                .ThenByDescending(a => words.Any(word =>
                    wordRegex(word).IsMatch(a.ContentSummary) || pluralRegex(word).IsMatch(a.ContentSummary)) ? 2 : 0)  
                .ThenByDescending(a => words.Any(word =>
                    wordRegex(word).IsMatch(a.Content) || pluralRegex(word).IsMatch(a.Content)) ? 1 : 0) 
                .ThenByDescending(a => a.DateStamp)  // Fallback-sorting by date (most recent first) if two articles have the same weight
                .ToList();


            return results;
        }

        public string GetProcessedArticleContent(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return string.Empty;

            // Splits the text into a list of paragraph-strings and wraps them in <p> tags based on if a double newline is found.
            // Replaces single newlines in a paragraph with a linebreak tag.
            var paragraphs = content.Split(new[] { "\n\n" }, StringSplitOptions.None)
                                  .Select(p => $"<p>{p.Replace("\n", "<br />")}</p>");

            // Joins the paragraphs together into one string.
            return string.Join("", paragraphs);
        }

        public string GetUnprocessedArticleContent(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return string.Empty;

            // Replace <br /> with \n
            content = content.Replace("<br />", "\n");

            // Remove <p> and </p>, replacing with double newlines
            content = Regex.Replace(content, @"<\/p>\s*<p>", "\n\n");
            content = content.Replace("<p>", "").Replace("</p>", "");

            return content;
        }


        // Checks if there is a like made by the user for the article. If there is, the like gets removed. If not, a new like is added.
        public async Task AddRemoveLikeAsync(int articleID, string userID)
        {
            var existingLike = await _applicationDBContext.Likes
                            .FirstOrDefaultAsync(l => l.ArticleId == articleID && l.UserId == userID);

            if (existingLike == null)
            {
                var newLike = new Like
                {
                    ArticleId = articleID,
                    UserId = userID
                };

                await _applicationDBContext.AddAsync(newLike);
            }
            else
                _applicationDBContext.Likes.Remove(existingLike);


            await _applicationDBContext.SaveChangesAsync();
        }

        public async Task<int> GetLikeCountAsync(int articleID)
        {
            var count = await _applicationDBContext.Likes.Where(l => l.ArticleId.Equals(articleID)).CountAsync();

            return count;
        }
        public bool IsCookiesAccepted()
        {
            return _IHttpContextAccessor.HttpContext.Request.Cookies.ContainsKey("cookiesConsent");
        }

        public void AcceptCookies()
        {
            var options = new CookieOptions
            {
                Expires = DateTime.Now.AddYears(1),
                SameSite = SameSiteMode.Lax,
                HttpOnly = true,
                Secure = true
            };
            _IHttpContextAccessor.HttpContext.Response.Cookies.Append("cookiesConsent", "true", options);
        }
        public async Task<SpotNow>GetData()
        {
            var response = await _httpclient.GetAsync("https://spotprices.lexlink.se/espot");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<SpotNow>(data)!;
            }
            else
            {
                return new SpotNow();
            }
        }
    }
}
