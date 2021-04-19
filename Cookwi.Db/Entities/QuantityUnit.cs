using System.ComponentModel.DataAnnotations.Schema;
using Cookwi.Common.Models;

namespace Cookwi.Db.Entities
{
    [Table("quantityunits")]
    public class QuantityUnit : DbEntity
    {
        public string Name { get; set; }
        public string Acronym { get; set; }
        public UnitType Type { get; set; }

        public QuantityUnit()
        {
            Name = "";
            Acronym = "";
            Type = UnitType.Other;
        }
    }
}
