using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Cookwi.Common.Models;

namespace Cookwi.Db.Entities
{
    [Table("tribes")]
    public class Tribe : DbEntity
    {
        [Column("ownerid")]
        public int OwnerId { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("imagepath")]
        public string ImagePath { get; set; }

        // EF
        public ICollection<Account> Members { get; set; }
        public ICollection<Recipe> Recipes { get; set; }

        public List<TribeMember> TribesMembers { get; set; }
        public List<TribeRecipe> TribesRecipes { get; set; }

        public Tribe()
        {
            Name = "";
            Description = "";
            ImagePath = "";
        }
    }

    [Table("tribesmembers")]
    public class TribeMember : DbEntity
    {
        [Column("tribeid")]
        public int TribeId { get; set; }

        [Column("accountid")]
        public int AccountId { get; set; }

        [Column("access")]
        public TribeAccess Access { get; set; }

        // EF
        public Tribe Tribe { get; set; }
        public Account Account { get; set; }

        public TribeMember()
        {
            Access = TribeAccess.Read;
        }
    }

    [Table("tribesrecipes")]
    public class TribeRecipe : DbEntity
    {
        [Column("tribeid")]
        public int TribeId { get; set; }

        [Column("recipeid")]
        public int RecipeId { get; set; }

        // EF
        public Tribe Tribe { get; set; }
        public Recipe Recipe { get; set; }
    }
}
