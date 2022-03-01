
using SaleSystem.Models.Users;
using System;

namespace SaleSystem.Models.Comments
{
    public class CommentView
    {
        public int ID { get; set; }
        public bool OwnerMode { get; set; }
        public string Content { get; set; }
        public int AuthorID { get; set; }
        public string AuthorName { get; set; }
        public DateTime Date { get; set; }
        public int LikesCount { get; set; }
        public bool IsLikedByUser { get; set; }
    }
}
