using System;
using DistributionPrototype.Signals;
using DistributionPrototype.Util;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace DistributionPrototype.UI
{
	public class NoiseDisplayPanel : MonoBehaviour, IDisposable
	{
		[SerializeField] private Image _noiseImage;
		[SerializeField] private Image _noiseThresholdImage;
		[SerializeField] private GameObject _noNoiseGameObject;

		private GenerationStartedSignal _generationStartedSignal;
		private SamplerNoiseGeneratedSignal _samplerNoiseGeneratedSignal;
		private LimiterNoiseGeneratedSignal _limiterNoiseGeneratedSignal;

		[Inject]
		public void SetSignals(
			GenerationStartedSignal generationStartedSignal,
			SamplerNoiseGeneratedSignal samplerNoiseGeneratedSignal,
			LimiterNoiseGeneratedSignal limiterNoiseGeneratedSignal)
		{
			_generationStartedSignal = generationStartedSignal;
			_samplerNoiseGeneratedSignal = samplerNoiseGeneratedSignal;
			_limiterNoiseGeneratedSignal = limiterNoiseGeneratedSignal;

			_generationStartedSignal.Listen(OnGenerationStarted);
			_samplerNoiseGeneratedSignal.Listen(OnSamplerNoiseGenerated);
			_limiterNoiseGeneratedSignal.Listen(OnLimiterNoiseGenerated);
		}

		public void Dispose()
		{
			_generationStartedSignal.Unlisten(OnGenerationStarted);
			_samplerNoiseGeneratedSignal.Unlisten(OnSamplerNoiseGenerated);
			_limiterNoiseGeneratedSignal.Unlisten(OnLimiterNoiseGenerated);
		}

		public void OnGenerationStarted()
		{
			ResetVisualization();
		}

		public void OnSamplerNoiseGenerated(Grid2D<float> noise)
		{
			_noNoiseGameObject.SetActive(false);
			_noiseImage.gameObject.SetActive(true);
			RenderNoise(noise);
		}

		public void OnLimiterNoiseGenerated(Grid2D<float> noise, float threshold)
		{
			_noNoiseGameObject.SetActive(false);
			_noiseThresholdImage.gameObject.SetActive(true);
			RenderThreshold(noise, threshold);
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