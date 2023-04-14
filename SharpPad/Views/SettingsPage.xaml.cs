using SharpPad.Helpers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;
using Microsoft.UI.Xaml.Media;
using System;

namespace SharpPad.Views;

public sealed partial class SettingsPage : Page
{
    private List<Tuple<string, FontFamily>> Fonts = FontHelper.Fonts;
    private List<double> FontSizes = FontHelper.FontSizes;

    private string Version = AssemblyHelper.Version;

    public SettingsPage()
    {
        InitializeComponent();

        {
            switch (App.storageHelper.AppTheme)
            {
                case "Light":
                    AppTheme.SelectedIndex = 0;
                    break;
                case "Dark":
                    AppTheme.SelectedIndex = 1;
                    break;
                default:
                    AppTheme.SelectedIndex = 2;
                    break;
            }

            Sound.IsOn = App.storageHelper.Sound;
            Family.SelectedIndex = FontHelper.Fonts.FindIndex(f => f.Item1 == App.storageHelper.FontFamily);
            Size.SelectedIndex = FontHelper.FontSizes.IndexOf(App.storageHelper.FontSize);
        }
    }

    private void AppTheme_SelectionChanged(object sender, SelectionChangedEventArgs args)
    {
        var selectedTheme = ((ComboBoxItem)AppTheme.SelectedItem)?.Tag?.ToString();
        App.storageHelper.AppTheme = selectedTheme;

        switch (selectedTheme)
        {
            case "Light": (App.window.Content as FrameworkElement).RequestedTheme = ElementTheme.Light; break;
            case "Dark": (App.window.Content as FrameworkElement).RequestedTheme = ElementTheme.Dark; break;
            default: (App.window.Content as FrameworkElement).RequestedTheme = ElementTheme.Default; break;
        }
    }

    private void Sound_Toggled(object sender, RoutedEventArgs args)
    {
        App.storageHelper.Sound = Sound.IsOn;

        if (Sound.IsOn)
        {
            ElementSoundPlayer.State = ElementSoundPlayerState.On;
            ElementSoundPlayer.SpatialAudioMode = ElementSpatialAudioMode.On;
        }
        else
        {
            ElementSoundPlayer.State = ElementSoundPlayerState.Off;
            ElementSoundPlayer.SpatialAudioMode = ElementSpatialAudioMode.Off;
        }
    }

    private void FontFamily_SelectionChanged(object sender, SelectionChangedEventArgs args)
    {
        FontHelper.Font = (FontFamily)Family.SelectedValue;
        App.storageHelper.FontFamily = FontHelper.Font.Source;
    }

    private void FontSize_SelectionChanged(object sender, SelectionChangedEventArgs args)
    {
        FontHelper.FontSize = (double)Size.SelectedValue;
        App.storageHelper.FontSize = FontHelper.FontSize;
    }
}
