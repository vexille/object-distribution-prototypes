using System;
using DistributionPrototype.Config;
using DistributionPrototype.Util;
using UnityEngine;
using UnityEngine.UI;

namespace DistributionPrototype
{
	public class NoiseDisplayPanel : MonoBehaviour
	{
		public NoiseType Type;

		public Image NoiseImage;
		public Image NoiseThresholdImage;

		public void RenderNoise(Grid2D<float> noise, float threshold)
		{
			var noiseTex = new Texture2D(noise.Width, noise.Height);
			var noiseThresholdTex = new Texture2D(noise.Width, noise.Height);

			for (int x = 0; x < noise.Width; x++)
			{
				for (int y = 0; y < noise.Height; y++)
				{
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