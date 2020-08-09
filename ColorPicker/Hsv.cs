using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColorPicker {
    public struct Hsv {
        public float H { get; }
        public float S { get; }
        public float V { get; }

        public Hsv(float h, float s, float v) {
            this.H = h;
            this.S = s;
            this.V = v;
        }

        public static bool operator==(Hsv lhs, Hsv rhs) {
            return lhs.H == rhs.H && lhs.S == rhs.S && lhs.V == rhs.V;
        }

        public static bool operator!=(Hsv lhs, Hsv rhs) {
            return !(lhs == rhs);
        }

        public override bool Equals(object obj) {
            if (null == obj || this.GetType() != obj.GetType())
                return false;
            return this == (Hsv)obj;
        }

        public override int GetHashCode() {
            const uint a = 1103515245;
            uint h = (uint)H.GetHashCode() + a * ((uint)S.GetHashCode() + a * (uint)V.GetHashCode());
            return (int)h;
        }
    }
}
