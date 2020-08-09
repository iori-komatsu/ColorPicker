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

        public (float, float, float) ToHSV() {
            float max = Math.Max(R, Math.Max(G, B));
            float min = Math.Min(R, Math.Min(G, B));
            float delta = max - min;

            float h;
            if (delta < 1e-10) {
                h = 0;
            } else if (max == R) {
                h = (((G - B) / delta) % 6.0f) / 6.0f;
            } else if (max == G) {
                h = (((B - R) / delta) + 2.0f) / 6.0f;
            } else {
                h = (((R - G) / delta) + 4.0f) / 6.0f;
            }

            float s = max < 1e-10 ? 0 : delta / max;

            float v = max;

            return (h, s, v);
        }

        private static float ToSRGB1(float x) {
            float x_ = Math.Max(6.10352e-5f, x);
            return Math.Min(x_ * 12.92f, (float)Math.Pow(Math.Max(x_, 0.00313067f), 1.0 / 2.4) * 1.055f - 0.055f);
        }

        public ColorF ToSRGB() {
            return new ColorF(ToSRGB1(A), ToSRGB1(R), ToSRGB1(G), ToSRGB1(B));
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
