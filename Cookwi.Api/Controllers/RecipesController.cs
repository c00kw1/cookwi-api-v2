using Cookwi.Api.Helpers;
using Cookwi.Api.Models.Recipes;
using Cookwi.Api.Services;
using Cookwi.Common.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Cookwi.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("recipes")]
    public class RecipesController : BaseController
    {
        private IRecipeService _service;
        private IQuantityUnitService _unitService;

        public RecipesController(IRecipeService service, IQuantityUnitService unitService)
        {
            _service = service;
            _unitService = unitService;
        }

        /// <summary>
        /// Retrieve all recipes for current user
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [SwaggerOperation(Summary = "Retrieve all the recipes for a user")]
        [SwaggerResponse(200, "The recipes", typeof(RecipeResponse[]))]
        [SwaggerResponse(404, "Resource not found")]
        [SwaggerResponse(500, "Internal server error")]
        public IActionResult GetAllRecipes()
        {
            var recipes = _service.GetAll(Account.Id);

            return Ok(recipes);
        }

        /// <summary>
        /// Retrieve a recipe from its ID
        /// </summary>
        /// <returns>One Recipe</returns>
        [HttpGet]
        [Route("{recipeId}")]
        [SwaggerOperation(Summary = "Retrieve one recipe by its ID")]
        [SwaggerResponse(200, "Asked recipe", typeof(RecipeResponse))]
        [SwaggerResponse(400, "Bad id format")]
        [SwaggerResponse(404, "Recipe not found")]
        [SwaggerResponse(500, "Internal server error")]
        public IActionResult GetOneRecipe(int recipeId)
        {
            var recipe = _service.GetOne(recipeId, Account.Id);

            return Ok(recipe);
        }

        /// <summary>
        /// Add a new recipe
        /// </summary>
        /// <param name="recipe">Recipe to insert</param>
        /// <returns>Inserted recipe</returns>
        [HttpPost]
        [SwaggerOperation(Summary = "Add a new recipe")]
        [SwaggerResponse(201, "Recipe object created and inserted in db", typeof(RecipeResponse))]
        [SwaggerResponse(400, "At least one of recipe's field is wrong", typeof(string))]
        [SwaggerResponse(500, "Cannot add the new recipe as an error occured")]
        public IActionResult CreateRecipe([FromBody] CreateRecipeRequest recipe)
        {
            var created = _service.Create(recipe, Account.Id);

            return StatusCode(201, created);
        }

        /// <summary>
        /// Update an existing recipe
        /// </summary>
        /// <param name="id">Recipe id</param>
        /// <param name="updatedRecipe">Updated recipe</param>
        /// <returns>Inserted recipe</returns>
        [HttpPut]
        [Route("{recipeId}")]
        [SwaggerOperation(Summary = "Update an existing recipe")]
        [SwaggerResponse(200, "Recipe object updated in db", typeof(RecipeResponse))]
        [SwaggerResponse(400, "At least one of recipe's field is wrong", typeof(string[]))]
        [SwaggerResponse(404, "Recipe to update is not found / does not exist")]
        [SwaggerResponse(500, "Cannot update the recipe as an error occured")]
        public IActionResult UpdateRecipe(int recipeId, [FromBody] CreateRecipeRequest updatedRecipe)
        {
            if (!_service.Exists(recipeId, Account.Id))
            {
                throw new NotFoundException($"Recipe {recipeId} does not exist");
            }

            var result = _service.Update(recipeId, updatedRecipe, Account.Id);

            return Ok(result);
        }

        /// <summary>
        /// Delete a recipe
        /// </summary>
        /// <param name="recipeId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{recipeId}")]
        [SwaggerOperation(Summary = "Removes an existing recipe")]
        [SwaggerResponse(200, "Recipe object removed from db")]
        [SwaggerResponse(404, "Recipe to remove is not found / does not exist")]
        [SwaggerResponse(500, "Cannot remove the recipe as an error occured")]
        public IActionResult DeleteRecipe(int recipeId)
        {
            _service.Delete(recipeId, Account.Id);

            return NoContent();
        }

        /// <summary>
        /// Add/update an image cover for a recipe
        /// </summary>
        /// <param name="id"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("{id}/image")]
        [RequestSizeLimit(10_485_760)] // 10 Mb
        [SwaggerOperation("Adds an image to a recipe")]
        [SwaggerResponse(200, "Image added")]
        [SwaggerResponse(400, "Image format is wrong")]
        [SwaggerResponse(404, "Recipe not found")]
        [SwaggerResponse(413, "File too large")]
        [SwaggerResponse(500, "An error occured with the server (s3 ?)")]
        public IActionResult AddImage(int id, IFormFile file)
        {
            if (file == null)
            {
                throw new BadRequestException("No image sent");
            }

            _service.UpdateCover(id, Account.Id, file);

            return NoContent();
        }

        #region Ingredients

        /// <summary>
        /// Retrieve all ingredient quantity units available
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("quantity-units")]
        [SwaggerOperation(Summary = "Get all the available quantity units")]
        [SwaggerResponse(200, "The units", typeof(QuantityUnitDto[]))]
        [SwaggerResponse(500, "An error has occured")]
        public IActionResult GetAll()
        {
            var units = _unitService.GetAll();

            return Ok(units);
        }

        #endregion

        #region Tags

        /// <summary>
        /// Get all tags from user's recipes
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("tags")]
        [SwaggerOperation("Retrieve all tags used by current user's recipes")]
        [SwaggerResponse(200, "All tags", typeof(string[]))]
        [SwaggerResponse(500, "Unexpected error occured")]
        public IActionResult GetAllTags()
        {
            var tags = _service.GetAllTags(Account.Id);

            return Ok(tags);
        }

        #endregion
    }
}