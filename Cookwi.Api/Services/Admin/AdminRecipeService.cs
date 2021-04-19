using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Cookwi.Api.Models.Recipes;
using Cookwi.Db;

namespace Cookwi.Api.Services.Admin
{
    public interface IAdminRecipeService
    {
        public IEnumerable<RecipeResponse> GetAll();
    }

    public class AdminRecipeService : IAdminRecipeService
    {
        private CookwiContext _ctx;
        private IMapper _mapper;

        public AdminRecipeService(CookwiContext ctx, IMapper mapper)
        {
            _ctx = ctx;
            _mapper = mapper;
        }

        public IEnumerable<RecipeResponse> GetAll()
        {
            return _mapper.Map<List<RecipeResponse>>(_ctx.Recipes.ToList());
        }
    }
}
