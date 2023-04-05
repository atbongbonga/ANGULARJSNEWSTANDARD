using Disbursements.Library.COPS.Services;
using Disbursements.Library.COPS.ViewModels.JGMAN;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Disbursements.Api.Controllers
{
    [Authorize]
    [Route("api/jgman")]
    [ApiController]
    public class JGMANController : ControllerBase
    {
        private HttpContext httpContext;
        private readonly JGMANService service;
        public JGMANController(IHttpContextAccessor accessor)
        {
            httpContext = accessor.HttpContext;
            var userCode = httpContext.User.FindFirstValue(ClaimTypes.Sid);
            this.service = new JGMANService(userCode);
        }

        [HttpPost("payments/post")]
        public IActionResult PostPayments(IEnumerable<SummaryView> data)
        {
            try
            {
                service.PostPayments(data);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.GetBaseException().Message);
            }
        }

        [HttpGet("payments/summary")]
        public ActionResult<SummaryView> GetSummary(string gen_id, string type, bool active)
        {
            try
            {
                return Ok(service.GetSummary(gen_id, type, active));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.GetBaseException().Message);
            }
        }

        [HttpGet("payments/details")]
        public ActionResult<DetailView> GetDetails(string gen_id, string type, string branch)
        {
            try
            {
                return Ok(service.GetDetails(gen_id, type, branch));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.GetBaseException().Message);
            }
        }
    }
}
