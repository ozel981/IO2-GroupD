using SaleSystem.Database.DatabaseModels;
using SaleSystem.Models.Categories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SaleSystem.Services
{
    public interface ICategoryService
    {
        public IEnumerable<CategoryView> GetAll();
    }
}
