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
            LogService.Init();
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(dbName);
            _recipes = database.GetCollection<Recipe>("recipes");
            LogService.LogDbConnection(connectionString);
        }
        catch (Exception ex)
        {
            LogService.LogDbError("MongoRecipeService.ctor", ex);
            throw;
        }
    }

    public async Task<List<Recipe>> GetAllAsync()
    {
        try
        {
            var list = await _recipes.Find(_ => true).ToListAsync();
            LogService.LogDbAction("GetAll");
            return list;
        }
        catch (Exception ex)
        {
            LogService.LogDbError("GetAllAsync", ex);
            throw;
        }
    }

    public async Task AddAsync(Recipe recipe)
    {
        try
        {
            await _recipes.InsertOneAsync(recipe);
            LogService.LogDbAction("Add", recipe.Id, recipe.Title);
        }
        catch (Exception ex)
        {
            LogService.LogDbError("AddAsync", ex);
            throw;
        }
    }

    public async Task UpdateAsync(string id, Recipe recipe)
    {
        try
        {
            await _recipes.ReplaceOneAsync(r => r.Id == id, recipe);
            LogService.LogDbAction("Update", id, recipe.Title);
        }
        catch (Exception ex)
        {
            LogService.LogDbError("UpdateAsync", ex);
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
            LogService.LogDbAction("Delete", id, title);
        }
        catch (Exception ex)
        {
            LogService.LogDbError("DeleteAsync", ex);
            throw;
        }
    }
}
