using LuftSchloss;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DistributionPrototype.UI {
    public class UIController : MonoBehaviour {
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

            SetTexture(NoiseImage, noiseTex);
            SetTexture(NoiseThresholdImage, noiseThresholdTex);
        }

        private static void SetTexture(Image image, Texture2D texture) {
            image.sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }
    }
}
