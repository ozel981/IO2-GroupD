using SaleSystem.Models;
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
    class CategoryViewComparer : IEqualityComparer<CategoryView>
    {
        public bool Equals(CategoryView x, CategoryView y)
        {
            if (x == null && y == null)
                return true;
            else if (x == null || y == null)
                return false;
            else if (x.ID != y.ID || x.Name != y.Name)
                return false;
            return true;
        }

        public int GetHashCode([DisallowNull] CategoryView obj)
        {
            return obj.GetHashCode();
        }
    }
}
