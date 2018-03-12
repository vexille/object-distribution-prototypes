using DistributionPrototype.Config;
using DistributionPrototype.Distribution.Decorator;
using DistributionPrototype.Signals;
using DistributionPrototype.Util;
using UnityEngine;
using Zenject;

namespace DistributionPrototype.Distribution
{
	/// <summary>
	/// Factory for sampler decorators that will encapsulate the underlying
	/// sampling algorithm. Works based on configuration data from
	/// <see cref="NoiseConfig"/> and <see cref="ObjectDistributionConfig"/>.
	/// </summary>
	public class SamplerDecoratorFactory
	{
		private SamplerNoiseGeneratedSignal _samplerNoiseGeneratedSignal;
		private LimiterNoiseGeneratedSignal _limiterNoiseGeneratedSignal;

		private readonly ConfigFacade _configFacade;
		private float _width;
		private float _height;
		
		/// <summary>
		/// flag that will enable performance logging.
		/// </summary>
		private bool _debugPerformance;

		/// <summary>
		/// Creates new factory with references to configuration data.
		/// </summary>
		public SamplerDecoratorFactory(
			ConfigFacade configFacade,
			[Inject(Id = "debugPerformance")] bool debugPerformance)
		{
			_configFacade = configFacade;
			_debugPerformance = debugPerformance;
		}

		[Inject]
		public void SetSignals(
			SamplerNoiseGeneratedSignal samplerNoiseGeneratedSignal,
			LimiterNoiseGeneratedSignal limiterNoiseGeneratedSignal)
		{
			_samplerNoiseGeneratedSignal = samplerNoiseGeneratedSignal;
			_limiterNoiseGeneratedSignal = limiterNoiseGeneratedSignal;
		}

		/// <summary>
		/// Creates a new sampler that will act on the given dimensions. The new
		/// sampler will be created using the current config data.
		/// </summary>
		/// <param name="width">Width of the spawn area</param>
		/// <param name="height">Height of the spawn area</param>
		/// <returns>The newly created sampler decorator</returns>
		public ISamplerDecorator GetSamplerDecorator(float width, float height)
		{
			_width = width;
			_height = height;

			ISamplerDecorator sampler = GetRootSampler();
			sampler = SetupNoiseLimitedDecorator(sampler);
			sampler = SetupTimedDecorator(sampler);
			
			return sampler;
		}

		private ISamplerDecorator GetRootSampler()
		{
			var distributionConfig = _configFacade.DistributionConfig;
			float radius = distributionConfig.GetLargestRadius() * distributionConfig.RadiusFactor;

			switch (distributionConfig.DistributionStrategy)
			{
				case ObjectDistributionConfig.Strategy.UniformPoissonSamplerA:
					return new UniformSamplerADecorator(_width, _height, radius);

				case ObjectDistributionConfig.Strategy.UniformPoissonSamplerB:
					return new UniformSamplerADecorator(_width, _height, radius);

				case ObjectDistributionConfig.Strategy.NonUniformPoissonSampler:
					Grid2D<float> noise = GenerateNoise();
					
					_samplerNoiseGeneratedSignal.Fire(noise);

					return new NonUniformSamplerDecorator(_width, _height, radius, noise);

				default:
					throw new System.ArgumentException("Unsupported distribution strategy");
			}
		}

		private ISamplerDecorator SetupNoiseLimitedDecorator(ISamplerDecorator sampler)
		{
			var distributionConfig = _configFacade.DistributionConfig;
			if (!distributionConfig.NoiseLimitedSpawn)
			{
				return sampler;
			}

			Grid2D<float> spawnNoise = GenerateNoise();

			_limiterNoiseGeneratedSignal.Fire(spawnNoise, distributionConfig.SpawnThreshold);

			return new NoiseLimitedSamplerDecorator(
				sampler,
				spawnNoise,
				distributionConfig.SpawnThreshold);
		}

		private ISamplerDecorator SetupTimedDecorator(ISamplerDecorator sampler)
		{
			if (!_debugPerformance)
			{
				return sampler;
			}

			return new TimedSamplerDecorator(
				sampler,
				elapsed => Debug.Log("Sampler preparation took " + elapsed.ToString("0.##") + " seconds"),
				elapsed => Debug.Log("Sampler execution took " + elapsed.ToString("0.##") + " seconds"));
		}

		private Grid2D<float> GenerateNoise()
		{
			return _configFacade.NoiseConfig.Type == NoiseType.Unity
				? NoiseGenerator.UnityNoise ((int)_width, (int)_height)
				: NoiseGenerator.PerlinNoise((int)_width, (int)_height,
					_configFacade.NoiseConfig.OctaveCount);
		}
	}
}
