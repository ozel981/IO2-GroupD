using SaleSystem.Database.DatabaseModels;
using SaleSystem.Models.Categories;
using SaleSystem.Models.Comments;
using SaleSystem.Models.Users;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SaleSystem.Models.Posts
{
    public class PostView
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime DateTime { get; set; }
        public string Category { get; set; }
        public bool IsPromoted { get; set; }
        public string AuthorName { get; set; }
        public int AuthorID { get; set; }
        public int LikesCount { get; set; }
        public bool IsLikedByUser { get; set; }
    }
}
