using System;
using System.ComponentModel.DataAnnotations;
using Cookwi.Common.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Cookwi.Api.Models.Accounts
{
    public class AuthenticateResponse
    {
        public string Id { get; set; }
        [EnumDataType(typeof(Gender))]
        [JsonConverter(typeof(StringEnumConverter))]
        public Gender Gender { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        [EnumDataType(typeof(Role))]
        [JsonConverter(typeof(StringEnumConverter))]
        public Role Role { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public bool IsVerified { get; set; }
        public string JwtToken { get; set; }

        [JsonIgnore] // refresh token is returned in http only cookie
        public string RefreshToken { get; set; }
    }
}