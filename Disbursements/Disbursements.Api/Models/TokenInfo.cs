using AccountingLegacy.Disbursements.Library.Auth;

namespace Disbursements.Api.Models
{
    public class TokenInfo
    {
        public string token { get; set; }
        public DateTime expiration { get; set; }
        public EmployeeModel user { get; set; }
    }
}
