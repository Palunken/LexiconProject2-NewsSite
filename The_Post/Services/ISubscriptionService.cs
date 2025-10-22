using System.Threading.Tasks;
using The_Post.Models;
using The_Post.Models.VM;

namespace The_Post.Services
{
    public interface ISubscriptionService
    {
        Task<Subscription?> AddSubscription(string userId, int subscriptionTypeId);
        Task SendConfirmationEmailAsync(string userId, int subscriptionTypeId, string baseUrl);
        
        Task<bool> CancelSubscriptionAsync(string userId);
        Task<SubscriptionStatsVM> GetSubscriptionStats();
        Task<List<SubscriptionStatsVM>> GetSubscriptionStatsOverTime();
    }
}
