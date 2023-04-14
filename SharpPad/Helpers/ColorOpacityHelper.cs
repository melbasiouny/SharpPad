using Microsoft.UI.Xaml.Media;
using System.Linq;
using Windows.UI;

namespace SharpPad.Helpers;

internal class ColorOpacityHelper
{
    public static SolidColorBrush GenerateOpaqueColor(string bookmarkName, int? colorCode = null)
    {
        var color = ColorsHelper.BookmarkColors.FirstOrDefault(c => c.Item1 == (colorCode == null ? BookmarkColorLoaderHelper.GetCode(bookmarkName) : colorCode)).Item2;
        var modifiedColor = Color.FromArgb(60, color.R, color.G, color.B);

        return new SolidColorBrush(modifiedColor);
    }
}
