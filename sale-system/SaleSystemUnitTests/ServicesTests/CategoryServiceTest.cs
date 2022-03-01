using SaleSystem.Database;
using Microsoft.EntityFrameworkCore;
using Moq;
using SaleSystem.Database.DatabaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using SaleSystem.Services;
using SaleSystem.Models.Categories;
using System.Diagnostics.CodeAnalysis;
using SaleSystemUnitTests.Comparers;

namespace SaleSystemUnitTests.ServicesTests
{
    public class CategoryServiceTest
    {
        private readonly Mock<SaleSystemDBContext> _mockContext;
        private readonly Mock<SaleSystemDBContext> _mockEmptyContext;

        private readonly int _categoryCount;
        private readonly List<Category> _expectedCategories;
        private readonly List<Category> _emptyCategories;
        public CategoryServiceTest()
        {
            _emptyCategories = new List<Category>();
            _expectedCategories = GetSampleCategories();
            _categoryCount = _expectedCategories.Count;

            var mockEmptyCategoriesSet = new Mock<DbSet<Category>>();
            mockEmptyCategoriesSet.Setup(m => m.AsQueryable()).Returns(_emptyCategories.AsQueryable());

            _mockEmptyContext = new Mock<SaleSystemDBContext>();
            _mockEmptyContext.Setup(c => c.Categories).Returns(mockEmptyCategoriesSet.Object);

            var mockCategoriesSet = new Mock<DbSet<Category>>();
            mockCategoriesSet.Setup(m => m.AsQueryable()).Returns(_expectedCategories.AsQueryable());

            _mockContext = new Mock<SaleSystemDBContext>();
            _mockContext.Setup(c => c.Categories).Returns(mockCategoriesSet.Object);
        }

        [Fact]
        public void GetAll_ValidCall()
        {
            var categoryServiceWithEmptyDB = new CategoryService(_mockEmptyContext.Object);
            var categoryService = new CategoryService(_mockContext.Object);

            var categories = categoryService.GetAll();
            var emptyCategories = categoryServiceWithEmptyDB.GetAll();

            Assert.Equal(_categoryCount, categories.Count());
            Assert.Equal(ConvertIntoCategoryView(_expectedCategories), categories, new CategoryViewComparer());
            Assert.Empty(emptyCategories);
        }

        private static List<Category> GetSampleCategories()
        {
            return new List<Category>
            {
                new Category
                {
                    ID = 1,
                    Name = "Buty"
                },
                new Category
                {
                    ID = 2,
                    Name = "RTV/AGD"
                },
                new Category
                {
                    ID = 3,
                    Name = "Elektronika"
                },
                new Category
                {
                    ID = 4,
                    Name = "Ubrania"
                },
                new Category
                {
                    ID = 5,
                    Name = "Telefony"
                },
                new Category
                {
                    ID = 6,
                    Name = "Jedzenie"
                },
                new Category
                {
                    ID = 7,
                    Name = "?$5%"
                },
                new Category
                {
                    ID = 8,
                    Name = ","
                },
            };
        }

        private static List<CategoryView> ConvertIntoCategoryView(List<Category> categories)
        {
            List<CategoryView> categoryViews = new List<CategoryView>();
            foreach (Category cat in categories)
            {
                categoryViews.Add(new CategoryView
                {
                    ID = cat.ID,
                    Name = cat.Name
                });
            }

            return categoryViews;
        }
    }
}
