﻿using System.ComponentModel.DataAnnotations;

namespace Cookwi.Api.Models.Accounts
{
    public class ValidateResetTokenRequest
    {
        [Required]
        public string Token { get; set; }
    }
}