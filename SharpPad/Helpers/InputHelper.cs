using System.Text.RegularExpressions;

namespace SharpPad.Helpers;

internal class InputHelper
{
    public static bool ValidBookmarkName(string bookmarkName) => Regex.IsMatch(bookmarkName, @"^[A-Za-z][A-Za-z_\-\. ]{0,14}[A-Za-z]$");

    public static bool ValidPageName(string pageName) => Regex.IsMatch(pageName, @"^[A-Za-z][A-Za-z_\-\. ]{0,22}[A-Za-z]$");
}
