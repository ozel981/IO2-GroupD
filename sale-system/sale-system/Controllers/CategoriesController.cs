using Microsoft.AspNetCore.Mvc;
using SaleSystem.Models.Categories;
using SaleSystem.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SaleSystem.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // GET: <CategoriesController>
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_categoryService.GetAll());
        }
    }
}
