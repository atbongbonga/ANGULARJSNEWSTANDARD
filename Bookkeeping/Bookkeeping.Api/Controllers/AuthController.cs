using Bookkeeping.Api.Models;
using Bookkeeping.Library.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Bookkeeping.Api.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService service;
        private IConfiguration configuration;

        public AuthController(IConfiguration configuration)
        {
            this.configuration = configuration;
            service = new AuthService();
        }

        [HttpPost("login")]
        public ActionResult<string> Login([FromBody] LoginModel login)
        {
            try
            {
                var employee = service.Login(login.username, login.password);
                var auth = new AuthProcessor(configuration);
                auth.CreateToken(employee, out string token, out DateTime expiration);
                return Ok(new { token = token, expiration = expiration, user = employee });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.GetBaseException().Message);
            }
        }
    }
}
