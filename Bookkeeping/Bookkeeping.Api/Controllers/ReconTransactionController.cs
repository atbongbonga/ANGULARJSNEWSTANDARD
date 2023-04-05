using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Bookkeeping.Library.InternalRecon.Services;
using Bookkeeping.Library.InternalRecon.ViewModels;
using Bookkeeping.Library.InternalRecon.Models;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Bookkeeping.Api.Controllers
{
    [Route("api/recon/manual")]
    [ApiController]
    public class ReconTransactionController : ControllerBase
    {
        private readonly ReconTransactionService service;
        private HttpContext httpContext;

        public ReconTransactionController(IHttpContextAccessor accessor)
        {
            httpContext = accessor.HttpContext!;
            var userCode = httpContext.User.FindFirstValue("EmpCode");
            service = new ReconTransactionService(userCode);
        }

        [HttpGet, Route("get")]
        public ActionResult<IEnumerable<ReconTransactionViewModel>> GetTransactions(string segment_0, string segment_1)
        {
            try
            {
                return Ok(service.GetTransactions(segment_0, segment_1));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.GetBaseException().Message);
            }
        }

        [HttpPost, Route("post")]
        public ActionResult InsertTransactions(IEnumerable<ReconTransactionModel> transactions)
        {
            try
            {
                return Ok(service.InsertTransations(transactions));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.GetBaseException().Message);
            }
        }

        [HttpPost, Route("update")]
        public ActionResult UpdateTransactions(int groupNumber)
        {
            try
            {
                service.UpdateTransactions(groupNumber);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.GetBaseException().Message);
            }
        }

        [HttpPost, Route("remove")]
        public ActionResult RemoveTransactions(IEnumerable<ReconTransactionModel> transactions)
        {
            try
            {
                service.RemoveTransactions(transactions);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.GetBaseException().Message);
            }
        }


    }
}
