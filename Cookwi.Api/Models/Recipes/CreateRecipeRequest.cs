using System;
using System.Collections.Generic;
using System.Linq;
using Cookwi.Db;
using FluentValidation;

namespace Cookwi.Api.Models.Recipes
{
    public class CreateRecipeRequest
    {
        public string Title { get; set; }
        public TimeSpan CookingTime { get; set; }
        public TimeSpan BakingTime { get; set; }
        public HashSet<string> Tags { get; set; }
        public List<RecipeStepDto> Steps { get; set; }
        public List<RecipeIngredientDto> Ingredients { get; set; }
    }

    public class CreateRecipeRequestValidator : AbstractValidator<CreateRecipeRequest>
    {
        public CreateRecipeRequestValidator(CookwiContext dbCtx)
        {
            RuleFor(c => c.Title).NotEmpty().NotNull().WithMessage("Title should not be empty");
            RuleFor(c => c.Steps).NotNull().NotEmpty().Must(s => s.Count > 0).WithMessage("At least one step should be present");
            RuleFor(c => c.Ingredients).NotNull().NotEmpty().Must(s => s.Count > 0).WithMessage("At least one ingredient should be present");
            // we check the units used exist
            RuleFor(c => c.Ingredients).Custom((ingredients, ctx) => {
                var units = dbCtx.QuantityUnits.ToList();
                ingredients.ForEach(i =>
                {
                    if (!units.Any(u => u.Acronym == i.Unit))
                    {
                        ctx.AddFailure($"Unit {i.Unit} used for ingredient {i.Name} does not exist");
                    }
                });
            });
        }
    }
}
