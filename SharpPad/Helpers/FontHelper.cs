using System;
using Microsoft.UI.Xaml.Media;
using System.Collections.Generic;

namespace SharpPad.Helpers;

internal class FontHelper
{
    public static FontFamily Font { get; set; } = new FontFamily("Cascadia Code");

    public static double FontSize { get; set; } = 16;

    public static List<Tuple<string, FontFamily>> Fonts { get; } = new List<Tuple<string, FontFamily>>()
    {
        new Tuple<string, FontFamily>("Arial", new FontFamily("Arial")),
        new Tuple<string, FontFamily>("Comic Sans MS", new FontFamily("Comic Sans MS")),
        new Tuple<string, FontFamily>("Courier New", new FontFamily("Courier New")),
        new Tuple<string, FontFamily>("Segoe UI", new FontFamily("Segoe UI")),
        new Tuple<string, FontFamily>("Cascadia Code", new FontFamily("Cascadia Code")),
        new Tuple<string, FontFamily>("Times New Roman", new FontFamily("Times New Roman"))
    };

    public static List<double> FontSizes { get; } = new List<double>()
    {
        8,
        9,
        10,
        11,
        12,
        14,
        16,
        18,
        20,
        24,
        28,
        36,
        48,
        72
    };
}
