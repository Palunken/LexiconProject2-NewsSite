using System;
using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using The_Post.Data;
using The_Post.Models;

namespace AzureFunctionsIsolated
{
    public class ArchiveArticles
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _db;

        public ArchiveArticles(ILoggerFactory loggerFactory, ApplicationDbContext db)
        {
            _logger = loggerFactory.CreateLogger<ArchiveArticles>();
            _db = db;
        }

        // For testing: 0 * * * * *    
        // Timer setup to run every day at midnight
        [Function("ArchiveArticles")]
        public void Run([TimerTrigger("0 0 0 * * *")] TimerInfo myTimer)
        {   
            var articlesToArchive = _db.Articles
                .Where(a => !a.IsArchived && a.DateStamp.AddDays(30) <= DateTime.UtcNow).ToList();

            if (articlesToArchive.Count == 0)
            {
                return; // Exit early if no articles need archiving
            }

            // For testing purposes, log the IDs of the articles that will be archived
            var articlesArchivedIDs = articlesToArchive.Select(a => a.Id).ToList();

            foreach (var article in articlesToArchive)
            {
                if (article.EditorsChoice)
                    article.EditorsChoice = false;

                article.IsArchived = true;
            }

            _db.SaveChanges();

            // -- Logging, for testing purposes --
            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}. These articles were archived (id): \n{string.Join("\n", articlesArchivedIDs)}");

            if (myTimer.ScheduleStatus is not null)
            {
                _logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
            }

            // -- End Logging --
        }
    }
}
