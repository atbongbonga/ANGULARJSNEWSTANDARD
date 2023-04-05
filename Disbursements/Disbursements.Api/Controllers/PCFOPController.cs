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
    public class PCFOPController : ControllerBase
    {
        private HttpContext httpContext;
        private readonly PCFService service;

        public PCFOPController(IHttpContextAccessor accessor)
        {
            httpContext = accessor.HttpContext;
            var userCode = httpContext.User.FindFirstValue(ClaimTypes.Sid);
            this.service = new PCFService(userCode);
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




        [HttpPost("posttagpcfpayment")]
        public IActionResult PostNoSap(PCFOP data)
        {
            try
            {
                service.TagPcfPayment(data);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.GetBaseException().Message);
            }
        }

    }
}
