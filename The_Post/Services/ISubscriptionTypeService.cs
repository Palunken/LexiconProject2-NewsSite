using The_Post.Models;

namespace The_Post.Services
{
    public interface ISubscriptionTypeService
    {
        Task<List<SubscriptionType>> GetAllSubscriptionTypes();
        Task<SubscriptionType?> GetCurrentSubscriptionTypeAsync(string userId);
        Task<SubscriptionType?> GetByIdAsync(int id);
        Task Create(SubscriptionType subType);
        Task Edit(SubscriptionType subType);
        Task Delete(int id);
        Task<bool> Exists(int id);
    }
}