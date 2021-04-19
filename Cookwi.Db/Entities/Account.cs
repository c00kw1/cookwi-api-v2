using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Cookwi.Common.Models;

namespace Cookwi.Db.Entities
{
    [Table("accounts")]
    public class Account : DbEntity
    {
        [Column("gender")]
        public Gender Gender { get; set; }

        [Column("firstname")]
        public string FirstName { get; set; }

        [Column("lastname")]
        public string LastName { get; set; }

        [Column("email")]
        public string Email { get; set; }

        [Column("passwordhash")]
        public string PasswordHash { get; set; }

        [Column("acceptterms")]
        public bool AcceptTerms { get; set; }

        [Column("roles")]
        public List<Role> Roles { get; set; }

        [Column("verificationtoken")]
        public string VerificationToken { get; set; }

        [Column("verified")]
        public DateTime? Verified { get; set; }

        [Column("resettoken")]
        public string ResetToken { get; set; }

        [Column("resettokenexpires")]
        public DateTime? ResetTokenExpires { get; set; }

        [Column("passwordreset")]
        public DateTime? PasswordReset { get; set; }

        // EF
        public ICollection<RefreshToken> RefreshTokens { get; set; }
        public ICollection<Tribe> Tribes { get; set; }

        public List<TribeMember> TribeMembers { get; set; }

        [NotMapped]
        public bool IsVerified => Verified.HasValue || PasswordReset.HasValue;

        public Account() : base()
        {
            Gender = Gender.Neutral;
            FirstName = "";
            LastName = "";
            Email = "";
            PasswordHash = "";
            AcceptTerms = false;
            Roles = new List<Role> { Role.User };
            VerificationToken = "";
            Verified = null;
            ResetToken = "";
            ResetTokenExpires = null;
            PasswordReset = null;
        }

        public bool OwnsToken(string token)
        {
            return RefreshTokens.Any(r => r.Token == token);
        }
    }
}