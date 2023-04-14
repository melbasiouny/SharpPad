using System.Collections.Generic;
using System.IO;

namespace SharpPad.Helpers;

internal class BookmarkColorLoaderHelper
{
    private static Dictionary<string, int> BookmarkColors;
    private string Filename { get; set; }

    public BookmarkColorLoaderHelper(string filename)
    {
        BookmarkColors = new Dictionary<string, int>();
        Filename = filename;

        LoadFromFile();
    }

    public void Add(string bookmark, int colorCode)
    {
        BookmarkColors[bookmark] = colorCode;

        SaveToFile();
    }

    public void Remove(string directory)
    {
        BookmarkColors.Remove(directory);

        SaveToFile();
    }

    public static int GetCode(string directory)
    {
        return BookmarkColors.TryGetValue(directory, out var colorCode) ? colorCode : 1;
    }

    public void SaveToFile()
    {
        List<string> lines = new List<string>();
        foreach (KeyValuePair<string, int> pair in BookmarkColors)
        {
            lines.Add($"{pair.Key}|{pair.Value}");
        }
        File.WriteAllLines(Filename, lines);
    }

    public void LoadFromFile()
    {
        if (File.Exists(Filename))
        {
            string[] lines = File.ReadAllLines(Filename);
            foreach (string line in lines)
            {
                string[] parts = line.Split('|');
                string directory = parts[0];
                int code = int.Parse(parts[1]);
                Add(directory, code);
            }
        }
    }
}
