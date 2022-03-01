using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaleSystem.Models.Comments;
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
    public class CommentController : Controller
    {
        private readonly ICommentService _commentService;
        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        // GET: <CommentController>/X
        [HttpGet("{id}")]
        public IActionResult GetComment(int id, [FromHeader] int userID)
        {
            GetComment comment;
            try
            {
                comment = _commentService.GetComment(id, userID);
            }
            catch (ObjectDisposedException ex)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, ex.Message);
            }
            return Ok(comment);
        }

        // PUT: <CommentController>/X
        [HttpPut("{id}")]
        public IActionResult UpdateComment(int id, [FromHeader] int userID, [FromBody] CommentUpdate comment)
        {
            CommentUpdateValidator validator = new CommentUpdateValidator();
            var result = validator.Validate(comment);
            if (!result.IsValid)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, result.Errors);
            }
            try
            {
                _commentService.UpdateComment(comment, id, userID);
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

        // DELETE: <CommentController>/X/
        [HttpDelete("{id}")]
        public IActionResult DeleteComment(int id, [FromHeader] int userID)
        {
            try
            {
                _commentService.RemoveComment(id, userID);
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

        // POST: <CommentController>
        [HttpPost]
        public IActionResult AddNewComment([FromHeader] int userID, [FromBody] NewComment comment)
        {
            NewCommentValidator validator = new NewCommentValidator();
            var result = validator.Validate(comment);
            if (!result.IsValid)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, result.Errors);
            }
            ResponseNewComment response;
            try
            {
                response = _commentService.AddComment(comment, userID);
            }
            catch (ObjectDisposedException ex)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, ex.Message);
            }
            catch (DbUpdateException ex)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, ex.Message);
            }
            return Ok(response);
        }

        // GET: <CommentController>/X/likedUsers
        [HttpGet("{id}/likedUsers")]
        public IActionResult GetUsersLikingAComment(int id)
        {
            IEnumerable<LikerID> likersIDs;
            try
            {
                likersIDs = _commentService.GetUsersLikingComment(id);
            }
            catch (ObjectDisposedException ex)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, ex.Message);
            }
            return Ok(likersIDs);
        }

        // PUT: <CommentController>/X/likedUsers
        [HttpPut("{id}/likedUsers")]
        public IActionResult UpdateCommentLikeStatus(int id, [FromHeader] int userID, [FromBody] CommentLikeStatusUpdate comment)
        {
            try
            {
                _commentService.UpdateCommentLikeStatus(id, comment, userID);
            }
            catch (ObjectDisposedException ex)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, ex.Message);
            }
            catch (DbUpdateException ex)
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
