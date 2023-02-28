using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AccountingLegacy.Disbursements.Library.PCF.ViewModels;
using AccountingLegacy.Disbursements.Library.PCF.Services;

namespace Disbursements.Api.Controllers.PCF
{
    [Route("api/pcfop")]
    [ApiController]
    public class PCFOPController : ControllerBase
    {

        public ActionResult PostPCFOP([FromBody] PCFUserInputView model) {
            try
            {
                PCFOPService service = new PCFOPService();
                return Ok(service.PostPCFOP(model));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.GetBaseException().Message);
            }
        }
    }

}
