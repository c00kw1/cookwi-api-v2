using System;
using System.Collections.Generic;
using Cookwi.Common.Models;
using Cookwi.Db.Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Cookwi.Db
{
    public class CookwiContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<Tribe> Tribes { get; set; }
        public DbSet<QuantityUnit> QuantityUnits { get; set; }

        public CookwiContext(DbContextOptions<CookwiContext> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            #region Recipe

            builder.Entity<Recipe>()
                .HasMany(r => r.Tribes)
                .WithMany(t => t.Recipes)
                .UsingEntity<TribeRecipe>(
                    e => e.HasOne(tr => tr.Tribe).WithMany(t => t.TribesRecipes).HasForeignKey(t => t.TribeId),
                    e => e.HasOne(tr => tr.Recipe).WithMany(r => r.TribesRecipes).HasForeignKey(r => r.RecipeId)
                );

            builder.Entity<Recipe>()
                .Property(r => r.Ingredients)
                .HasConversion(ing => JsonConvert.SerializeObject(ing), ing => JsonConvert.DeserializeObject<List<RecipeIngredient>>(ing));

            builder.Entity<Recipe>()
                .Property(r => r.Steps)
                .HasConversion(s => JsonConvert.SerializeObject(s), s => JsonConvert.DeserializeObject<List<RecipeStep>>(s));

            #endregion

            #region QuantityUnits

            builder.Entity<QuantityUnit>()
                .Property(q => q.Type)
                .HasConversion(t => t.ToString(), t => (UnitType)Enum.Parse(typeof(UnitType), t));

            #endregion

            #region Account

            builder.Entity<Account>()
                .HasMany(a => a.RefreshTokens)
                .WithOne(r => r.Account)
                .HasForeignKey(r => r.OwnerId);

            builder.Entity<Account>()
                .HasMany(a => a.TribeMembers)
                .WithOne(tm => tm.Account)
                .HasForeignKey(tm => tm.AccountId);

            builder.Entity<Account>()
                .Property(a => a.Gender)
                .HasConversion(g => g.ToString(), g => (Gender)Enum.Parse(typeof(Gender), g));

            builder.Entity<Account>()
                .Property(a => a.Roles)
                .HasPostgresArrayConversion(g => g.ToString(), g => (Role)Enum.Parse(typeof(Role), g));

            #endregion

            #region Tribe

            builder.Entity<Tribe>()
                .HasMany(t => t.Recipes)
                .WithMany(r => r.Tribes)
                .UsingEntity<TribeRecipe>(
                    e => e.HasOne(tr => tr.Recipe).WithMany(r => r.TribesRecipes).HasForeignKey(tr => tr.RecipeId),
                    e => e.HasOne(tr => tr.Tribe).WithMany(t => t.TribesRecipes).HasForeignKey(tr => tr.TribeId)
                );

            builder.Entity<Tribe>()
                .HasMany(t => t.Members)
                .WithMany(r => r.Tribes)
                .UsingEntity<TribeMember>(
                    e => e.HasOne(tm => tm.Account).WithMany(r => r.TribeMembers).HasForeignKey(tm => tm.AccountId),
                    e => e.HasOne(tm => tm.Tribe).WithMany(t => t.TribesMembers).HasForeignKey(tm => tm.TribeId)
                );

            builder.Entity<TribeMember>()
                .Property(tm => tm.Access)
                .HasConversion(a => a.ToString(), a => (TribeAccess)Enum.Parse(typeof(TribeAccess), a));

            #endregion
        }
    }
}
