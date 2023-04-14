using System;
using System.IO;
using System.Linq;
using SharpPad.Views;
using SharpPad.Helpers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using Microsoft.UI;
using Windows.UI;

namespace SharpPad;

public sealed partial class MainWindow : Window
{
    private NavigationViewItem RightTappedBookmark;
    private ObservableCollection<NavigationViewItem> Bookmarks;

    public MainWindow()
    {
        InitializeComponent();

        DispatcherQueue.TryEnqueue(() =>
        {
            App.window.SetTitleBar(AppTitleBar);

            switch (App.storageHelper.AppTheme)
            {
                case "Light":
                    (App.window.Content as FrameworkElement).RequestedTheme = ElementTheme.Light;
                    break;
                case "Dark":
                    (App.window.Content as FrameworkElement).RequestedTheme = ElementTheme.Dark;
                    break;
                default:
                    (App.window.Content as FrameworkElement).RequestedTheme = ElementTheme.Default;
                    break;
            }

            if (App.storageHelper.Sound)
            {
                ElementSoundPlayer.State = ElementSoundPlayerState.On;
                ElementSoundPlayer.SpatialAudioMode = ElementSpatialAudioMode.On;
            }
            else
            {
                ElementSoundPlayer.State = ElementSoundPlayerState.Off;
                ElementSoundPlayer.SpatialAudioMode = ElementSpatialAudioMode.Off;
            }

            FontHelper.Font = new FontFamily(App.storageHelper.FontFamily);
            FontHelper.FontSize = App.storageHelper.FontSize;
        });

        Bookmarks = new ObservableCollection<NavigationViewItem>();
        BookmarksView.MenuItemsSource = Bookmarks;
        PageFrame.Navigate(typeof(InitialPage));
        LoadBookmarks();
        LoadColors();
    }

    private void BookmarkName_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
    {
        AddBookmarkButton.IsEnabled = !string.IsNullOrEmpty(sender.Text)
            && !Bookmarks.Any(item => (string)item.Content == sender.Text)
            && InputHelper.ValidBookmarkName(sender.Text);
    }

    private void AddBookmark_Click(object sender, RoutedEventArgs args)
    {
        AddBookmark(BookmarkName.Text, ColorPickerHelper.GetSelectedColorCode());
        BookmarkHelper.CreateBookmark(BookmarkName.Text, ColorPickerHelper.GetSelectedColorCode());

        AddBookmarkFlyout.Hide();
        BookmarkName.Text = string.Empty;
        AddBookmarkButton.IsEnabled = false;
    }

    private void BookmarksView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        var navigationViewItem = args.SelectedItemContainer as NavigationViewItem;

        if (navigationViewItem != null && navigationViewItem.Tag != null)
        {
            if (args.IsSettingsSelected) NavigateTo(typeof(SettingsPage), navigationViewItem);
            else if (navigationViewItem.Tag.ToString() == "Bookmark") NavigateTo(typeof(BookmarkPage), navigationViewItem);
        }
        else PageFrame.Navigate(typeof(InitialPage));
    }

    private void Bookmark_RightTapped(object sender, RightTappedRoutedEventArgs args)
    {
        NavigationViewItem bookmark = sender as NavigationViewItem;
        BookmarkFlyoutMenu.ShowAt(bookmark);
        RightTappedBookmark = bookmark;
    }

    private void RemoveBookmark_Click(object sender, RoutedEventArgs args)
    {
        DispatcherQueue.TryEnqueue(async () =>
        {
            var dialog = new ContentDialog();
            dialog.XamlRoot = Content.XamlRoot;
            dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
            dialog.Title = $"Remove {RightTappedBookmark.Content}?";
            dialog.PrimaryButtonText = "Remove";
            dialog.SecondaryButtonText = "Cancel";
            dialog.Content = "When you remove a bookmark, all the associated pages will be removed as well. Please note that this action is irreversible and cannot be undone.";
            dialog.DefaultButton = ContentDialogButton.Secondary;
            dialog.Loaded += OnLoaded;

            void OnLoaded(object sender, RoutedEventArgs args)
            {
                DependencyObject popupRoot = VisualTreeHelper.GetParent(dialog);

                Rectangle smokeLayer = null;
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(popupRoot); i++)
                {
                    var child = VisualTreeHelper.GetChild(popupRoot, i);
                    if (child is Rectangle rectangle)
                    {
                        smokeLayer = rectangle;
                        break;
                    }
                }

                if (smokeLayer != null)
                {
                    smokeLayer.Margin = new Thickness(0);
                    smokeLayer.RegisterPropertyChangedCallback(FrameworkElement.MarginProperty,
                        (depObj, depPrprty) =>
                        {
                            if (depPrprty == FrameworkElement.MarginProperty) depObj.ClearValue(depPrprty);
                        });
                }
            }

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                BookmarkHelper.DeleteBookmark((string)RightTappedBookmark.Content);
                Bookmarks.Remove(RightTappedBookmark);
            }
        });
    }

    private void LoadBookmarks()
    {
        foreach (var bookmark in BookmarkHelper.GetBookmarks()) AddBookmark(System.IO.Path.GetFileName(bookmark));
    }

    private void LoadColors()
    {
        ColorPickerHelper colorPickerHelper = new ColorPickerHelper();
        var clrs = colorPickerHelper.InitializeColorPicker();

        foreach (var color in clrs)
        {
            ColorPicker.Children.Add(color);
        }
    }

    private NavigationViewItem AddBookmark(string bookmarkName, int? colorCode = null)
    {
        var bookmark = new NavigationViewItem();

        bookmark.Tag = "Bookmark";
        bookmark.Content = bookmarkName;
        bookmark.RightTapped += Bookmark_RightTapped;
        bookmark.Icon = new SymbolIcon(Symbol.Bookmarks);
        bookmark.Background = ColorOpacityHelper.GenerateOpaqueColor(bookmarkName, colorCode);

        Bookmarks.Add(bookmark);

        return bookmark;
    }

    private void NavigateTo(Type page, NavigationViewItem navigationViewItem)
    {
        BookmarksView.SelectedItem = navigationViewItem;
        PageFrame.Navigate(page, navigationViewItem.Tag.ToString() == "Bookmark" ? navigationViewItem.Content : null);
    }
}
