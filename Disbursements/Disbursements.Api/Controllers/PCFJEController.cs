using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Disbursements.Library.PCF.Services;
using Disbursements.Library.PCF.ViewModels;
using System.Security.Claims;


namespace Disbursements.Api.Controllers
{
    [Route("api/pcfje")]
    [ApiController]
    public class PCFJEController : ControllerBase
    {
        private HttpContext httpContext;
        private readonly JournalEntryService service;

        public PCFJEController(IHttpContextAccessor accessor)
        {
            httpContext = accessor.HttpContext;
            var userCode = httpContext.User.FindFirstValue(ClaimTypes.Sid);
            this.service = new JournalEntryService(userCode);
        }

        [HttpPost("pcfje/post")]
        public IActionResult PostJe(JrnlEntryView data)
        {
            try
            {
                service.PostJrnlEntry(data);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.GetBaseException().Message);
            }
        }


    }
}
