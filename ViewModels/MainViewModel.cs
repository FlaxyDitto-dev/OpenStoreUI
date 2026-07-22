using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpenStoreUI.Models;
using OpenStoreUI.Services;

namespace OpenStoreUI.ViewModels;

public partial class MainViewModel : ObservableObject
{
    public WinGetService WinGetService { get; } = new();

    [ObservableProperty]
    private string _searchQuery = string.Empty;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _statusMessage = "Ready";

    public ObservableCollection<AppPackage> SearchResults { get; } = new();
    public ObservableCollection<AppPackage> InstalledPackages { get; } = new();
    public ObservableCollection<AppPackage> AvailableUpdates { get; } = new();

    [RelayCommand]
    private async Task SearchAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchQuery)) return;

        IsLoading = true;
        StatusMessage = $"Searching for '{SearchQuery}'...";
        SearchResults.Clear();

        var results = await WinGetService.SearchPackagesAsync(SearchQuery);
        foreach (var item in results) SearchResults.Add(item);

        IsLoading = false;
        StatusMessage = $"Found {SearchResults.Count} packages.";
    }

    [RelayCommand]
    public async Task LoadInstalledAsync()
    {
        IsLoading = true;
        StatusMessage = "Fetching installed applications...";
        InstalledPackages.Clear();

        var results = await WinGetService.GetInstalledPackagesAsync();
        foreach (var item in results) InstalledPackages.Add(item);

        IsLoading = false;
        StatusMessage = $"Loaded {InstalledPackages.Count} installed packages.";
    }

    [RelayCommand]
    public async Task LoadUpdatesAsync()
    {
        IsLoading = true;
        StatusMessage = "Checking for available updates...";
        AvailableUpdates.Clear();

        var results = await WinGetService.GetAvailableUpdatesAsync();
        foreach (var item in results) AvailableUpdates.Add(item);

        IsLoading = false;
        StatusMessage = $"{AvailableUpdates.Count} updates available.";
    }

    [RelayCommand]
    private async Task InstallPackageAsync(AppPackage package)
    {
        if (package == null) return;
        IsLoading = true;
        StatusMessage = $"Installing {package.Name}...";

        bool success = await WinGetService.InstallPackageAsync(package.Id);
        IsLoading = false;
        StatusMessage = success ? $"Successfully installed {package.Name}!" : $"Failed to install {package.Name}.";
    }

    [RelayCommand]
    public async Task UpdatePackageAsync(AppPackage package)
    {
        if (package == null) return;
        IsLoading = true;
        StatusMessage = $"Updating {package.Name}...";

        bool success = await WinGetService.UpdatePackageAsync(package.Id);
        IsLoading = false;
        StatusMessage = success ? $"Successfully updated {package.Name}!" : $"Failed to update {package.Name}.";
    }

    [RelayCommand]
    public async Task UninstallPackageAsync(AppPackage package)
    {
        if (package == null) return;
        IsLoading = true;
        StatusMessage = $"Uninstalling {package.Name}...";

        bool success = await WinGetService.UninstallPackageAsync(package.Id);
        IsLoading = false;
        StatusMessage = success ? $"Successfully uninstalled {package.Name}." : $"Failed to uninstall {package.Name}.";
        
        if (success) InstalledPackages.Remove(package);
    }
}
