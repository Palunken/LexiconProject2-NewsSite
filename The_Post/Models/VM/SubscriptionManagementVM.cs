namespace The_Post.Models.VM
{
    public class SubscriptionManagementVM
    {
        public SubscriptionManagementVM()
        {
            SubscriptionTypes = new List<SubscriptionType>();
            NewSubscriptionType = new SubscriptionType();
        }

        // Holds a list of existing subscription types
        public IEnumerable<SubscriptionType> SubscriptionTypes { get; set; }

        // Holds the new subscription type being created
        public SubscriptionType NewSubscriptionType { get; set; }
    }
}
