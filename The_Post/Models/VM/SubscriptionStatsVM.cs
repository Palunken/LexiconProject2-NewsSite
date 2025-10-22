namespace The_Post.Models.VM
{
    public class SubscriptionStatsVM
    {
        public DateTime Month { get; set; } // Represents the first day of the month
        public int TotalSubscribers { get; set; }
        public int ActiveSubscriptions { get; set; }
        public int ExpiredSubscriptions { get; set; }
    }
}