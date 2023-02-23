using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PF.Library.Services;
using System.Security.Claims;

namespace PF.Api.Controllers
{
    [ApiController, Route("api/bank_arrangement"), Authorize]
    public class BankArrangementController : ControllerBase
    {
        private BankArrangementService service;
        private HttpContext httpContext;

        public BankArrangementController(IHttpContextAccessor accessor)
        {
            httpContext = accessor.HttpContext;
            var userCode = httpContext.User.FindFirstValue("EmpCode");
            service = new BankArrangementService(userCode);
        }

        [HttpPost, Route("setup/post")]
        public IActionResult PostSetup(int id)
        {
            try
            {
                service.PostSetup(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.GetBaseException().Message);
            }
        }

        [HttpPost, Route("accrual/post")]
        public IActionResult PostAccrual(int id)
        {
            try
            {
                service.PostAccrual(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.GetBaseException().Message);
            }
        }

        [HttpPost, Route("reversal/post")]
        public IActionResult PostReversal(int id)
        {
            try
            {
                service.PostReversal(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.GetBaseException().Message);
            }
        }

        [HttpPost, Route("payment/post")]
        public IActionResult PostPayment(DateTime bank_date, [FromBody]IEnumerable<int> docentries)
        {
            try
            {
                service.PostPayment(bank_date, docentries);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.GetBaseException().Message);
            }
        }

    }
}
