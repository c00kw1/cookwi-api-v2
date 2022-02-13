using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cookwi.Db.Entities
{
    public abstract class DbEntity
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("created")]
        public DateTime Created { get; set; }

        [Column("updated")]
        public DateTime? Updated { get; set; }

        protected DbEntity()
        {
            Created = DateTime.Now.ToUniversalTime();
            Updated = null;
        }
    }
}