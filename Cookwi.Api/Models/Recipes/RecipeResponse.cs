using System;
using System.Collections.Generic;

namespace Cookwi.Api.Models.Recipes
{
    public class RecipeResponse
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string ImagePath { get; set; }
        public TimeSpan CookingTime { get; set; }
        public TimeSpan BakingTime { get; set; }
        public HashSet<string> Tags { get; set; }
        public List<RecipeStepDto> Steps { get; set; }
        public List<RecipeIngredientDto> Ingredients { get; set; }
    }

    public class RecipeStepDto
    {
        public int Position { get; set; }
        public string Content { get; set; }
    }

    public class RecipeIngredientDto
    {
        public string Name { get; set; }
        public double Quantity { get; set; }
        public string Unit { get; set; }
    }
}
