using DistributionPrototype.Messages;
using DistributionPrototype.Util;
using Frictionless;
using UnityEngine;
using UnityEngine.UI;

namespace DistributionPrototype
{
	public class NoiseDisplayPanel : MonoBehaviour
	{
		public Image NoiseImage;
		public Image NoiseThresholdImage;
		
		private void Start()
		{
			var messageRounter = ServiceFactory.Instance.Resolve<MessageRouter>();
			messageRounter.AddHandler<GenerationStartedMessage>(message =>
			{
				ResetVisualization();
			});

			messageRounter.AddHandler<SamplerNoiseGeneratedMessage>(message =>
			{
				NoiseImage.gameObject.SetActive(true);
				RenderNoise(message.Noise);
			});
			messageRounter.AddHandler<LimiterNoiseGeneratedMessage>(message =>
			{
				NoiseThresholdImage.gameObject.SetActive(true);
				RenderThreshold(message.Noise, message.Threshold);
			});
		}

		private void ResetVisualization()
		{
			// Deactivate both images at start so only the used noises are active
			NoiseImage.gameObject.SetActive(false);
			NoiseThresholdImage.gameObject.SetActive(false);
		}

		public void RenderNoise(Grid2D<float> noise)
		{
			var noiseTex = new Texture2D(noise.Width, noise.Height);
			for (int x = 0; x < noise.Width; x++)
			{
				for (int y = 0; y < noise.Height; y++)
				{
					var val = noise.Get(x, y);

					noiseTex.SetPixel(x, y, new Color(val, val, val));
				}
			}

			noiseTex.Apply();
			NoiseImage.SetTexture(noiseTex);
		}

		private void RenderThreshold(Grid2D<float> noise, float threshold)
		{
			var noiseThresholdTex = new Texture2D(noise.Width, noise.Height);
			for (int x = 0; x < noise.Width; x++)
			{
				for (int y = 0; y < noise.Height; y++)
				{
					var maskColor = noise.Get(x, y) <= threshold 
						? Color.white 
						: Color.black;
					
					noiseThresholdTex.SetPixel(x, y, maskColor);
				}
			}
			
			noiseThresholdTex.Apply();
			NoiseThresholdImage.SetTexture(noiseThresholdTex);
		}
	}
}