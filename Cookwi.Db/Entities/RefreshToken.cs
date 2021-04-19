using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cookwi.Db.Entities
{
    [Table("refreshtokens")]
    public class RefreshToken : DbEntity
    {
        [Column("ownerid")]
        public int OwnerId { get; set; }

        [Column("token")]
        public string Token { get; set; }

        [Column("expires")]
        public DateTime Expires { get; set; }

        [Column("createdbyip")]
        public string CreatedByIp { get; set; }

        [Column("revoked")]
        public DateTime? Revoked { get; set; }

        [Column("revokedbyip")]
        public string RevokedByIp { get; set; }

        [Column("replacedbytoken")]
        public string ReplacedByToken { get; set; }

        // EF
        public Account Account { get; set; }

        [NotMapped]
        public bool IsExpired => DateTime.UtcNow >= Expires;
        [NotMapped]
        public bool IsActive => (Revoked == null || Revoked == DateTime.MinValue) && !IsExpired;

        public RefreshToken() : base()
        {
            Token = "";
            Expires = DateTime.MinValue;
            CreatedByIp = "";
            Revoked = null;
            RevokedByIp = "";
            ReplacedByToken = "";
        }
    }
}