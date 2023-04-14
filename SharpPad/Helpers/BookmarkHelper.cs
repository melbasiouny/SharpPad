using System.IO;
using System.Collections.Generic;

namespace SharpPad.Helpers;

internal class BookmarkHelper
{
    public static string RootDirectory = Windows.Storage.ApplicationData.Current.LocalFolder.Path;

    public static BookmarkColorLoaderHelper BookmarkColorLoaderHelper { get; set; } = new(Path.Combine(RootDirectory, "BookmarkColors.data"));

    public static void CreateBookmark(string bookmarkName, int colorCode)
    {
        var bookmarkDirectory = Path.Combine(RootDirectory, bookmarkName);

        if (!Directory.Exists(bookmarkDirectory))
        {
            Directory.CreateDirectory(bookmarkDirectory);
            CreateBookmarkPage(bookmarkName, "Untitled page 1");
            BookmarkColorLoaderHelper.Add(bookmarkName, colorCode);
        }
    }

    public static void CreateBookmarkPage(string bookmarkName, string pageName)
    {
        var bookmarkDirectory = Path.Combine(RootDirectory, bookmarkName);
        var pageDirectory = Path.Combine(bookmarkDirectory, pageName + ".rtf");

        if (Directory.Exists(bookmarkDirectory) && !File.Exists(pageDirectory)) File.Create(pageDirectory).Close();
    }

    public static void SaveBookmarkPage(string bookmarkName, string pageName, string content)
    {
        var bookmarkDirectory = Path.Combine(RootDirectory, bookmarkName);
        var pageDirectory = Path.Combine(bookmarkDirectory, pageName + ".rtf");

        if (Directory.Exists(bookmarkDirectory) && File.Exists(pageDirectory)) File.WriteAllText(pageDirectory, content);
    }

    public static void RenameBookmarkPage(string bookmarkName, string previousPageName, string newPageName)
    {
        var bookmarkDirectory = Path.Combine(RootDirectory, bookmarkName);
        var previousPageDirectory = Path.Combine(bookmarkDirectory, previousPageName + ".rtf");
        var newPageDirectory = Path.Combine(bookmarkDirectory, newPageName + ".rtf");

        if (Directory.Exists(bookmarkDirectory) && File.Exists(previousPageDirectory)) File.Move(previousPageDirectory, newPageDirectory);
    }

    public static string LoadBookmarkPage(string bookmarkName, string pageName)
    {
        var content = string.Empty;
        var bookmarkDirectory = Path.Combine(RootDirectory, bookmarkName);
        var pageDirectory = Path.Combine(bookmarkDirectory, pageName + ".rtf");

        if (Directory.Exists(bookmarkDirectory) && File.Exists(pageDirectory)) content = File.ReadAllText(pageDirectory);

        return content;
    }

    public static void DeleteBookmark(string bookmarkName)
    {
        var bookmarkDirectory = Path.Combine(RootDirectory, bookmarkName);

        if (Directory.Exists(bookmarkDirectory))
        {
            Directory.Delete(bookmarkDirectory, true);
            BookmarkColorLoaderHelper.Remove(bookmarkName);
        }
    }

    public static void DeleteBookmarkPage(string bookmarkName, string pageName)
    {
        var bookmarkDirectory = Path.Combine(RootDirectory, bookmarkName);
        var pageDirectory = Path.Combine(bookmarkDirectory, pageName + ".rtf");

        if (Directory.Exists(bookmarkDirectory) && File.Exists(pageDirectory)) File.Delete(pageDirectory);
    }

    public static List<string> GetBookmarks()
    {
        var bookmarks = new List<string>();
        
        bookmarks.AddRange(Directory.GetDirectories(RootDirectory));
        
        return bookmarks;
    }

    public static List<string> GetBookmarkPages(string bookmarkName)
    {
        var pages = new List<string>();

        pages.AddRange(Directory.GetFiles(Path.Combine(RootDirectory, bookmarkName)));

        return pages;
    }
}
