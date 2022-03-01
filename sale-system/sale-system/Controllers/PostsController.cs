using Microsoft.AspNetCore.Mvc;
using SaleSystem.Models.Posts;
using SaleSystem.Services;
using SaleSystem.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SaleSystem.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class PostsController : ControllerBase
    {
        private readonly IPostService _postService;
        public PostsController(IPostService _postService)
        {
            this._postService = _postService;
        }

        // GET: <PostsController>
        [HttpGet]
        public IActionResult GetAll([FromHeader] int userID)
        {
            IEnumerable<PostView> posts;
            try
            {
                posts = _postService.GetAll(userID);
            }
            catch (UnauthorizedAccessException e)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, e.Message);
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, e.Message);
            }
            return Ok(posts);
        }

        // GET: <PostsController>/filtered
        [HttpGet("filtered")]
        public IActionResult GetFiltered([FromHeader] int userID, [FromHeader] List<int> categoriesIDs)
        {
            IEnumerable<PostView> posts;
            try
            {
                posts = _postService.GetFilteredPosts(userID, categoriesIDs);
            }
            catch (UnauthorizedAccessException e)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, e.Message);
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, e.Message);
            }
            return Ok(posts);
        }

        // GET: <PostsController>/filtered/2
        [HttpGet("filtered/{page}")]
        public IActionResult GetFiltered([FromHeader] int userID, [FromHeader] List<int> categoriesIDs, int page, [FromHeader] int pageSize = 10)
        {
            PostPagedList postPagedList;
            try
            {
                postPagedList = _postService.GetFilteredPosts(userID, categoriesIDs, page, pageSize);
            }
            catch (UnauthorizedAccessException e)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, e.Message);
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, e.Message);
            }
            return Ok(postPagedList);
        }

        // GET <PostsController>/5
        [HttpGet("{userId}")]
        public IActionResult GetUserPosts(int userId)
        {
            IEnumerable<PostView> userPosts;
            try
            {
                userPosts = _postService.GetUserPosts(userId);
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, e.Message);
            }
            return Ok(userPosts);
        }
    }
}