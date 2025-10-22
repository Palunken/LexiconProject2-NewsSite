using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using The_Post.Models;
using The_Post.Services;
using The_Post.Areas.Identity.Pages;

public class ConfirmSubscriptionModel : BaseCookiesPageModel
{
    private readonly UserManager<User> _userManager;
    private readonly ISubscriptionService _subscriptionService;

    public bool IsSuccess { get; private set; }  //  property

    public ConfirmSubscriptionModel(UserManager<User> userManager, ISubscriptionService subscriptionService, IArticleService articleService)
        : base(articleService)
    {
        _userManager = userManager;
        _subscriptionService = subscriptionService;
    }

    public async Task<IActionResult> OnGetAsync(string userId, string token, int subscriptionTypeId)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            return BadRequest("Invalid confirmation link.");

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return NotFound();

        var isValid = await _userManager.VerifyUserTokenAsync(user, TokenOptions.DefaultProvider, "SubscriptionConfirmation", token);
        if (!isValid)
            return BadRequest("Invalid or expired token.");

        // Add subscription after confirmation
        var subscription = await _subscriptionService.AddSubscription(userId, subscriptionTypeId);
        if (subscription == null)
        {
            TempData["ErrorMessage"] = "Subscription activation failed.";
            IsSuccess = false;  //  Set to false if the subscription fails
            return RedirectToPage("Subscription");
        }

        IsSuccess = true;  //  Set to true when subscription is successful
        return RedirectToPage("SubscriptionSuccess", new { expireDate = subscription.Expires.ToString("yyyy-MM-dd") });
    }
}