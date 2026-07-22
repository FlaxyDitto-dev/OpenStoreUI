using Microsoft.UI.Xaml.Controls;
using OpenStoreUI.ViewModels;

namespace OpenStoreUI;

public sealed partial class SettingsPage : Page
{
    public SettingsPage()
    {
        this.InitializeComponent();
        
        // Match currently selected source
        string current = App.MainVM.WinGetService.SelectedSource;
        SourceComboBox.SelectedIndex = current switch
        {
            "winget" => 1,
            "msstore" => 2,
            _ => 0
        };
    }

    private void SourceComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (SourceComboBox.SelectedItem is ComboBoxItem item && item.Tag is string tag)
        {
            App.MainVM.WinGetService.SelectedSource = tag;
        }
    }
}