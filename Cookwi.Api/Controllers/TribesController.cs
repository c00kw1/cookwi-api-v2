using Cookwi.Api.Helpers;
using Cookwi.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cookwi.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("tribes")]
    public class TribesController : BaseController
    {
        private ITribeService _service;

        public TribesController(ITribeService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult GetAllTribes()
        {
            var tribes = _service.GetAll(Account.Id);

            return Ok(tribes);
        }
    }
}
