using Disbursements.Library.PaymentRequisition.Services;
using Disbursements.Library.PaymentRequisition.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace Disbursements.Api.Controllers.PaymentRequisition
{

    [Route("api/paymentrequisition")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private HttpContext httpContext;
        private readonly PaymentService service;
        public PaymentController(IHttpContextAccessor accessor)
        {
            httpContext = accessor.HttpContext;
            var userCode = httpContext.User.FindFirstValue(ClaimTypes.Sid);
            this.service = new PaymentService(userCode);
        }

        [HttpPost("payments/post")]
        public IActionResult PostPayments(int docEntry, int sapEntry, string cardCode)
        {
            try
            {
                service.PostPayment(docEntry , sapEntry , cardCode);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.GetBaseException().Message);
            }
        }


    }

   
}
