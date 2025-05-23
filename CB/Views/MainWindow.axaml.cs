using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using CB.Models;
using CB.ViewModels;

namespace CB.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
    }
    
   
    //Метод для обработки двойного клика по элементу списка рецептов
    private void ListBox_OnDoubleTapped(object? sender, RoutedEventArgs e)
    {
        if (sender is ListBox lb && lb.SelectedItem is Recipe recipe && DataContext is MainWindowViewModel vm)
            vm.ShowRecipeCommand.Execute(recipe).Subscribe();
    }
    
    //Метод для обработки клика по пустому месту для добавления нового рецепта
    private void EmptyArea_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        // Проверяем, что кликнули именно по пустому месту (например, по Border)
        if (DataContext is CB.ViewModels.MainWindowViewModel vm)
        {
            vm.SelectedRecipe = null;
        }
    }
}