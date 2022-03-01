using Microsoft.AspNetCore.Mvc;

using SaleSystem.Models.Newsletters;
using SaleSystem.Services.Interfaces;
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
    public class NewsletterController : ControllerBase
    {
        private readonly INewsletterService _newsletterService;
        public NewsletterController(INewsletterService newsletterService)
        {
            _newsletterService = newsletterService;
        }

        // POST: <NewsletterController>/<user_id>
        [HttpPost]
        public IActionResult Subscribe([FromHeader] int userID, [FromBody] SubscriptionInfo info)
        {
            try
            {
                _newsletterService.Subscribe(userID, info);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, ex.Message);
            }
            return Ok();
        }
    }
}
