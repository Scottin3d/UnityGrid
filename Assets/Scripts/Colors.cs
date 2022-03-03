using UnityEngine;

namespace Utils {
    public static class ColorsUtils {
        public static Color CreateColorFrom255RGB(float r = 0f, float g = 0f, float b = 0f, float a = 1f) {
            return new Color(r / 255f, g / 255f, b / 255f, Mathf.Clamp01(a));
        }

        public static Color PixelStripe(int v) {
            Texture2D tex = Resources.Load("Texture/BRGBWStrip.psd", typeof(Texture2D)) as Texture2D;
            Mathf.Clamp(v, 0, 100);
            return tex.GetPixel(0, v);
        }

        public static Color[] BRGBWColorCache() {
            Texture2D tex = Resources.Load("Assets/Resources/Texture/BRGBWStrip.psd", typeof(Texture2D)) as Texture2D;
            return tex.GetPixels();
        }

        public static Color BlendColors(Color a, Color b, float alpha = 1f) {
            return new Color((a.r + b.r) / 2f, 
                             (a.g + b.g) / 2f, 
                             (a.b + b.b) / 2f,
                             (alpha));
        }
    }



}