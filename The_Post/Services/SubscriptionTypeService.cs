using Microsoft.EntityFrameworkCore;
using The_Post.Data; 
using The_Post.Models;

namespace The_Post.Services
{
    public class SubscriptionTypeService : ISubscriptionTypeService
    {
        private readonly ApplicationDbContext _db;

        public SubscriptionTypeService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<SubscriptionType>> GetAllSubscriptionTypes()
        {
            return await _db.SubscriptionTypes.ToListAsync();
        }

        public async Task<SubscriptionType?> GetCurrentSubscriptionTypeAsync(string userId)
        {
            var activeSubscription = await _db.Subscriptions
                .Include(s => s.SubscriptionType)  // Include the related subscription type
                .Where(s => s.UserId == userId
                            && s.PaymentComplete
                            && s.Expires > DateTime.UtcNow)
                .OrderByDescending(s => s.Created)  
                .FirstOrDefaultAsync();

            return activeSubscription?.SubscriptionType;
        }

        public async Task<SubscriptionType?> GetByIdAsync(int id)
        {
            return await _db.SubscriptionTypes.FindAsync(id);
        }

        public async Task Create(SubscriptionType subType)
        {
            _db.SubscriptionTypes.Add(subType);
            await _db.SaveChangesAsync();
        }

        public async Task Edit(SubscriptionType subType)
        {
            var existingSubType = await _db.SubscriptionTypes.FindAsync(subType.Id);
            if (existingSubType == null)
            {
                throw new ArgumentException("SubscriptionType not found", nameof(subType.Id));
            }

            _db.Entry(existingSubType).CurrentValues.SetValues(subType);
            await _db.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var subType = await GetByIdAsync(id);
            if (subType == null)
            {
                throw new ArgumentException("SubscriptionType not found", nameof(id));
            }
            _db.SubscriptionTypes.Remove(subType);
            await _db.SaveChangesAsync();
        }

        public async Task<bool> Exists(int id)
        {
            return await _db.SubscriptionTypes.AnyAsync(st => st.Id == id);
        }
    }
}
