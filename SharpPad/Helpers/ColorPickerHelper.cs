using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI;

namespace SharpPad.Helpers;

internal class ColorPickerHelper
{
    private List<Button> ColorPicker { get; set; } = new List<Button>();
    public static Color SelectedColor { get; set; } = ColorsHelper.BookmarkColors.First().Item2;

    public List<Button> InitializeColorPicker()
    {
        foreach (var color in ColorsHelper.BookmarkColors)
        {
            Button colorButton = new Button();
            Grid colorContent = new Grid();

            colorContent.Children.Add(new Rectangle() { Fill = new SolidColorBrush(color.Item2) });
            colorContent.Children.Add(new FontIcon() { Glyph = "\uF13E", Visibility = SelectedColor == color.Item2 ? Visibility.Visible : Visibility.Collapsed });
            colorButton.Content = colorContent;
            colorButton.Click += (sender, e) =>
            {
                foreach (var button in ColorPicker)
                {
                    if (button == colorButton)
                    {
                        ((Grid)button.Content).Children[1].Visibility = Visibility.Visible;
                        SelectedColor = ((((Grid)button.Content).Children[0] as Rectangle).Fill as SolidColorBrush).Color;
                    }
                    else
                    {
                        ((Grid)button.Content).Children[1].Visibility = Visibility.Collapsed;
                    }
                }
            };

            ColorPicker.Add(colorButton);
        }

        return ColorPicker;
    }

    public static int GetSelectedColorCode()
    {
        return ColorsHelper.BookmarkColors.FirstOrDefault(c => c.Item2 == SelectedColor).Item1;
    }
}
