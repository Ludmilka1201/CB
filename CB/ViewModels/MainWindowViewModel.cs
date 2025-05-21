using System;
using System.Collections.Generic;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using CB.Models;
using DynamicData.Binding;


namespace CB.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
     // Основная коллекция рецептов
        public ObservableCollection<Recipe> Recipes { get; } = new();

        // Поисковые и фильтрующие поля
        private string _searchTitle = "";
        public string SearchTitle
        {
            get => _searchTitle;
            set => this.RaiseAndSetIfChanged(ref _searchTitle, value);
        }

        private string _searchIngredient = "";
        public string SearchIngredient
        {
            get => _searchIngredient;
            set => this.RaiseAndSetIfChanged(ref _searchIngredient, value);
        }

        private string _filterCategory = "";
        public string FilterCategory
        {
            get => _filterCategory;
            set => this.RaiseAndSetIfChanged(ref _filterCategory, value);
        }

        private string _filterIngredient = "";
        public string FilterIngredient
        {
            get => _filterIngredient;
            set => this.RaiseAndSetIfChanged(ref _filterIngredient, value);
        }

        // выбранный рецепт
        private Recipe? _selectedRecipe;
        public Recipe? SelectedRecipe
        {
            get => _selectedRecipe;
            set => this.RaiseAndSetIfChanged(ref _selectedRecipe, value);
        }

        // Для редактирования/добавления
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

        // Для просмотра
        private bool _isViewDialogOpen;
        public bool IsViewDialogOpen
        {
            get => _isViewDialogOpen;
            set => this.RaiseAndSetIfChanged(ref _isViewDialogOpen, value);
        }

        private Recipe? _viewRecipe;
        public Recipe? ViewRecipe
        {
            get => _viewRecipe;
            set => this.RaiseAndSetIfChanged(ref _viewRecipe, value);
        }

        // Команды
        public ReactiveCommand<Unit, Unit> AddRecipeCommand { get; }
        public ReactiveCommand<Unit, Unit> EditRecipeCommand { get; }
        public ReactiveCommand<Unit, Unit> DeleteRecipeCommand { get; }
        public ReactiveCommand<Unit, Unit> SaveRecipeCommand { get; }
        public ReactiveCommand<Unit, Unit> CancelEditCommand { get; }
        public ReactiveCommand<Recipe, Unit> ShowRecipeCommand { get; }
        public ReactiveCommand<Unit, Unit> CloseViewDialogCommand { get; }

        // Реактивное вычисляемое свойство для фильтра
        private ObservableAsPropertyHelper<ReadOnlyObservableCollection<Recipe>>? _filteredRecipes;
        public ReadOnlyObservableCollection<Recipe> FilteredRecipes => _filteredRecipes?.Value ?? new(new ObservableCollection<Recipe>());

        public MainWindowViewModel()
        {
            // Пример данных
            Recipes.Add(new Recipe
            {
                Title = "Панкейки",
                Ingredients = new() {
                    new Ingredient { Name = "Мука", Quantity = 200, Unit = "г" },
                    new Ingredient { Name = "Молоко", Quantity = 300, Unit = "мл" },
                    new Ingredient { Name = "Яйцо", Quantity = 1, Unit = "шт" }
                },
                Instructions = "Смешайте всё и жарьте.",
                Category = "Завтрак"
            });
            Recipes.Add(new Recipe
            {
                Title = "Омлет",
                Ingredients = new() {
                    new Ingredient { Name = "Яйцо", Quantity = 3, Unit = "шт" },
                    new Ingredient { Name = "Молоко", Quantity = 50, Unit = "мл" }
                },
                Instructions = "Взбейте яйца с молоком и жарьте.",
                Category = "Завтрак"
            });

            // Реактивная фильтрация
            this.WhenAnyValue(
                x => x.SearchTitle,
                x => x.SearchIngredient,
                x => x.FilterCategory,
                x => x.FilterIngredient)
                .Throttle(TimeSpan.FromMilliseconds(100))
                .Select(_ => Unit.Default)
                .Merge(Recipes.ToObservableChangeSet().Select(_ => Unit.Default))
                .Select(_ => FilterCore())
                .ToProperty(this, x => x.FilteredRecipes, out _filteredRecipes);

            AddRecipeCommand = ReactiveCommand.Create(OnAddRecipe, this.WhenAnyValue(x => x.SelectedRecipe).Select(r => r == null));
            EditRecipeCommand = ReactiveCommand.Create(OnEditRecipe, this.WhenAnyValue(x => x.SelectedRecipe).Select(r => r != null));
            DeleteRecipeCommand = ReactiveCommand.Create(OnDeleteRecipe, this.WhenAnyValue(x => x.SelectedRecipe).Select(r => r != null));
            SaveRecipeCommand = ReactiveCommand.Create(OnSaveRecipe);
            CancelEditCommand = ReactiveCommand.Create(OnCancelEdit);
            ShowRecipeCommand = ReactiveCommand.Create<Recipe>(OnShowRecipe);
            CloseViewDialogCommand = ReactiveCommand.Create(CloseViewDialog);
        }

        // Фильтрация логика
        private ReadOnlyObservableCollection<Recipe> FilterCore()
        {
            var filtered = Recipes.Where(r =>
                (string.IsNullOrWhiteSpace(SearchTitle) || r.Title.Contains(SearchTitle, StringComparison.OrdinalIgnoreCase)) &&
                (string.IsNullOrWhiteSpace(SearchIngredient) || r.Ingredients.Any(i => i.Name.Contains(SearchIngredient, StringComparison.OrdinalIgnoreCase))) &&
                (string.IsNullOrWhiteSpace(FilterCategory) || r.Category.Contains(FilterCategory, StringComparison.OrdinalIgnoreCase)) &&
                (string.IsNullOrWhiteSpace(FilterIngredient) || r.Ingredients.Any(i => i.Name.Contains(FilterIngredient, StringComparison.OrdinalIgnoreCase)))
            );
            return new ReadOnlyObservableCollection<Recipe>(new ObservableCollection<Recipe>(filtered));
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

        private void OnShowRecipe(Recipe recipe)
        {
            ViewRecipe = recipe;
            IsViewDialogOpen = true;
        }

        private void CloseViewDialog()
        {
            IsViewDialogOpen = false;
        }
}

    
