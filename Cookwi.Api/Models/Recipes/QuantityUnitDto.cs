using Cookwi.Common.Models;

namespace Cookwi.Api.Models.Recipes
{
    public class QuantityUnitDto
    {
        public string Name { get; set; }
        public string Acronym { get; set; }
        public UnitType Type { get; set; }
    }
}
