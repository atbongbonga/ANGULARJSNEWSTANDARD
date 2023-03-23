using Bookkeeping.Library.InternalRecon.Models;
using Bookkeeping.Library.InternalRecon.Services;
using Bookkeeping.Library.InternalRecon.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Bookkeeping.Api.Controllers
{
    [Route("api/recon/auto")]
    [ApiController]
    public class AutoReconController : ControllerBase
    {
        private readonly AutoReconService service;
        private IConfiguration configuration;

        public AutoReconController(IConfiguration configuration)
        {
            this.configuration = configuration;
            service = new AutoReconService();
        }

        [HttpGet, Route("get")]
        public ActionResult<IEnumerable<AutoReconTransactionModel>> GetTransactions(int transactionType)
        {
            try
            {
                return Ok(service.GetTransactions(transactionType));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.GetBaseException().Message);
            }
        }

        [HttpGet, Route("get_type")]
        public ActionResult<IEnumerable<AutoReconTransactionModel>> GetTypes()
        {
            try
            {
                return Ok(service.GetTypes());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.GetBaseException().Message);
            }
        }

        [HttpPost, Route("update")]
        public ActionResult Update(AutoReconTransactionModel transaction)
        {
            try
            {
                service.Update(transaction.TransactionType, transaction.GroupNumber, transaction.SyncedDate);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.GetBaseException().Message);
            }
        }

        [HttpPost, Route("log")]
        public ActionResult Log(AutoReconTransactionModel transaction)
        {
            try
            {
                service.Log(transaction);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.GetBaseException().Message);
            }
        }

    }
}
