using Microsoft.AspNetCore.Mvc;
using SaleSystem.Models.Categories;
using SaleSystem.Models.Users;
using SaleSystem.Services;
using SaleSystem.Validators;
using System;
using System.Collections.Generic;
using System.Net;

namespace SaleSystem.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        public UsersController(IUserService _userService)
        {
            this._userService = _userService;
        }

        // GET: <UsersController>
        [HttpGet]
        public IActionResult Get()
        {
            IEnumerable<UserView> users;
            try
            {
                users = _userService.GetAll();
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, e.Message);
            }
            return Ok(users);
        }

        // GET <UsersController>/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            UserView user;
            try
            {
                user = _userService.GetUser(id);
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, e.Message);
            }
            return Ok(user);
        }

        // PUT <UsersController>/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] UserUpdate user, [FromHeader] int userID)
        {
            UserUpdateValidator validator = new UserUpdateValidator();
            var result = validator.Validate(user);
            if (!result.IsValid)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, result.Errors);
            }
            try
            {
                _userService.UpdateUser(id, user, userID);
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

        // DELETE <UsersController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id, [FromHeader] int userID)
        {
            try
            {
                _userService.DeleteUser(id, userID);
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

        // POST <UsersController>
        [HttpPost]
        public IActionResult Post([FromBody] UserUpdate user)
        {
            ResponseNewUser response;
            UserUpdateValidator validator = new UserUpdateValidator();
            var result = validator.Validate(user);
            if (!result.IsValid)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, result.Errors);
            }
            try
            {
                response = _userService.CreateUser(user);
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, e.Message);
            }
            return Ok(response);
        }

        // GET: <UsersController>/5/subscribedCategories
        [HttpGet("{id}/subscribedCategories")]
        public IActionResult GetSubscribedCategories(int id)
        {
            IEnumerable<CategoryID> categories;
            try
            {
                categories = _userService.GetSubscribedCategories(id);
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, e.Message);
            }
            return Ok(categories);
        }
    }
}
