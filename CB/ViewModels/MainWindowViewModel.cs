using System;
using System.Collections.Generic;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using CB.Models;
using DynamicData.Binding;


namespace CB.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
       private readonly MongoRecipeService _dbService = new();

        public ObservableCollection<Recipe> Recipes { get; } = new();

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

        public ReactiveCommand<Unit, Unit> AddRecipeCommand { get; }
        public ReactiveCommand<Unit, Unit> EditRecipeCommand { get; }
        public ReactiveCommand<Unit, Unit> DeleteRecipeCommand { get; }
        public ReactiveCommand<Unit, Unit> SaveRecipeCommand { get; }
        public ReactiveCommand<Unit, Unit> CancelEditCommand { get; }
        public ReactiveCommand<Recipe, Unit> ShowRecipeCommand { get; }
        public ReactiveCommand<Unit, Unit> CloseViewDialogCommand { get; }

        private ObservableAsPropertyHelper<ReadOnlyObservableCollection<Recipe>>? _filteredRecipes;
        public ReadOnlyObservableCollection<Recipe> FilteredRecipes => _filteredRecipes?.Value ?? new(new ObservableCollection<Recipe>());

        public MainWindowViewModel()
        {
            this.WhenAnyValue(
                x => x.SearchTitle,
                x => x.SearchIngredient,
                x => x.FilterCategory,
                x => x.FilterIngredient)
                .Throttle(System.TimeSpan.FromMilliseconds(100))
                .Select(_ => Unit.Default)
                .Merge(Recipes.ToObservableChangeSet().Select(_ => Unit.Default))
                .Select(_ => FilterCore())
                .ToProperty(this, x => x.FilteredRecipes, out _filteredRecipes);

            AddRecipeCommand = ReactiveCommand.Create(OnAddRecipe, this.WhenAnyValue(x => x.SelectedRecipe).Select(r => r == null));
            EditRecipeCommand = ReactiveCommand.Create(OnEditRecipe, this.WhenAnyValue(x => x.SelectedRecipe).Select(r => r != null));
            DeleteRecipeCommand = ReactiveCommand.Create(OnDeleteRecipe, this.WhenAnyValue(x => x.SelectedRecipe).Select(r => r != null));
            SaveRecipeCommand = ReactiveCommand.CreateFromTask(OnSaveRecipeAsync);
            CancelEditCommand = ReactiveCommand.Create(OnCancelEdit);
            ShowRecipeCommand = ReactiveCommand.Create<Recipe>(OnShowRecipe);
            CloseViewDialogCommand = ReactiveCommand.Create(CloseViewDialog);

            // Загрузка из базы автоматически при запуске
            _ = LoadRecipesAsync();
        }

        private ReadOnlyObservableCollection<Recipe> FilterCore()
        {
            var filtered = Recipes.Where(r =>
                (string.IsNullOrWhiteSpace(SearchTitle) || r.Title.Contains(SearchTitle, System.StringComparison.OrdinalIgnoreCase)) &&
                (string.IsNullOrWhiteSpace(SearchIngredient) || r.Ingredients.Any(i => i.Name.Contains(SearchIngredient, System.StringComparison.OrdinalIgnoreCase))) &&
                (string.IsNullOrWhiteSpace(FilterCategory) || r.Category.Contains(FilterCategory, System.StringComparison.OrdinalIgnoreCase)) &&
                (string.IsNullOrWhiteSpace(FilterIngredient) || r.Ingredients.Any(i => i.Name.Contains(FilterIngredient, System.StringComparison.OrdinalIgnoreCase)))
            );
            return new ReadOnlyObservableCollection<Recipe>(new ObservableCollection<Recipe>(filtered));
        }

        public async Task LoadRecipesAsync()
        {
            var all = await _dbService.GetAllAsync();
            Recipes.Clear();
            foreach (var r in all)
                Recipes.Add(r);
        }

        private void OnAddRecipe()
        {
            SelectedRecipe = null;
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

        private async Task OnSaveRecipeAsync()
        {
            if (EditViewModel == null) return;

            var newRecipe = EditViewModel.ToRecipe();

            if (SelectedRecipe != null && Recipes.Contains(SelectedRecipe))
            {
                // редактирование
                newRecipe.Id = SelectedRecipe.Id;
                await _dbService.UpdateAsync(newRecipe.Id!, newRecipe);
                int idx = Recipes.IndexOf(SelectedRecipe);
                Recipes[idx] = newRecipe;
            }
            else
            {
                // добавление
                await _dbService.AddAsync(newRecipe);
                Recipes.Add(newRecipe);
            }

            SelectedRecipe = newRecipe;
            IsEditDialogOpen = false;
        }

        private void OnCancelEdit()
        {
            IsEditDialogOpen = false;
        }

        private async void OnDeleteRecipe()
        {
            if (SelectedRecipe != null)
            {
                await _dbService.DeleteAsync(SelectedRecipe.Id!);
                Recipes.Remove(SelectedRecipe);
                SelectedRecipe = null;
            }
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


    
