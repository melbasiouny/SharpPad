using SharpPad.Helpers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System.Collections.Generic;
using System.Linq;
using Microsoft.UI.Xaml.Media;
using System;
using Microsoft.UI.Xaml.Shapes;

namespace SharpPad.Views;

public sealed partial class BookmarkPage : Page
{
    private string BookmarkName { get; set; }
    private List<string> PageNames { get; set; }

    public BookmarkPage()
    {
        InitializeComponent();

        PageNames = new List<string>();
    }

    protected override void OnNavigatedTo(NavigationEventArgs args)
    {
        base.OnNavigatedTo(args);

        if (args.Parameter is string bookmarkName)
        {
            BookmarkName = bookmarkName;
            BookmarkPagesHeader.Text = bookmarkName;
            BookmarkPages.Background = ColorOpacityHelper.GenerateOpaqueColor(bookmarkName, BookmarkColorLoaderHelper.GetCode(bookmarkName));
        }
    }

    private void BookmarkPages_AddTabButtonClick(TabView sender, object args)
    {
        var matchedPageNames = PageNames.Where(name => name.StartsWith("Untitled page"));
        var count = matchedPageNames.Any()
                    ? matchedPageNames.Select(name => int.TryParse(name.Substring("Untitled page ".Length), out int number) ? number : 0)
                      .Max() + 1
                    : 1;

        var pageName = "Untitled page" + (count > 0 ? $" {count}" : string.Empty);

        BookmarkHelper.CreateBookmarkPage(BookmarkName, pageName);
        sender.TabItems.Add(CreateNewPage(pageName));
    }

    private void BookmarkPages_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
    {
        TabViewItem page = args.Item as TabViewItem;

        DispatcherQueue.TryEnqueue(async () =>
        {
            var dialog = new ContentDialog();
            dialog.XamlRoot = Content.XamlRoot;
            dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
            dialog.Title = $"Remove {page.Header}?";
            dialog.PrimaryButtonText = "Remove";
            dialog.SecondaryButtonText = "Cancel";
            dialog.Content = "By removing this page, all the data associated with it will be lost. Are you sure you want to proceed?";
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
                BookmarkHelper.DeleteBookmarkPage(BookmarkName, page.Header.ToString());
                PageNames.Remove(page.Header.ToString());
                sender.TabItems.Remove(args.Tab);

                if (PageNames.Count == 0)
                {
                    BookmarkHelper.CreateBookmarkPage(BookmarkName, "Untitled page 1");
                    sender.TabItems.Add(CreateNewPage("Untitled page 1"));
                    sender.SelectedIndex = 0;
                }
            }
        });
    }

    private void BookmarkPages_Loaded(object sender, RoutedEventArgs args)
    {
        foreach (var page in BookmarkHelper.GetBookmarkPages(BookmarkName)) (sender as TabView).TabItems.Add(CreateNewPage(System.IO.Path.GetFileNameWithoutExtension(page)));
    }

    private TabViewItem CreateNewPage(string pageName)
    {
        Frame editorPage = new Frame();
        TabViewItem page = new TabViewItem();

        page.Header = pageName;
        page.IconSource = new SymbolIconSource { Symbol = Symbol.Page2 };
        page.DoubleTapped += Page_DoubleTapped;

        editorPage.Navigate(typeof(EditorPage), (BookmarkName, pageName));
        page.Content = editorPage;

        PageNames.Add(pageName);

        return page;
    }

    private void Page_DoubleTapped(object sender, DoubleTappedRoutedEventArgs args)
    {
        TabViewItem page = (sender as TabViewItem);
        TextBox pageRename = new TextBox();

        var pageName = page.Header.ToString();
        pageRename.MaxLength = 24;
        pageRename.Width = page.Width - 76;
        pageRename.PlaceholderText = "Page name";
        pageRename.Text = page.Header.ToString();
        pageRename.LostFocus += delegate
        {
            if (InputHelper.ValidPageName(pageRename.Text) && !PageNames.Contains(pageRename.Text))
            {
                BookmarkHelper.RenameBookmarkPage(BookmarkName, pageName, pageRename.Text);
                var previousName = PageNames.IndexOf(pageName);
                PageNames[previousName] = pageRename.Text;
                page.Header = pageRename.Text;
            }
            else
            {
                page.Header = pageName;
            }
        };

        page.Header = pageRename;
    }
}
