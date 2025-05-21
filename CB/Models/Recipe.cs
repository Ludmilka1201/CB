using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CB.Models;

public class Recipe
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public string Title { get; set; } = "";
    public List<Ingredient> Ingredients { get; set; } = new();
    public string Instructions { get; set; } = "";
    public string Category { get; set; } = "";
}