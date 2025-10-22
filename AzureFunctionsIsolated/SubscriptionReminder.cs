using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using The_Post.Data;
using The_Post.Services;
using Microsoft.AspNetCore.Identity.UI.Services;

public class SubscriptionReminder
{
    private readonly ApplicationDbContext _context;
    private readonly IEmailSender _emailSender;
    private readonly ILogger _logger;
    

    public SubscriptionReminder(ApplicationDbContext context, IEmailSender emailSender, ILoggerFactory loggerFactory)
    {
        _context = context;
        _emailSender = emailSender;
        _logger = loggerFactory.CreateLogger<SubscriptionReminder>();
    }
    // 0 0 8 * * *
    [Function("SubscriptionReminder")]
    public async Task Run([TimerTrigger("0 * * * * *")] TimerInfo myTimer)
    {
        _logger.LogInformation($"Azure Function executed at: {DateTime.UtcNow}");

        // Define expiry threshold (e.g., 3 days before expiration)
        DateTime thresholdDate = DateTime.UtcNow.AddDays(3);

        // Get subscriptions that are expiring soon
        var expiringSubscriptions = _context.Subscriptions
            .Include(s => s.User)
            .Where(s => s.Expires.Date == thresholdDate.Date)
            .ToList();

        foreach (var subscription in expiringSubscriptions)
        {
            string email = subscription.User.Email;
            string subject = "Subscription Expiry Reminder";
            string message = $@"
                <p>Dear {subscription.User.FirstName},</p>
                <p>Your subscription is expiring on <strong>{subscription.Expires:yyyy-MM-dd}</strong>.</p>
                <p>To continue enjoying our services, please renew your subscription.</p>
                <p>Best regards, <br/> The Post Team</p>";

            // Send email
            await _emailSender.SendEmailAsync(email, subject, message);
            _logger.LogInformation($"Reminder email sent to: {email}");
        }
    }
}