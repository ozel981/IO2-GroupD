using SaleSystem.Database;
using SaleSystem.Database.DatabaseModels;
using SaleSystem.Models.Categories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SaleSystem.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly SaleSystemDBContext _context;

        public CategoryService(SaleSystemDBContext context)
        {
            _context = context;
        }

        public IEnumerable<CategoryView> GetAll()
        {

            List<CategoryView> categories = new List<CategoryView>();
            var dbCategories = _context.Categories.AsQueryable();

            foreach (Category cat in dbCategories)
            {
                categories.Add(new CategoryView
                {
                    ID = cat.ID,
                    Name = cat.Name,
                });
            }

            return categories;
        }
    }
}
