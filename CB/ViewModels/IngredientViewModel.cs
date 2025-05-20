using System;
using CB.Models;
using ReactiveUI;
using System.Reactive;

namespace CB.ViewModels;

    public class IngredientViewModel : ReactiveObject
    {
        public Ingredient Ingredient { get; }

        public ReactiveCommand<Unit, Unit> RemoveCommand { get; }

        public IngredientViewModel(Ingredient ingredient, Action<IngredientViewModel> removeAction)
        {
            Ingredient = ingredient;
            RemoveCommand = ReactiveCommand.Create(() => removeAction(this));
            _name = ingredient.Name;
            _quantity = ingredient.Quantity;
            _unit = ingredient.Unit;
        }

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                this.RaiseAndSetIfChanged(ref _name, value);
                Ingredient.Name = value;
            }
        }

        private double _quantity;
        public double Quantity
        {
            get => _quantity;
            set
            {
                this.RaiseAndSetIfChanged(ref _quantity, value);
                Ingredient.Quantity = value;
            }
        }

        private string _unit;
        public string Unit
        {
            get => _unit;
            set
            {
                this.RaiseAndSetIfChanged(ref _unit, value);
                Ingredient.Unit = value;
            }
        }
    }
