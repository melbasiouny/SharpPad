using System;
using System.Collections.Generic;
using Windows.UI;

namespace SharpPad.Helpers;

internal class ColorsHelper
{
    public static List<Tuple<int, Color>> BookmarkColors { get; private set; } = new List<Tuple<int, Color>>()
    {
        new Tuple<int, Color>(1, Color.FromArgb(100, 255, 77, 77)),
        new Tuple<int, Color>(2, Color.FromArgb(100, 253, 77, 255)),
        new Tuple<int, Color>(3, Color.FromArgb(100, 140, 47, 255)),
        new Tuple<int, Color>(4, Color.FromArgb(100, 47, 52, 255)),
        new Tuple<int, Color>(5, Color.FromArgb(100, 47, 169, 255)),
        new Tuple<int, Color>(6, Color.FromArgb(100, 47, 248, 255)),
        new Tuple<int, Color>(7, Color.FromArgb(100, 47, 255, 174)),
        new Tuple<int, Color>(8, Color.FromArgb(100, 47, 255, 62)),
        new Tuple<int, Color>(9, Color.FromArgb(100, 219, 255, 51)),
        new Tuple<int, Color>(10, Color.FromArgb(100, 255, 219, 51)),
        new Tuple<int, Color>(11, Color.FromArgb(100, 255, 142, 51)),
    };
}
