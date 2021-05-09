using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cookwi.Db.Entities
{
    [Table("recipes")]
    public class Recipe : DbEntity
    {
        [Column("ownerid")]
        public int OwnerId { get; set; }

        [Column("title")]
        public string Title { get; set; }

        [Column("imagepath")]
        public string ImagePath { get; set; }

        [Column("cookingtime")]
        public TimeSpan CookingTime { get; set; }

        [Column("bakingtime")]
        public TimeSpan BakingTime { get; set; }

        [Column("tags")]
        public List<string> Tags { get; set; }

        [Column("steps", TypeName = "json")]
        public List<RecipeStep> Steps { get; set; }

        [Column("ingredients", TypeName = "json")]
        public List<RecipeIngredient> Ingredients { get; set; }

        // EF
        public ICollection<Tribe> Tribes { get; set; }
        public List<TribeRecipe> TribesRecipes { get; set; }

        public Recipe() : base()
        {
            Title = "";
            ImagePath = "";
            CookingTime = TimeSpan.Zero;
            BakingTime = TimeSpan.Zero;
            Tags = new List<string>();
            Steps = new List<RecipeStep>();
            Ingredients = new List<RecipeIngredient>();
        }
    }

    public class RecipeStep
    {
        public int Position { get; set; }
        public string Content { get; set; }

        public RecipeStep()
        {
            Position = 0;
            Content = "";
        }
    }

    public class RecipeIngredient
    {
        public string Name { get; set; }
        public double Quantity { get; set; }
        public string Unit { get; set; }

        public RecipeIngredient()
        {
            Name = "";
            Quantity = 0;
            Unit = "";
        }
    }
}
