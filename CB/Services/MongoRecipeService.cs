using CB.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

public class MongoRecipeService
{
    private readonly IMongoCollection<Recipe> _recipes;

    public MongoRecipeService(string connectionString = "mongodb://localhost:27017", string dbName = "recipesdb")
    {
        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(dbName);
        _recipes = database.GetCollection<Recipe>("recipes");
    }

    public async Task<List<Recipe>> GetAllAsync() =>
        await _recipes.Find(_ => true).ToListAsync();

    public async Task AddAsync(Recipe recipe) =>
        await _recipes.InsertOneAsync(recipe);

    public async Task UpdateAsync(string id, Recipe recipe) =>
        await _recipes.ReplaceOneAsync(r => r.Id == id, recipe);

    public async Task DeleteAsync(string id) =>
        await _recipes.DeleteOneAsync(r => r.Id == id);
}