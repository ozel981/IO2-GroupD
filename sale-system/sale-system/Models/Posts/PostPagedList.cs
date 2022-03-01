using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SaleSystem.Models.Posts
{
    public class PostPagedList
    {
        public List<PostView> Posts { get; set; }
        public bool IsLastPage { get; set; }
    }
}
