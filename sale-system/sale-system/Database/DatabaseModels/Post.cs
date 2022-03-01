using System;
using System.Collections.Generic;

namespace SaleSystem.Database.DatabaseModels
{
    public class Post
    {
        public int ID { get; set; }
        public User Creator { get; set; }
        public User Enterpreneur { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreationDateTime { get; set; }
        public DateTime? PromotionEndDateTime { get; set; }
        public Category Category { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<LikePost> LikePost { get; set; }
    }
}
