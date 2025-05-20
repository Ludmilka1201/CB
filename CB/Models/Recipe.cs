using System.Collections.Generic;

namespace CB.Models;

public class Recipe
{
    public string Title { get; set; } = "";
    public List<Ingredient> Ingredients { get; set; } = new List<Ingredient>();
    public string Instructions { get; set; } = "";
    public string Category { get; set; } = "";
}