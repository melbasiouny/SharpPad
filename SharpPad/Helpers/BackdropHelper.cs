using WinRT;
using Microsoft.UI.Xaml;
using System.Runtime.InteropServices;

namespace SharpPad.Helpers;

internal class BackdropHelper
{
    WindowsSystemDispatcherQueueHelper m_wsdqHelper;
    Microsoft.UI.Composition.SystemBackdrops.MicaController m_controller;
    Microsoft.UI.Composition.SystemBackdrops.SystemBackdropConfiguration m_configurationSource;

    public void ActivateBackdrop()
    {
        if (Microsoft.UI.Composition.SystemBackdrops.MicaController.IsSupported())
        {
            m_wsdqHelper = new WindowsSystemDispatcherQueueHelper();
            m_wsdqHelper.EnsureWindowsSystemDispatcherQueueController();

            m_configurationSource = new Microsoft.UI.Composition.SystemBackdrops.SystemBackdropConfiguration();
            App.window.Activated += OnActivated;
            App.window.Closed += OnClosed;
            ((FrameworkElement)App.window.Content).ActualThemeChanged += OnThemeChanged;

            m_configurationSource.IsInputActive = true;
            SetConfigurationSourceTheme();

            m_controller = new Microsoft.UI.Composition.SystemBackdrops.MicaController();
            m_controller.AddSystemBackdropTarget(App.window.As<Microsoft.UI.Composition.ICompositionSupportsSystemBackdrop>());
            m_controller.SetSystemBackdropConfiguration(m_configurationSource);
        }
    }

    private void OnActivated(object sender, WindowActivatedEventArgs args)
    {
        m_configurationSource.IsInputActive = args.WindowActivationState != WindowActivationState.Deactivated;
    }

    private void OnClosed(object sender, WindowEventArgs args)
    {
        if (m_controller != null)
        {
            m_controller.Dispose();
            m_controller = null;
        }

        App.window.Activated -= OnActivated;
        m_configurationSource = null;
    }

    private void OnThemeChanged(FrameworkElement sender, object args)
    {
        if (m_configurationSource != null) SetConfigurationSourceTheme();
    }

    private void SetConfigurationSourceTheme()
    {
        switch (((FrameworkElement)App.window.Content).ActualTheme)
        {
            case ElementTheme.Dark: m_configurationSource.Theme = Microsoft.UI.Composition.SystemBackdrops.SystemBackdropTheme.Dark; break;
            case ElementTheme.Light: m_configurationSource.Theme = Microsoft.UI.Composition.SystemBackdrops.SystemBackdropTheme.Light; break;
            case ElementTheme.Default: m_configurationSource.Theme = Microsoft.UI.Composition.SystemBackdrops.SystemBackdropTheme.Default; break;
        }
    }

    internal class WindowsSystemDispatcherQueueHelper
    {
        [StructLayout(LayoutKind.Sequential)]
        struct DispatcherQueueOptions
        {
            internal int dwSize;
            internal int threadType;
            internal int apartmentType;
        }

        [DllImport("CoreMessaging.dll")]
        private static extern int CreateDispatcherQueueController([In] DispatcherQueueOptions options, [In, Out, MarshalAs(UnmanagedType.IUnknown)] ref object dispatcherQueueController);

        object m_dispatcherQueueController = null;

        public void EnsureWindowsSystemDispatcherQueueController()
        {
            if (Windows.System.DispatcherQueue.GetForCurrentThread() != null) return;

            if (m_dispatcherQueueController == null)
            {
                DispatcherQueueOptions options;
                options.dwSize = Marshal.SizeOf(typeof(DispatcherQueueOptions));
                options.threadType = 2;
                options.apartmentType = 2;

                CreateDispatcherQueueController(options, ref m_dispatcherQueueController);
            }
        }
    }
}
