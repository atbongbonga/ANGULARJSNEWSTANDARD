using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Disbursements.Library.PCF.Services;
using Disbursements.Library.PCF.ViewModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Disbursements.Api.Controllers
{
    [Route("api/pcfop"), Authorize]
    [ApiController]
    public class PCFOPController : Controller
    {
           private HttpContext httpContext;
        private readonly PCFService service;

        public PCFOPController(IHttpContextAccessor accessor)
        {
            httpContext = accessor.HttpContext;
            var userCode = httpContext.User.FindFirstValue(ClaimTypes.Sid);
            this.service = new PCFService(userCode);
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost("post")]
        public IActionResult PostOP(PCFOP data)
        {
            try
            {
                service.PostPayment(data);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.GetBaseException().Message);
            }
        }
    }
}
