using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SaleSystem.Database.DatabaseModels
{
    public class Comment
    {
        public int ID { get; set; }
        public string Content { get; set; }
        public User User { get; set; }
        public int PostID { get; set; }
        public Post Post { get; set; }
        public DateTime CreationDateTime { get; set; }

        public virtual ICollection<LikeComment> LikeComment { get; set; }
    }
}
