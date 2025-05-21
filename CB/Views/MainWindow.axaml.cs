using System;
using Avalonia.Controls;
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
    
   
    
    private void ListBox_OnDoubleTapped(object? sender, Avalonia.Input.TappedEventArgs e)
    {
        if (sender is ListBox lb && lb.SelectedItem is Recipe recipe && DataContext is MainWindowViewModel vm)
            vm.ShowRecipeCommand.Execute(recipe).Subscribe();
    }
}