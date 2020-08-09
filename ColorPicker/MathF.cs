using System;

namespace ColorPicker {
    static class MathF {
        public static float Lerp(float x, float y, float s) {
            return x + s * (y - x);
        }

        public static float Saturate(float x) {
            if (x < 0) return 0;
            if (x > 1) return 1;
            return x;
        }

        public static float Frac(float x) {
            return x - (int)x;
        }

        public static float Sq(float x) {
            return x * x;
        }

        public static bool Between(float x, float lo, float hi) {
            return lo <= x && x <= hi;
        }
    }
}
