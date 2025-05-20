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
    
    
}