using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OpenStoreUI.Models;

namespace OpenStoreUI.Services;

public class WinGetService
{
    public string SelectedSource { get; set; } = "all";

    private async Task<string> RunWinGetAsync(string arguments, bool useShellExecute = false)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "winget",
            Arguments = arguments,
            RedirectStandardOutput = !useShellExecute,
            RedirectStandardError = !useShellExecute,
            UseShellExecute = useShellExecute,
            CreateNoWindow = !useShellExecute,
            StandardOutputEncoding = useShellExecute ? null : System.Text.Encoding.UTF8
        };

        using var process = new Process { StartInfo = startInfo };
        process.Start();

        string output = "";
        if (!useShellExecute)
        {
            output = await process.StandardOutput.ReadToEndAsync();
        }

        await process.WaitForExitAsync();

        return Regex.Replace(output, @"[\u0008\u001b\u2580-\u259F\u25A0-\u25FF\r]", "");
    }

    public async Task<List<AppPackage>> SearchPackagesAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query)) return new List<AppPackage>();

        string sourceFlag = SelectedSource == "all" ? "" : $"--source {SelectedSource}";
        string rawOutput = await RunWinGetAsync($"search \"{query}\" {sourceFlag} --accept-source-agreements");
        
        return ParsePackageList(rawOutput);
    }

    public async Task<List<AppPackage>> GetInstalledPackagesAsync()
    {
        string rawOutput = await RunWinGetAsync("list --accept-source-agreements");
        return ParsePackageList(rawOutput);
    }

    public async Task<List<AppPackage>> GetAvailableUpdatesAsync()
    {
        string rawOutput = await RunWinGetAsync("list --upgrade-available --include-unknown --accept-source-agreements");
        return ParsePackageList(rawOutput, isUpgradeList: true);
    }

    public async Task<bool> InstallPackageAsync(string packageId)
    {
        // Try interactive execution without hiding installer prompts
        string args = $"install --id \"{packageId}\" --exact --accept-package-agreements --accept-source-agreements";
        await RunWinGetAsync(args, useShellExecute: true);
        return true;
    }

    public async Task<bool> UpdatePackageAsync(string packageId)
    {
        // UseShellExecute opens elevation prompt if needed
        string args = $"upgrade --id \"{packageId}\" --exact --accept-package-agreements --accept-source-agreements";
        await RunWinGetAsync(args, useShellExecute: true);
        return true;
    }

    public async Task<bool> UninstallPackageAsync(string packageId)
    {
        // Allows uninstall prompts to pop up if installer requires user action
        string args = $"uninstall --id \"{packageId}\" --exact";
        await RunWinGetAsync(args, useShellExecute: true);
        return true;
    }

    private List<AppPackage> ParsePackageList(string rawOutput, bool isUpgradeList = false)
    {
        var packages = new List<AppPackage>();
        string[] lines = rawOutput.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        int headerIndex = -1;
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].Contains("---"))
            {
                headerIndex = i;
                break;
            }
        }

        if (headerIndex <= 0 || headerIndex >= lines.Length - 1) return packages;

        string headerLine = lines[headerIndex - 1];

        int nameIdx = headerLine.IndexOf("Name", StringComparison.OrdinalIgnoreCase);
        int idIdx = headerLine.IndexOf("Id", StringComparison.OrdinalIgnoreCase);
        int versionIdx = headerLine.IndexOf("Version", StringComparison.OrdinalIgnoreCase);
        int matchAvailableIdx = isUpgradeList 
            ? headerLine.IndexOf("Available", StringComparison.OrdinalIgnoreCase)
            : headerLine.IndexOf("Match", StringComparison.OrdinalIgnoreCase);
        int sourceIdx = headerLine.IndexOf("Source", StringComparison.OrdinalIgnoreCase);

        if (nameIdx == -1) nameIdx = 0;
        if (idIdx == -1) idIdx = 30;
        if (versionIdx == -1) versionIdx = 60;

        for (int i = headerIndex + 1; i < lines.Length; i++)
        {
            string line = lines[i];
            if (string.IsNullOrWhiteSpace(line) || line.Length <= idIdx) continue;

            try
            {
                string name = GetSubstringSafe(line, nameIdx, idIdx - nameIdx).Trim();
                string id = GetSubstringSafe(line, idIdx, (versionIdx > idIdx ? versionIdx : line.Length) - idIdx).Trim();
                
                string version = "";
                if (versionIdx != -1 && versionIdx < line.Length)
                {
                    int endVer = (matchAvailableIdx > versionIdx) ? matchAvailableIdx : line.Length;
                    version = GetSubstringSafe(line, versionIdx, endVer - versionIdx).Trim();
                }

                string available = "";
                if (isUpgradeList && matchAvailableIdx != -1 && matchAvailableIdx < line.Length)
                {
                    int endAvail = (sourceIdx > matchAvailableIdx) ? sourceIdx : line.Length;
                    available = GetSubstringSafe(line, matchAvailableIdx, endAvail - matchAvailableIdx).Trim();
                }

                string source = "winget";
                if (sourceIdx != -1 && sourceIdx < line.Length)
                {
                    source = line.Substring(sourceIdx).Trim();
                }

                if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(id))
                {
                    packages.Add(new AppPackage
                    {
                        Name = name,
                        Id = id,
                        Version = version,
                        AvailableVersion = available,
                        Source = string.IsNullOrEmpty(source) ? "winget" : source
                    });
                }
            }
            catch
            {
                // Ignore misalignment
            }
        }

        return packages;
    }

    private string GetSubstringSafe(string text, int startIndex, int length)
    {
        if (startIndex >= text.Length) return "";
        if (startIndex + length > text.Length) return text.Substring(startIndex);
        return text.Substring(startIndex, length);
    }
}