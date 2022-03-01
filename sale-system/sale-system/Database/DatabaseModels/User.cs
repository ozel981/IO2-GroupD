using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SaleSystem.Database.DatabaseModels
{
    public class User
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string EmailAddress { get; set; }
        public UserType Type { get; set; }
        public bool IsActive { get; set; }
        public bool IsVerified { get; set; }
        public DateTime CreationDateTime { get; set; }

        public virtual ICollection<Post> PostsCreated { get; set; }
        public virtual ICollection<Post> PostsAboutMyEnterprise { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }

        public virtual ICollection<SubscriberCategory> SubscriberCategory { get; set; }
        public virtual ICollection<LikePost> LikePost { get; set; }
        public virtual ICollection<LikeComment> LikeComment { get; set; }
        public bool IsAdmin()
        {
            return Type == UserType.Admin;
        }
        public bool IsEntrepreneur()
        {
            return Type == UserType.Entrepreneur;
        }
    }
    public enum UserType
    {
        Individual,
        Entrepreneur,
        Admin
    }
}
