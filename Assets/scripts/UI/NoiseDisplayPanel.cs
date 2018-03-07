using DistributionPrototype.Messages;
using DistributionPrototype.Util;
using Frictionless;
using UnityEngine;
using UnityEngine.UI;

namespace DistributionPrototype.UI
{
	public class NoiseDisplayPanel : MonoBehaviour
	{
		[SerializeField] private Image _noiseImage;
		[SerializeField] private Image _noiseThresholdImage;
		[SerializeField] private GameObject _noNoiseGameObject;

		private void Awake()
		{
			var messageRounter = ServiceFactory.Instance.Resolve<MessageRouter>();
			messageRounter.AddHandler<GenerationStartedMessage>(message =>
			{
				ResetVisualization();
			});

			messageRounter.AddHandler<SamplerNoiseGeneratedMessage>(message =>
			{
				_noNoiseGameObject.SetActive(false);
				_noiseImage.gameObject.SetActive(true);
				RenderNoise(message.Noise);
			});

			messageRounter.AddHandler<LimiterNoiseGeneratedMessage>(message =>
			{
				_noNoiseGameObject.SetActive(false);
				_noiseThresholdImage.gameObject.SetActive(true);
				RenderThreshold(message.Noise, message.Threshold);
			});
		}

		private void ResetVisualization()
		{
			// Deactivate both images at start so only the used noises are active
			_noiseImage.gameObject.SetActive(false);
			_noiseThresholdImage.gameObject.SetActive(false);

			// By default show the "no noise used" feedback
			_noNoiseGameObject.SetActive(true);
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
			_noiseImage.SetTexture(noiseTex);
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
			_noiseThresholdImage.SetTexture(noiseThresholdTex);
		}
	}
}