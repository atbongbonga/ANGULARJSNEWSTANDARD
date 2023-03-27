using AccountingLegacy.Disbursements.Api.Models;
using AccountingLegacy.Disbursements.Library.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace AccountingLegacy.Disbursements.Api.Controllers
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
        public IActionResult Login([FromBody]LoginModel login)
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

        [HttpPost("test2")]
        public IActionResult Test2()
        {
            try
            {
                return Ok(new { token = "COPS_TOKEN" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.GetBaseException().Message);
            }
        }

        [HttpGet("test")]
        public IActionResult Test()
        {
            try
            {
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.GetBaseException().Message);
            }
        }
    }
}
