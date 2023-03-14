﻿using Microsoft.AspNetCore.Authorization;
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
        private UtilityPaymentService service;
        private HttpContext httpContext;
        public UtilityPaymentController(IHttpContextAccessor accessor)
        {
            httpContext = accessor.HttpContext;
            var userCode = httpContext.User.FindFirstValue("EmpCode");
            service = new UtilityPaymentService(userCode);

        }

        [HttpPost, Route("post")]
        public IActionResult PostUtilityPayment(PaymentView payment)
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
    }
}
