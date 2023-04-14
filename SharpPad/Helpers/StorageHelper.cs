namespace SharpPad.Helpers;

public class StorageHelper
{
    public string AppTheme { get; set; }
    public bool Sound { get; set; }
    public string FontFamily { get; set; }
    public double FontSize { get; set; }

    public StorageHelper()
    {
        var sound = Windows.Storage.ApplicationData.Current.LocalSettings.Values["Sound"];
        var appTheme = Windows.Storage.ApplicationData.Current.LocalSettings.Values["AppTheme"];
        var fontSize = Windows.Storage.ApplicationData.Current.LocalSettings.Values["FontSize"];
        var fontFamily = Windows.Storage.ApplicationData.Current.LocalSettings.Values["FontFamily"];

        Sound = sound == null ? true : (bool)sound;
        FontSize = fontSize == null ? 16 : (double)fontSize;
        AppTheme = appTheme == null ? "Default" : (string)appTheme;
        FontFamily = fontFamily == null ? "Cascadia Code" : (string)fontFamily;
    }

    public void StoreApplicationData()
    {
        Windows.Storage.ApplicationData.Current.LocalSettings.Values["Sound"] = Sound;
        Windows.Storage.ApplicationData.Current.LocalSettings.Values["FontSize"] = FontSize;
        Windows.Storage.ApplicationData.Current.LocalSettings.Values["AppTheme"] = AppTheme;
        Windows.Storage.ApplicationData.Current.LocalSettings.Values["FontFamily"] = FontFamily;
    }
}
