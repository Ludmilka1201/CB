using CB.Models;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive;
using System;
using System.Linq;

namespace CB.ViewModels;

    public class RecipeEditViewModel : ReactiveObject
    {
        public RecipeEditViewModel(Recipe? recipe = null)
        {
            if (recipe != null)
            {
                Title = recipe.Title;
                Category = recipe.Category;
                Instructions = recipe.Instructions;
                Ingredients = new ObservableCollection<IngredientViewModel>(
                    recipe.Ingredients.Select(i => new IngredientViewModel(i, RemoveIngredient)));
            }
            AddIngredientCommand = ReactiveCommand.Create(AddIngredient);
            //RemoveIngredientCommand = ReactiveCommand.Create<Ingredient>(RemoveIngredient);
        }

        string _title = "";
        public string Title { get => _title; set => this.RaiseAndSetIfChanged(ref _title, value); }

        string _category = "";
        public string Category { get => _category; set => this.RaiseAndSetIfChanged(ref _category, value); }

        string _instructions = "";
        public string Instructions { get => _instructions; set => this.RaiseAndSetIfChanged(ref _instructions, value); }

        public ObservableCollection<IngredientViewModel> Ingredients { get; } = new();

        public ReactiveCommand<Unit, Unit> AddIngredientCommand { get; }
        public ReactiveCommand<Ingredient, Unit> RemoveIngredientCommand { get; }

        private void AddIngredient()
        {
            Ingredients.Add(new IngredientViewModel(new Ingredient(), RemoveIngredient));
        }
        private void RemoveIngredient(IngredientViewModel vm)
        {
            Ingredients.Remove(vm);
        }

        public Recipe ToRecipe()
        {
            return new Recipe
            {
                Title = Title,
                Category = Category,
                Instructions = Instructions,
                Ingredients = Ingredients.Select(i => new Ingredient
                {
                    Name = i.Name,
                    Quantity = i.Quantity,
                    Unit = i.Unit
                }).ToList()
            };
        }
    }
