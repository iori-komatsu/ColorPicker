using System;
using System.Windows.Media;

namespace ColorPicker {
    struct ColorF {
        public float A { get; private set; }
        public float R { get; private set; }
        public float G { get; private set; }
        public float B { get; private set; }

        public ColorF(float a, float r, float g, float b) {
            A = a;
            R = r;
            G = g;
            B = b;
        }

        public ColorF(float r, float g, float b) : this(1.0f, r, g, b) { }

        public static ColorF FromHsv(float h, float s, float v) {
            float p1 = Math.Abs(MathF.Frac(h + 1.0f) * 6.0f - 3.0f);
            float p2 = Math.Abs(MathF.Frac(h + 2.0f / 3.0f) * 6.0f - 3.0f);
            float p3 = Math.Abs(MathF.Frac(h + 1.0f / 3.0f) * 6.0f - 3.0f);
            float r = v * MathF.Lerp(1.0f, MathF.Saturate(p1 - 1.0f), s);
            float g = v * MathF.Lerp(1.0f, MathF.Saturate(p2 - 1.0f), s);
            float b = v * MathF.Lerp(1.0f, MathF.Saturate(p3 - 1.0f), s);
            return new ColorF(r, g, b);
        }

        private static byte FloatToByte(float x) {
            int i = (int)(x * 256.0f);
            if (i < 0) return 0;
            if (i > 255) return 255;
            return (byte)i;
        }

        public Color ToColor() {
            return Color.FromArgb(
                FloatToByte(A),
                FloatToByte(R),
                FloatToByte(G),
                FloatToByte(B)
            );
        }
    }
}
