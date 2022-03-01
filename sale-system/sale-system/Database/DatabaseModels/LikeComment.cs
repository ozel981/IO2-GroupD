using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SaleSystem.Database.DatabaseModels
{
    public class LikeComment
    {
        public int CommentID { get; set; }
        public Comment Comment { get; set; }
        public int? UserID { get; set; }
        public User User { get; set; }
    }
}
