using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Disbursements.Library.COPS.ViewModels.Utility;
using Disbursements.Library.COPS.Services;


namespace Disbursements.Api.Controllers
{
    [ApiController, Route("api/utility"), Authorize]

    public class UtilityPaymentController : ControllerBase
    {
        private UtilityService service;
        private HttpContext httpContext;
        public UtilityPaymentController(IHttpContextAccessor accessor)
        {
            httpContext = accessor.HttpContext;
            var userCode = httpContext.User.FindFirstValue("EmpCode");
<<<<<<< HEAD
            service = new UtilityPaymentService(userCode);
              
=======
            service = new UtilityService(userCode);

>>>>>>> a88d4076171846ecc6bb80e9a1b27f1a93ee2fb7
        }

        [HttpPost, Route("post")]
        public IActionResult PostUtilityPayment(PaymentUtilityView payment)
        {
            try
            {
                service.PostUtilityPayment(payment);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.GetBaseException().Message);
            }
        }
    }
}
