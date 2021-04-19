using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Cookwi.Common.Models;

namespace Cookwi.Api.Models.Accounts
{
    public class UpdateRequest
    {
        private string _password;
        private string _confirmPassword;
        private string[] _roles;
        private string _email;
        
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [EnumDataType(typeof(Role[]))]
        public string[] Roles
        {
            get => _roles;
            set => _roles = ReplaceArrayEmptyWithNull(value);
        }

        [EmailAddress]
        public string Email
        {
            get => _email;
            set => _email = replaceEmptyWithNull(value);
        }

        [MinLength(6)]
        public string Password
        {
            get => _password;
            set => _password = replaceEmptyWithNull(value);
        }

        [Compare("Password")]
        public string ConfirmPassword 
        {
            get => _confirmPassword;
            set => _confirmPassword = replaceEmptyWithNull(value);
        }

        // helpers

        private string replaceEmptyWithNull(string value)
        {
            // replace empty string with null to make field optional
            return string.IsNullOrEmpty(value) ? null : value;
        }

        private string[] ReplaceArrayEmptyWithNull(string[] values)
        {
            var newList = new List<string>();
            foreach (var val in values)
            {
                if (string.IsNullOrEmpty(val)) continue;
                newList.Add(val);
            }

            return newList.ToArray();
        }
    }
}