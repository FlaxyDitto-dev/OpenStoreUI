namespace OpenStoreUI.Models;

public class AppPackage
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string AvailableVersion { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;

    // Helper formatted properties for UI binding
    public string InstalledVersionDisplay => $"Installed: {Version}";
    public string NewVersionDisplay => $"New: {AvailableVersion}";
}
