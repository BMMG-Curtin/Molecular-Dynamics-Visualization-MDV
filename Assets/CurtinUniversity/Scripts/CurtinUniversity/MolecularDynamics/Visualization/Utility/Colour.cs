using UnityEngine;
using System;

namespace CurtinUniversity.MolecularDynamics.Visualization.Utility {

    public static class Colour {

        // Given H,S,L in range of 0-1
        // Returns a Color (RGB struct) in range of 0-255
        public static Color32 HSL2RGB(double h, double sl, double l) {
            double v;
            double r, g, b;

            r = l;   // default to gray
            g = l;
            b = l;
            v = (l <= 0.5) ? (l * (1.0 + sl)) : (l + sl - l * sl);
            if (v > 0) {
                double m;
                double sv;
                int sextant;
                double fract, vsf, mid1, mid2;

                m = l + l - v;
                sv = (v - m) / v;
                h *= 6.0;
                sextant = (int)h;
                fract = h - sextant;
                vsf = v * sv * fract;
                mid1 = m + vsf;
                mid2 = v - vsf;
                switch (sextant) {
                    case 0:
                        r = v;
                        g = mid1;
                        b = m;
                        break;
                    case 1:
                        r = mid2;
                        g = v;
                        b = m;
                        break;
                    case 2:
                        r = m;
                        g = v;
                        b = mid1;
                        break;
                    case 3:
                        r = m;
                        g = mid2;
                        b = v;
                        break;
                    case 4:
                        r = mid1;
                        g = m;
                        b = v;
                        break;
                    case 5:
                        r = v;
                        g = m;
                        b = mid2;
                        break;
                }
            }

            Debug.Log(r + ", " + g + ", " + b);
            Color32 rgb = new Color32(
                Convert.ToByte(r * 255.0f),
                Convert.ToByte(g * 255.0f),
                Convert.ToByte(b * 255.0f),
                255
            );

            return rgb;
        }

        public static Color32 HSVToRGB(float H, float S, float V) {
            if (S == 0f)
                return new Color(V, V, V);
            else if (V == 0f)
                return Color.black;
            else {
                Color col = Color.black;
                float Hval = H * 6f;
                int sel = Mathf.FloorToInt(Hval);
                float mod = Hval - sel;
                float v1 = V * (1f - S);
                float v2 = V * (1f - S * mod);
                float v3 = V * (1f - S * (1f - mod));
                switch (sel + 1) {
                    case 0:
                        col.r = V;
                        col.g = v1;
                        col.b = v2;
                        break;
                    case 1:
                        col.r = V;
                        col.g = v3;
                        col.b = v1;
                        break;
                    case 2:
                        col.r = v2;
                        col.g = V;
                        col.b = v1;
                        break;
                    case 3:
                        col.r = v1;
                        col.g = V;
                        col.b = v3;
                        break;
                    case 4:
                        col.r = v1;
                        col.g = v2;
                        col.b = V;
                        break;
                    case 5:
                        col.r = v3;
                        col.g = v1;
                        col.b = V;
                        break;
                    case 6:
                        col.r = V;
                        col.g = v1;
                        col.b = v2;
                        break;
                    case 7:
                        col.r = V;
                        col.g = v3;
                        col.b = v1;
                        break;
                }
                col.r = Mathf.Clamp(col.r, 0f, 1f);
                col.g = Mathf.Clamp(col.g, 0f, 1f);
                col.b = Mathf.Clamp(col.b, 0f, 1f);
                return col;
            }
        }
    }
}
