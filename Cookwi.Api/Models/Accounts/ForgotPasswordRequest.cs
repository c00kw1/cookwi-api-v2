using System.ComponentModel.DataAnnotations;

namespace Cookwi.Api.Models.Accounts
{
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}