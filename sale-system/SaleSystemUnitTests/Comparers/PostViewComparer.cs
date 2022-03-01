using SaleSystem.Models.Posts;
using SaleSystem.Models.Categories;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SaleSystemUnitTests.Comparers
{
    class PostViewComparer : IEqualityComparer<PostView>
    {
        public bool Equals(PostView x, PostView y)
        {
            if (x == null && y == null)
                return true;
            else if (x == null || y == null)
                return false;
            else if (x.ID != y.ID ||
                x.Content != y.Content ||
                x.AuthorID != y.AuthorID ||
                x.Title != y.Title)
                return false;

            Assert.Equal(x.Category, y.Category);
            return true;
        }

        public int GetHashCode([DisallowNull] PostView obj)
        {
            return obj.GetHashCode();
        }
    }
}
