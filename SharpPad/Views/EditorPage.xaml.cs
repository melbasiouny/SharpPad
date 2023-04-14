using SharpPad.Helpers;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using WinRT.Interop;
using System.Runtime.InteropServices;
using Windows.Storage.Pickers;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace SharpPad.Views;

public sealed partial class EditorPage : Page
{
    [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto, PreserveSig = true, SetLastError = false)]
    public static extern IntPtr GetActiveWindow();
    private string BookmarkName { get; set; }
    private string PageName { get; set; }

    public EditorPage()
    {
        InitializeComponent();

        Editor.FontFamily = FontHelper.Font;
        Editor.FontSize = FontHelper.FontSize;
    }

    protected override void OnNavigatedTo(NavigationEventArgs args)
    {
        base.OnNavigatedTo(args);

        if (args.Parameter is (string bookmarkName, string pageName))
        {
            BookmarkName = bookmarkName;
            PageName = pageName;

            Editor.Document.SetText(TextSetOptions.FormatRtf | TextSetOptions.ApplyRtfDocumentDefaults, BookmarkHelper.LoadBookmarkPage(BookmarkName, PageName));
            ITextRange documentRange = Editor.Document.GetRange(0, TextConstants.MaxUnitCount);
            documentRange.CharacterFormat.ForegroundColor = ((Brush)Application.Current.Resources["SystemControlForegroundBaseHighBrush"] as SolidColorBrush).Color;
        }
    }

    private void SaveFileButton_Click(object sender, RoutedEventArgs args)
    {
        Editor.Document.GetText(TextGetOptions.FormatRtf, out var content);
        BookmarkHelper.SaveBookmarkPage(BookmarkName, PageName, content);
    }

    private async void OpenFileButton_Click(object sender, RoutedEventArgs args)
    {
        var fileOpenPicker = new FileOpenPicker();

        fileOpenPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
        fileOpenPicker.FileTypeFilter.Add(".rtf");

        if (Window.Current == null)
        {
            IntPtr hwnd = GetActiveWindow();
            InitializeWithWindow.Initialize(fileOpenPicker, hwnd);
        }

        var file = await fileOpenPicker.PickSingleFileAsync();

        if (file != null)
        {
            using (Windows.Storage.Streams.IRandomAccessStream randomAccessStream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
            {
                Editor.Document.LoadFromStream(TextSetOptions.FormatRtf, randomAccessStream);
            }
        }
    }

    private void SearchBox_TextChanged(object sender, TextChangedEventArgs args)
    {
        SearchRemoveHighlights();

        Color highlightForegroundColor = (Color)App.Current.Resources["SystemAccentColor"];

        string textToSearch = SearchBox.Text;
        if (textToSearch != null)
        {
            ITextRange searchRange = Editor.Document.GetRange(0, 0);
            while (searchRange.FindText(textToSearch, TextConstants.MaxUnitCount, FindOptions.None) > 0)
            {
                searchRange.CharacterFormat.ForegroundColor = highlightForegroundColor;
            }
        }
    }

    private void SearchRemoveHighlights()
    {
        ITextRange documentRange = Editor.Document.GetRange(0, TextConstants.MaxUnitCount);
        SolidColorBrush defaultForeground = Editor.Foreground as SolidColorBrush;

        documentRange.CharacterFormat.ForegroundColor = defaultForeground.Color;
    }
}
