using System;
using System.Collections.Generic;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using CB.Models;


namespace CB.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
     public ObservableCollection<Recipe> Recipes { get; } = new();

        private Recipe? _selectedRecipe;
        public Recipe? SelectedRecipe
        {
            get => _selectedRecipe;
            set => this.RaiseAndSetIfChanged(ref _selectedRecipe, value);
        }

        private bool _isEditDialogOpen;
        public bool IsEditDialogOpen
        {
            get => _isEditDialogOpen;
            set => this.RaiseAndSetIfChanged(ref _isEditDialogOpen, value);
        }

        private RecipeEditViewModel? _editViewModel;
        public RecipeEditViewModel? EditViewModel
        {
            get => _editViewModel;
            set => this.RaiseAndSetIfChanged(ref _editViewModel, value);
        }

        public ReactiveCommand<Unit, Unit> AddRecipeCommand { get; }
        public ReactiveCommand<Unit, Unit> EditRecipeCommand { get; }
        public ReactiveCommand<Unit, Unit> DeleteRecipeCommand { get; }
        public ReactiveCommand<Unit, Unit> SaveRecipeCommand { get; }
        public ReactiveCommand<Unit, Unit> CancelEditCommand { get; }

        public MainWindowViewModel()
        {
            AddRecipeCommand = ReactiveCommand.Create(OnAddRecipe);
            EditRecipeCommand = ReactiveCommand.Create(OnEditRecipe, this.WhenAnyValue(x => x.SelectedRecipe).Select(x => x != null));
            DeleteRecipeCommand = ReactiveCommand.Create(OnDeleteRecipe, this.WhenAnyValue(x => x.SelectedRecipe).Select(x => x != null));
            SaveRecipeCommand = ReactiveCommand.Create(OnSaveRecipe);
            CancelEditCommand = ReactiveCommand.Create(OnCancelEdit);

            // Пример рецепта
            Recipes.Add(new Recipe
            {
                Title = "Панкейки",
                Ingredients = new List<Ingredient>
                {
                    new Ingredient { Name = "Мука", Quantity = 200, Unit = "г" },
                    new Ingredient { Name = "Молоко", Quantity = 300, Unit = "мл" },
                    new Ingredient { Name = "Яйцо", Quantity = 1, Unit = "шт" }
                },
                Instructions = "Смешайте всё и жарьте.",
                Category = "Завтрак"
            });
        }

        private void OnAddRecipe()
        {
            EditViewModel = new RecipeEditViewModel();
            IsEditDialogOpen = true;
        }

        private void OnEditRecipe()
        {
            if (SelectedRecipe != null)
            {
                EditViewModel = new RecipeEditViewModel(SelectedRecipe);
                IsEditDialogOpen = true;
            }
        }

        private void OnDeleteRecipe()
        {
            if (SelectedRecipe != null)
            {
                Recipes.Remove(SelectedRecipe);
                SelectedRecipe = null;
            }
        }

        private void OnSaveRecipe()
        {
            if (EditViewModel != null)
            {
                var newRecipe = EditViewModel.ToRecipe();
                if (SelectedRecipe != null && Recipes.Contains(SelectedRecipe))
                {
                    int idx = Recipes.IndexOf(SelectedRecipe);
                    Recipes[idx] = newRecipe;
                }
                else
                {
                    Recipes.Add(newRecipe);
                }
                SelectedRecipe = newRecipe;
            }
            IsEditDialogOpen = false;
        }

        private void OnCancelEdit()
        {
            IsEditDialogOpen = false;
        }
    }

    
