namespace SaleSystem.Database.DatabaseModels
{
    public class LikePost
    {
        public int PostID { get; set; }
        public Post Post { get; set; }
        public int? UserID { get; set; }
        public User User { get; set; }
    }
}
