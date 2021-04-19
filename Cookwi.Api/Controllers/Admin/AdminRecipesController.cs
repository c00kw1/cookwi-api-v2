using Cookwi.Api.Helpers;
using Cookwi.Api.Services.Admin;
using Cookwi.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cookwi.Api.Controllers.Admin
{
    [ApiController]
    [Authorize(Role.User, Role.Admin)]
    [Route("admin/recipes")]
    public class AdminRecipesController : BaseController
    {
        private IAdminRecipeService _service;

        public AdminRecipesController(IAdminRecipeService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var recipes = _service.GetAll();

            return Ok(recipes);
        }
    }
}
