using System;
using CB.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using CB.Services;

public class MongoRecipeService
{
     private readonly IMongoCollection<Recipe> _recipes;

        public MongoRecipeService(string connectionString = "mongodb://localhost:27017", string dbName = "recipesdb")
        {
            try
            {
                var client = new MongoClient(connectionString);
                var database = client.GetDatabase(dbName);
                _recipes = database.GetCollection<Recipe>("recipes");
                // Логируем подключение
                LogService.LogDbConnectionAsync(connectionString).Wait();
            }
            catch (Exception ex)
            {
                LogService.LogDbErrorAsync("MongoRecipeService.ctor", ex).Wait();
                throw;
            }
        }

        public async Task<List<Recipe>> GetAllAsync()
        {
            try
            {
                return await _recipes.Find(_ => true).ToListAsync();
            }
            catch (Exception ex)
            {
                await LogService.LogDbErrorAsync("GetAllAsync", ex);
                throw;
            }
        }

        public async Task AddAsync(Recipe recipe)
        {
            try
            {
                await _recipes.InsertOneAsync(recipe);
                await LogService.LogDbActionAsync("Add", recipe.Id ?? "null", recipe.Title);
            }
            catch (Exception ex)
            {
                await LogService.LogDbErrorAsync("AddAsync", ex);
                throw;
            }
        }

        public async Task UpdateAsync(string id, Recipe recipe)
        {
            try
            {
                await _recipes.ReplaceOneAsync(r => r.Id == id, recipe);
                await LogService.LogDbActionAsync("Update", id, recipe.Title);
            }
            catch (Exception ex)
            {
                await LogService.LogDbErrorAsync("UpdateAsync", ex);
                throw;
            }
        }

        public async Task DeleteAsync(string id)
        {
            try
            {
                var recipe = await _recipes.Find(r => r.Id == id).FirstOrDefaultAsync();
                string? title = recipe?.Title;
                await _recipes.DeleteOneAsync(r => r.Id == id);
                await LogService.LogDbActionAsync("Delete", id, title);
            }
            catch (Exception ex)
            {
                await LogService.LogDbErrorAsync("DeleteAsync", ex);
                throw;
            }
        }
}