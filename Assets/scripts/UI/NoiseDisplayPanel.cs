using DistributionPrototype.Config;
using LuftSchloss;
using UnityEngine;
using UnityEngine.UI;

namespace DistributionPrototype {
    public class NoiseDisplayPanel : MonoBehaviour {
        public NoiseType Type;

        public Image NoiseImage;
        public Image NoiseThresholdImage;

        public void RenderNoise(Grid<float> noise, float threshold) {
            var noiseTex = new Texture2D(noise.Width, noise.Height);
            var noiseThresholdTex = new Texture2D(noise.Width, noise.Height);

            for (int y = 0; y < noise.Width; y++) {
                for (int x = 0; x < noise.Height; x++) {
                    var val = noise.Get(x, y);
                    var maskColor = val <= threshold ? Color.white : Color.black;

                    noiseTex.SetPixel(x, y, new Color(val, val, val));
                    noiseThresholdTex.SetPixel(x, y, maskColor);
                }
            }

            noiseTex.Apply();
            noiseThresholdTex.Apply();

            NoiseImage.SetTexture(noiseTex);
            NoiseThresholdImage.SetTexture(noiseThresholdTex);
        }
    }
}
