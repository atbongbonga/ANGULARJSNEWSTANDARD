using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Disbursements.Library.COPS.ViewModels;
using Disbursements.Library.COPS.Services;

namespace Disbursements.Api.Controllers
{
    [ApiController, Route("api/payment"), Authorize]

    public class PaymentController : ControllerBase
    {
        private PaymentService service;
        private HttpContext httpContext;
        public PaymentController(IHttpContextAccessor accessor)
        {
            httpContext = accessor.HttpContext;
            var userCode = httpContext.User.FindFirstValue("EmpCode");
            service = new PaymentService(userCode);
        }

        [HttpPost, Route("post")]
        public IActionResult PostPayment(PaymentView payment)
        {
            try
            {
              
                service.PostPayment(payment);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.GetBaseException().Message);
            }
        }

        [HttpPost, Route("update")]
        public IActionResult UpdatePayment(PaymentHeaderView payment)
        {
            try
            {
                service.UpdatePayment(payment);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.GetBaseException().Message);
            }
        }

        [HttpPost, Route("cancel")]
        public IActionResult CancelPayment(int docNum)
        {
            try
            {
                service.CancelPayment(docNum);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.GetBaseException().Message);
            }
        }
    }
}
