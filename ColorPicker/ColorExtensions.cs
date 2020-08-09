using System;
using System.Windows.Media;

namespace ColorPicker {
    static class ColorExtensions {
        public static Color Invert(this Color c) {
            return Color.FromArgb(c.A, (byte)(255 - c.R), (byte)(255 - c.G), (byte)(255 - c.B));
        }
    }
}
