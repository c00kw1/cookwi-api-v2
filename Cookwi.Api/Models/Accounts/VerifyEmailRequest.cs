using System.ComponentModel.DataAnnotations;

namespace Cookwi.Api.Models.Accounts
{
    public class VerifyEmailRequest
    {
        [Required]
        public string Token { get; set; }
    }
}