using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Amazon.S3;
using Amazon.S3.Transfer;
using AutoMapper;
using Cookwi.Api.Helpers;
using Cookwi.Api.Models.Recipes;
using Cookwi.Common.Exceptions;
using Cookwi.Db;
using Cookwi.Db.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Cookwi.Api.Services
{
    public interface IRecipeService
    {
        IEnumerable<RecipeResponse> GetAll(int userId);
        RecipeResponse GetOne(int recipeId, int userId);
        RecipeResponse Create(CreateRecipeRequest recipe, int userId);
        RecipeResponse Update(int recipeId, CreateRecipeRequest recipe, int userId);
        bool Exists(int recipeId, int userId);
        void UpdateCover(int recipeId, int userId, IFormFile file);
        void Delete(int recipeId, int userId);
        IEnumerable<string> GetAllTags(int userId);
    }

    public class RecipeService : IRecipeService
    {
        private readonly IMapper _mapper;
        private readonly IAmazonS3 _s3;
        private readonly TransferUtility _transferUtility;
        private readonly AppSettings _appSettings;
        private readonly CookwiContext _ctx;
        private static readonly string[] _authorizedImageFormat = { "image/png", "image/jpeg" };
        private const int _minImageWidth = 800;
        private const int _minImageHeight = 600;
        private static readonly double _maxImageWeight = 3.Mb();

        public RecipeService(IMapper mapper, CookwiContext ctx, IAmazonS3 s3, IOptions<AppSettings> appSettings)
        {
            _mapper = mapper;
            _s3 = s3;
            _transferUtility = new TransferUtility(_s3);
            _appSettings = appSettings.Value;
            _ctx = ctx;
        }

        public IEnumerable<RecipeResponse> GetAll(int userId)
        {
            var recipes = _ctx.Recipes.Where(r => r.OwnerId == userId).ToList();

            return _mapper.Map<IEnumerable<RecipeResponse>>(recipes);
        }

        public RecipeResponse GetOne(int recipeId, int userId)
        {
            var recipe = _ctx.Recipes.Where(r => r.Id == recipeId && r.OwnerId == userId).FirstOrDefault();
            if (recipe == null)
            {
                throw new NotFoundException("Recipe not found");
            }

            return _mapper.Map<RecipeResponse>(recipe);
        }

        public RecipeResponse Create(CreateRecipeRequest recipe, int ownerId)
        {
            var validatorResult = new CreateRecipeRequestValidator(_ctx).Validate(recipe);
            if (!validatorResult.IsValid)
            {
                throw new BadRequestException(validatorResult.ToHttpError("Bad recipe format"));
            }
            var entity = _mapper.Map<Recipe>(recipe);

            entity.OwnerId = ownerId;
            var createdEntity = _ctx.Recipes.Add(entity);
            _ctx.SaveChanges();

            return _mapper.Map<RecipeResponse>(createdEntity.Entity);
        }

        public RecipeResponse Update(int recipeId, CreateRecipeRequest recipe, int userId)
        {
            var validatorResult = new CreateRecipeRequestValidator(_ctx).Validate(recipe);
            if (!validatorResult.IsValid)
            {
                throw new BadRequestException(validatorResult.ToHttpError("Bad recipe format"));
            }

            var recipeEntity = _ctx.Recipes.FirstOrDefault(r => r.Id == recipeId);
            if (recipe == null)
            {
                throw new NotFoundException($"Recipe ${recipeId} not found");
            }

            _ctx.Update(recipeEntity);
            _mapper.Map(recipe, recipeEntity);
            _ctx.SaveChanges();

            return _mapper.Map<RecipeResponse>(recipeEntity);
        }

        public void UpdateCover(int recipeId, int userId, IFormFile file)
        {
            if (!_authorizedImageFormat.Contains(file.ContentType.ToLower()))
            {
                throw new BadRequestException($"Bad image format, authorized are {string.Join(", ", _authorizedImageFormat)}");
            }

            var recipe = _ctx.Recipes.FirstOrDefault(r => r.Id == recipeId);
            if (recipe == null)
            {
                throw new NotFoundException($"Recipe ${recipeId} not found");
            }

            using (var image = Image.FromStream(file.OpenReadStream()))
            {
                if (image.Width < _minImageWidth || image.Height < _minImageHeight || file.Length > _maxImageWeight)
                {
                    throw new BadRequestException($"Image must be at least {_minImageWidth}x{_minImageHeight} pixels and less than {_maxImageWeight} o");
                }
            }

            using (var stream = file.OpenReadStream())
            {
                var imageInternalPathS3 = $"recipes/{recipeId}/cover{Path.GetExtension(file.FileName)}";
                _transferUtility.Upload(new TransferUtilityUploadRequest
                {
                    BucketName = _appSettings.S3.Bucket,
                    ContentType = file.ContentType,
                    Key = imageInternalPathS3,
                    InputStream = stream
                });
                
                var url = $"https://{_appSettings.S3.Bucket}.s3.{_appSettings.S3.Region}.scw.cloud/{_appSettings.S3.Bucket}/{imageInternalPathS3}";
                _ctx.Recipes.Update(recipe);
                recipe.ImagePath = url;
                _ctx.SaveChanges();
            }
        }

        public void Delete(int recipeId, int userId)
        {
            var recipe = _ctx.Recipes.FirstOrDefault(r => r.Id == recipeId && r.OwnerId == userId);
            if (recipe == null)
            {
                throw new NotFoundException($"Recipe ${recipeId} not found");
            }

            _ctx.Recipes.Remove(recipe);
            _ctx.SaveChanges();
        }

        public bool Exists(int recipeId, int userId)
        {
            return _ctx.Recipes.Any(r => r.Id == recipeId && r.OwnerId == userId);
        }

        public IEnumerable<string> GetAllTags(int userId)
        {
            return _ctx.Recipes.SelectMany(r => r.Tags).ToHashSet();
        }
    }
}
