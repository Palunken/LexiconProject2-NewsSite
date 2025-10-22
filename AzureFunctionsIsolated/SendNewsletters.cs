using System;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using The_Post.Models;
using The_Post.Data;
using static System.Net.WebRequestMethods;

namespace AzureFunctionsIsolated
{
    public class SendNewsletters
    {
        private readonly ILogger _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly IEmailSender _emailSender;

        public SendNewsletters(ILoggerFactory loggerFactory, ApplicationDbContext dbContext, IEmailSender emailSender)
        {
            _logger = loggerFactory.CreateLogger<SendNewsletters>();
            _dbContext = dbContext;
            _emailSender = emailSender;
        }
        // For testing, triggers every minute:   0 * * * * *    
        // The main function that sends the newsletter
        // This function is triggered every Friday at midnight
        [Function("SendNewsletter")]
        public async Task Run([TimerTrigger("0 0 0 * * 5")] TimerInfo myTimer)
        {
            // For local testing
            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            if (myTimer.ScheduleStatus is not null)
            {
                _logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
            }


            // Get all users that are subscribed to the newsletter
            var users = _dbContext.Users
                .Include(u => u.NewsletterCategories)
                .Where(u => u.IsSubscribedToNewsletter).ToList();

            // Get all articles that are not archived
            var allArticles = _dbContext.Articles
                .Include(a => a.Categories)
                .Where(a => a.IsArchived == false).ToList();

            // Get all articles that are editors choice
            var fetchedEditorsChoiceArticles = allArticles.Where(a => a.EditorsChoice).ToList();

            // Loop through each user and send them a newsletter
            foreach (var user in users)
            {
                List<List<Article>> articlesByCategory = new List<List<Article>>();
                List<Article> articlesEditorsChoice = new List<Article>();

                // Check if the user has opted in to include editors choice articles
                if (user.EditorsChoiceNewsletter)
                {
                    articlesEditorsChoice = fetchedEditorsChoiceArticles;
                }

                // If the user has opted in to receive articles by category
                if (user.NewsletterCategories.Count != 0)
                {
                    // Get the top 5 most viewed articles in each category for the past week
                    foreach (var category in user.NewsletterCategories)
                    {
                        var articles = allArticles
                            .Where(a => a.Categories.Contains(category))
                            .Where(a => a.DateStamp.AddDays(7) >= DateTime.UtcNow)
                            .OrderByDescending(a => a.Views) 
                            .Take(5).ToList();

                        articlesByCategory.Add(articles);
                    }
                }

                // Get the category names for the email-headings (e.g. "The latest in Sports news")
                List<string> categoryNames = user.NewsletterCategories.Select(c => c.Name).ToList();

                // Build the HTML content for the email, calls the BuildEmailHtml method
                var emailContent = BuildEmailHtml(articlesByCategory, categoryNames, fetchedEditorsChoiceArticles);


                // Send the newsletter
                try
                {
                    await _emailSender.SendEmailAsync(user.Email, "Your Weekly Newsletter", emailContent);

                    _logger.LogInformation($"Sending newsletter to {user.Email}");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Failed to send newsletter to {user.Email}: {ex.Message}");
                }
            }

        }

        // Builds the HTML content for the email
        public string BuildEmailHtml(List<List<Article>> articlesByCategory, List<string> categoryNames, List<Article> articlesEditorsChoice)
        {
            var htmlContent = "<html><head>";

            // Email-safe styles (no external CSS)
            htmlContent += @"
            <style>
                body {
                    text-align: center;
                    font-family: Arial, sans-serif;
                }
                .container {
                    width: 100%;
                    max-width: 600px; /* Set a max width */
                    margin: 0 auto;
                }
                img {
                    width: 100%;
                    max-width: 600px; /* Set a max width */
                    height: auto;
                    margin: 0 auto;
                    display: block;
                }
            </style>";

            htmlContent += "</head><body style='text-align:center'>";

            htmlContent += "<h1>Your Weekly Newsletter</h1>";

            htmlContent += "<div class='container'>";

            // Insert the Editors Choice section
            htmlContent += BuildEditorsChoiceHtml(articlesEditorsChoice);

            // Insert each category section
            int categoryIndex = 0;
            foreach (var categoryArticles in articlesByCategory)
            {
                htmlContent += BuildCategoryHtml(categoryArticles, categoryNames[categoryIndex++]);
            }

            // Close the container and the HTML document
            htmlContent += "</div></body></html>";

            return htmlContent;
        }

        // The base URL for viewing an article. Only the article ID needs to be appended to this URL.
        string baseUrlViewArticle = "https://thepost.azurewebsites.net/Article/ViewArticle?articleID=";

        // For local testing
        //string baseUrlViewArticle = "https://localhost:7116/Article/ViewArticle?articleID=";

        // Builds the HTML content for the Editors Choice section
        public string BuildEditorsChoiceHtml(List<Article> articles)
        {
            var htmlContent = "<h2>Editors Choice</h2><ul style='display: inline-block; padding: 0;'>";

            foreach (var article in articles)
            {
                // Encode spaces in the image URL. Otherwise, the email client may not display the image.
                string encodedImageSmall = article.ImageSmallLink.Replace(" ", "%20");

                // If the small image is not available, use the original image
                string encodedImageOriginal = article.ImageOriginalLink.Replace(" ", "%20");


                // Create the URL for viewing the article
                var articleUrl = baseUrlViewArticle + article.Id;

                // Encode spaces in the image URL. Otherwise, the email client may not display the image.
                string encodedImage = article.ImageOriginalLink.Replace(" ", "%20");

                htmlContent += $@"
                <li style='list-style: none; text-align: center; margin-bottom: 40px;'>
                    <div style='max-width:100%; background:#cccccc; text-align:center;'>
                        <img src='{encodedImageSmall}' 
                             onerror=""this.onerror=null; this.src='{{encodedImageOriginal}}';"" 
                             alt='Image not available'>
                    </div>
                    <h3>
                        <a href='{articleUrl}'>{article.HeadLine}</a>
                    </h3>
                    <p style='text-align: left;'>{article.ContentSummary}</p>
                    <hr style='margin-top: 25px'>
                </li>";
            }

            htmlContent += "</ul>";
            return htmlContent;
        }

        // Builds the HTML content for a category section
        public string BuildCategoryHtml(List<Article> articles, string category)
        {
            var htmlContent = $"<h2>The Most Read {category} News Articles This Week</h2><ul style='display: inline-block; padding: 0;'>";

            foreach (var article in articles)
            {
                // Create the URL for viewing the article
                var articleUrl = baseUrlViewArticle + article.Id;

                // Encode spaces in the image URL. Otherwise, the email client may not display the image.
                string encodedImageSmall = article.ImageSmallLink.Replace(" ", "%20");

                // If the small image is not available, use the original image
                string encodedImageOriginal = article.ImageOriginalLink.Replace(" ", "%20");

                htmlContent += $@"
                <li style='list-style: none; text-align: center; margin-bottom: 40px;'>
                    <div style='max-width:100%; background:#cccccc; text-align:center;'>
                        <img src='{encodedImageSmall}' 
                             onerror=""this.onerror=null; this.src='{{encodedImageOriginal}}';"" 
                             alt='Image not available'>
                    </div>
                    <h3>
                        <a href='{articleUrl}'>{article.HeadLine}</a>
                    </h3>
                    <p style='text-align: left;'>{article.ContentSummary}</p>
                    <hr style='margin-top: 25px'>
                </li>";
            }
            htmlContent += "</ul>";
            return htmlContent;
        }



    }
}
