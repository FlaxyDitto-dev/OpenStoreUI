using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace OpenStoreUI;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        this.InitializeComponent();

        // Enable Translucent Glass / Mica Alt Material
        this.SystemBackdrop = new MicaBackdrop() { Kind = Microsoft.UI.Composition.SystemBackdrops.MicaKind.BaseAlt };

        NavView.SelectedItem = NavView.MenuItems[0];
        ContentFrame.Navigate(typeof(MainPage));
    }

    private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
    {
        if (args.IsSettingsInvoked)
        {
            ContentFrame.Navigate(typeof(SettingsPage));
            return;
        }

        if (args.InvokedItemContainer is NavigationViewItem item)
        {
            switch (item.Tag.ToString())
            {
                case "discover":
                    ContentFrame.Navigate(typeof(MainPage));
                    break;
                case "installed":
                    ContentFrame.Navigate(typeof(InstalledPage));
                    break;
                case "updates":
                    ContentFrame.Navigate(typeof(UpdatesPage));
                    break;
            }
        }
    }
}