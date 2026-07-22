using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using OpenStoreUI.Models;
using OpenStoreUI.ViewModels;

namespace OpenStoreUI;

public sealed partial class MainPage : Page
{
    public MainViewModel ViewModel { get; } = new();

    public MainPage()
    {
        this.InitializeComponent();
    }

    private void SearchBox_KeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            if (ViewModel.SearchCommand.CanExecute(null))
            {
                ViewModel.SearchCommand.Execute(null);
            }
        }
    }

    private void InstallButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.DataContext is AppPackage package)
        {
            ViewModel.InstallPackageCommand.Execute(package);
        }
    }
}