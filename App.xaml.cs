using Microsoft.UI.Xaml;
using OpenStoreUI.ViewModels;

namespace OpenStoreUI;

public partial class App : Application
{
    public static MainViewModel MainVM { get; } = new();

    public App()
    {
        this.InitializeComponent();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        m_window = new MainWindow();
        m_window.Activate();
    }

    private Window m_window;
}