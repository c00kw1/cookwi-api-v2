using System.ComponentModel.DataAnnotations.Schema;
using Cookwi.Common.Models;

namespace Cookwi.Db.Entities
{
    [Table("quantityunits")]
    public class QuantityUnit : DbEntity
    {
        [Column("name")]
        public string Name { get; set; }

        [Column("acronym")]
        public string Acronym { get; set; }

        [Column("type")]
        public UnitType Type { get; set; }

        public QuantityUnit()
        {
            Name = "";
            Acronym = "";
            Type = UnitType.Other;
        }
    }
}
