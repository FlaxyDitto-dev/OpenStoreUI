using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using OpenStoreUI.Models;
using OpenStoreUI.ViewModels;

namespace OpenStoreUI;

public sealed partial class UpdatesPage : Page
{
    public MainViewModel ViewModel => App.MainVM;

    public UpdatesPage()
    {
        this.InitializeComponent();
        _ = ViewModel.LoadUpdatesAsync();
    }

    private void Refresh_Click(object sender, RoutedEventArgs e) => _ = ViewModel.LoadUpdatesAsync();

    private void Update_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.DataContext is AppPackage package)
        {
            _ = ViewModel.UpdatePackageAsync(package);
        }
    }
}