using Microsoft.AspNetCore.Mvc;
using SaleSystem.Models.Posts;
using SaleSystem.Services;
using SaleSystem.Services.Interfaces;
using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Collections.Generic;
using SaleSystem.Models.Comments;
using SaleSystem.Validators;

namespace SaleSystem.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class PostController : Controller
    {
        private readonly IPostService _postService;
        private readonly INewsletterService _newsletterService;
        private readonly ICommentService _commentService;
        public PostController(IPostService postService, INewsletterService newsletterService, ICommentService commentService)
        {
            _postService = postService;
            _newsletterService = newsletterService;
            _commentService = commentService;
        }

        // GET <PostController>/5
        [HttpGet("{id}")]
        public IActionResult Get(int id, [FromHeader] int userId)
        {
            PostView post;
            try
            {
                post = _postService.GetPost(id, userId);
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, e.Message);
            }
            return Ok(post);
        }

        // POST <PostController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] PostCreate post, [FromHeader] int userId)
        {
            ResponseNewPost response;
            PostCreateValidator validator = new PostCreateValidator();
            var result = validator.Validate(post);
            if (!result.IsValid)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, result.Errors);
            }
            try
            {
                response = _postService.CreatePost(post, userId);
                await _newsletterService.Notify(post.CategoryID);
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, e.Message);
            }
            return Ok(response);
        }

        // PUT <PostController>/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] PostUpdate post, [FromHeader] int userId)
        {
            PostUpdateValidator validator = new PostUpdateValidator();
            var result = validator.Validate(post);
            if (!result.IsValid)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, result.Errors);
            }
            try
            {
                _postService.UpdatePost(id, post, userId);
            }
            catch (UnauthorizedAccessException e)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, e.Message);
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, e.Message);
            }
            return Ok();
        }

        //GET <PostController>/5/comments
        [HttpGet("{postId}/comments")]
        public IActionResult GetPostComments([FromHeader] int userId, int postId)
        {
            IEnumerable<CommentView> result;

            try
            {
                result = _commentService.GetPostComments(postId, userId);
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, e.Message);
            }

            return Ok(result);
        }

        // GET <PostController>/5/likeUsers
        [HttpGet("{postId}/likedUsers")]
        public IActionResult GetPostLikers(int postId)
        {
            IEnumerable<PostLikerID> postLikers;
            try
            {
                postLikers = _postService.GetPostLikers(postId);
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, e.Message);
            }

            return Ok(postLikers);
        }

        // DELETE <PostsController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id, [FromHeader] int userId)
        {
            try
            {
                _postService.DeletePost(id, userId);
            }
            catch (UnauthorizedAccessException e)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, e.Message);
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, e.Message);
            }

            return Ok();
        }

        // POST <PostController>/promote
        [HttpPost("promote")]
        public IActionResult Promote([FromBody] PostPromotion p, [FromHeader] int userId)
        {
            try
            {
                _postService.PromotePost(p, userId);
            }
            catch (UnauthorizedAccessException e)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, e.Message);
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, e.Message);
            }
            return Ok();
        }

        // PUT: <PostController>/X/likeUsers
        [HttpPut("{id}/likedUsers")]
        public IActionResult LikePost(int id, [FromHeader] int userId, [FromBody] PostLikeStatusUpdate statusUpdate)
        {
            try
            {
                _postService.UpdatePostLikeStatus(id, statusUpdate, userId);
            }
            catch (ObjectDisposedException ex)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, ex.Message);
            }
            catch (DbUpdateException ex)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, ex.Message);
            }
            return Ok();
        }
    }
}
