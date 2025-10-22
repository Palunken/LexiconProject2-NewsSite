namespace The_Post.Models
{
    public class Like
    {
        public int Id { get; set; }
        public int ArticleId { get; set; }
        public string UserId { get; set; }

        public Article Article { get; set; }
        public User User { get; set; }
    }
}
