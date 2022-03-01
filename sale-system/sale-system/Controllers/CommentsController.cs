using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaleSystem.Models.Categories;
using SaleSystem.Models.Comments;
using SaleSystem.Models.Users;
using SaleSystem.Services;
using SaleSystem.Services.Interfaces;
using SaleSystem.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SaleSystem.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _commentService;
        public CommentsController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        // GET: <CommentsController>
        [HttpGet]
        public IActionResult GetComments([FromHeader] int userID)
        {
            IEnumerable<CommentView> comments;
            try
            {
                comments = _commentService.GetComments(userID);
            }
            catch (ObjectDisposedException ex)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, ex.Message);
            }
            return Ok(comments);
        }
    }
}
