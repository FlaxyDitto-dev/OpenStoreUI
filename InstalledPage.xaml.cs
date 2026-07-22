using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using OpenStoreUI.Models;
using OpenStoreUI.ViewModels;

namespace OpenStoreUI;

public sealed partial class InstalledPage : Page
{
    public MainViewModel ViewModel => App.MainVM;

    public InstalledPage()
    {
        this.InitializeComponent();
        _ = ViewModel.LoadInstalledAsync();
    }

    private void Refresh_Click(object sender, RoutedEventArgs e) => _ = ViewModel.LoadInstalledAsync();

    private void Uninstall_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.DataContext is AppPackage package)
        {
            _ = ViewModel.UninstallPackageAsync(package);
        }
    }
}