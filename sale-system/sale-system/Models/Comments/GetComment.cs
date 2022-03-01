using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SaleSystem.Models.Comments
{
    public class GetComment
    {
        public int Id { get; set; }
        public bool OwnerMode { get; set; }
        public string Content { get; set; }
        public DateTime Date { get; set; }
        public int AuthorID { get; set; }
        public string AuthorName { get; set; }
        public bool IsLikedByUser { get; set; }
        public int LikesCount { get; set; }
    }
}
