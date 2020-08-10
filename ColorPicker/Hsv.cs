using System;

namespace ColorPicker {
    public struct Hsv : IEquatable<Hsv> {
        public float H { get; }
        public float S { get; }
        public float V { get; }

        public Hsv(float h, float s, float v) {
            this.H = h;
            this.S = s;
            this.V = v;
        }

        public override bool Equals(object obj) {
            return obj is Hsv hsv && Equals(hsv);
        }

        public bool Equals(Hsv other) {
            return H == other.H &&
                   S == other.S &&
                   V == other.V;
        }

        public override int GetHashCode() {
            int hashCode = -1397884734;
            hashCode = hashCode * -1521134295 + H.GetHashCode();
            hashCode = hashCode * -1521134295 + S.GetHashCode();
            hashCode = hashCode * -1521134295 + V.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(Hsv left, Hsv right) {
            return left.Equals(right);
        }

        public static bool operator !=(Hsv left, Hsv right) {
            return !(left == right);
        }
    }
}
