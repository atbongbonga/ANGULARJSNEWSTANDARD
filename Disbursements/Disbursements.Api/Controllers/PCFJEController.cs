﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Disbursements.Library.PCF.Services;
using Disbursements.Library.PCF.ViewModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Disbursements.Api.Controllers
{
    [Route("api/pcfje"), Authorize]
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

        [HttpPost("post")]
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
