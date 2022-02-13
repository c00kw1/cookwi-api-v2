using AutoMapper;
using Cookwi.Api.Models.Accounts;
using Cookwi.Api.Models.Recipes;
using Cookwi.Api.Models.Tribes;
using Cookwi.Db.Entities;

namespace Cookwi.Api.Helpers
{
    public class AutoMapperProfile : Profile
    {
        // mappings between model and entity objects
        public AutoMapperProfile()
        {
            #region Account

            CreateMap<Account, AccountResponse>();
            CreateMap<Account, AuthenticateResponse>();
            CreateMap<RegisterRequest, Account>();
            CreateMap<CreateAccountRequest, Account>();
            CreateMap<UpdateRequest, Account>()
                .ForAllMembers(x => x.Condition(
                    (src, dest, prop) =>
                    {
                        // ignore null & empty string properties
                        if (prop == null) return false;
                        if (prop.GetType() == typeof(string) && string.IsNullOrEmpty((string)prop)) return false;

                        // ignore null role
                        if (x.DestinationMember.Name == "Role" && src.Roles == null) return false;

                        return true;
                    }
                ));

            #endregion

            #region Recipe

            CreateMap<CreateRecipeRequest, Recipe>();
            CreateMap<Recipe, RecipeResponse>();

            // both ways simple objects
            CreateMap<RecipeStepDto, RecipeStep>();
            CreateMap<RecipeStep, RecipeStepDto>();
            CreateMap<RecipeIngredientDto, RecipeIngredient>();
            CreateMap<RecipeIngredient, RecipeIngredientDto>();
            CreateMap<QuantityUnitDto, QuantityUnit>();
            CreateMap<QuantityUnit, QuantityUnitDto>();

            #endregion

            #region Tribes

            CreateMap<Tribe, TribeResponse>();
            CreateMap<TribeResponse, Tribe>();

            CreateMap<Account, TribeMember>();
            CreateMap<TribeMember, Account>();

            #endregion
        }
    }
}