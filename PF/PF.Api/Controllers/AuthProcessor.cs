using AccountingLegacy.PF.Library.Auth;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AccountingLegacy.PF.Api.Controllers
{
    public class AuthProcessor
    {
        private IConfiguration configuration;
        public AuthProcessor(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void CreateToken(EmployeeModel employee, out string token, out DateTime expiration)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Sid, Convert.ToString(employee.UserID)),
                new Claim(ClaimTypes.Role, employee.UType),
                new Claim(ClaimTypes.Name, employee.FullName),
                new Claim("EmpCode", employee.EmpID),
                new Claim("FName", employee.FName),
                new Claim("Position", employee.Position),
                new Claim("WhsCode", employee.WhsCode),
                new Claim("DeptCode", employee.DeptCode.ToString()),
                new Claim("Dept", employee.Dept),
                new Claim("SecCode", employee.SecCode.ToString()),
                new Claim("Section", employee.Section)
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(configuration.GetSection("AppSettings:Token").Value));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var jwt = new JwtSecurityToken(claims: claims, expires: DateTime.Now.AddHours(10), signingCredentials: credentials);
            token = new JwtSecurityTokenHandler().WriteToken(jwt);
            expiration = jwt.ValidTo;
        }
    }
}
