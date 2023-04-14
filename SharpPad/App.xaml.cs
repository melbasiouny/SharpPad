using SharpPad.Helpers;
using Microsoft.UI.Xaml;

namespace SharpPad;

public partial class App : Application
{
    public static Window window;
    public static StorageHelper storageHelper;

    public App()
    {
        InitializeComponent();

        storageHelper = new StorageHelper();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        window = new MainWindow();
        window.ExtendsContentIntoTitleBar = true;
        window.Closed += Window_Closed;
        window.Title = "SharpPad";
        window.Activate();

        var backdropHelper = new BackdropHelper();
        backdropHelper.ActivateBackdrop();
    }

    private void Window_Closed(object sender, WindowEventArgs args)
    {
        storageHelper.StoreApplicationData();
    }
}
