using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Bookkeeping.Library.InternalRecon.Services;
using Bookkeeping.Library.InternalRecon.ViewModels;
using Bookkeeping.Library.InternalRecon.Models;

namespace Bookkeeping.Api.Controllers
{
    [Route("api/recon")]
    [ApiController]
    public class ReconTransactionController : ControllerBase
    {
        private readonly ReconTransactionService service;
        private IConfiguration configuration;
        
        public ReconTransactionController(IConfiguration configuration)
        {
            this.configuration = configuration;
            service = new ReconTransactionService();
        }

        [HttpGet, Route("get")]
        public ActionResult<IEnumerable<ReconTransactionViewModel>> GetTransactions(int transactionType, string segment_0, string segment_1)
        {
            try
            {
                return Ok(service.GetTransactions(transactionType, segment_0, segment_1));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.GetBaseException().Message);
            }
        }

        [HttpPost, Route("post")]
        public ActionResult InsertTransations(IEnumerable<ReconTransactionModel> transactions)
        {
            try
            {
                service.InsertTransations(transactions);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.GetBaseException().Message);
            }
        }

        [HttpPost, Route("update")]
        public ActionResult UpdateTransactions(IEnumerable<ReconTransactionModel> transactions)
        {
            try
            {
                service.UpdateTransactions(transactions);
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
