using DistributionPrototype.Config;
using DistributionPrototype.Distribution.Decorator;
using DistributionPrototype.Messages;
using DistributionPrototype.Util;
using Frictionless;
using UnityEngine;

namespace DistributionPrototype.Distribution
{
	/// <summary>
	/// Factory for sampler decorators that will encapsulate the underlying
	/// sampling algorithm. Works based on configuration data from
	/// <see cref="NoiseConfig"/> and <see cref="ObjectDistributionConfig"/>.
	/// </summary>
	public class SamplerDecoratorFactory
	{
		private readonly MessageRouter _messageRouter;
		private readonly NoiseConfig _noiseConfig;
		private readonly ObjectDistributionConfig _distributionConfig;
		private float _width;
		private float _height;
		
		/// <summary>
		/// Gets and sets flag that will enable performance logging.
		/// </summary>
		public bool DebugPerformance { get; set; }

		/// <summary>
		/// Creates new factory with references to configuration data.
		/// </summary>
		/// <param name="noiseConfig"></param>
		/// <param name="distributionConfig"></param>
		public SamplerDecoratorFactory(
			NoiseConfig noiseConfig,
			ObjectDistributionConfig distributionConfig)
		{
			_noiseConfig = noiseConfig;
			_distributionConfig = distributionConfig;

			_messageRouter = ServiceFactory.Instance.Resolve<MessageRouter>();
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
			float radius = _distributionConfig.GetLargestRadius() * _distributionConfig.RadiusFactor;

			switch (_distributionConfig.DistributionStrategy)
			{
				case ObjectDistributionConfig.Strategy.UniformPoissonSamplerA:
					return new UniformSamplerADecorator(_width, _height, radius);

				case ObjectDistributionConfig.Strategy.UniformPoissonSamplerB:
					return new UniformSamplerADecorator(_width, _height, radius);

				case ObjectDistributionConfig.Strategy.NonUniformPoissonSampler:
					Grid2D<float> noise = GenerateNoise();

					_messageRouter.RaiseMessage(
						new SamplerNoiseGeneratedMessage { Noise = noise });

					return new NonUniformSamplerDecorator(_width, _height, radius, noise);

				default:
					throw new System.ArgumentException("Unsupported distribution strategy");
			}
		}

		private ISamplerDecorator SetupNoiseLimitedDecorator(ISamplerDecorator sampler)
		{
			if (!_distributionConfig.NoiseLimitedSpawn)
			{
				return sampler;
			}

			Grid2D<float> spawnNoise = GenerateNoise();

			_messageRouter.RaiseMessage(
				new LimiterNoiseGeneratedMessage
				{
					Noise = spawnNoise,
					Threshold = _distributionConfig.SpawnThreshold
				});

			return new NoiseLimitedSamplerDecorator(
				sampler,
				spawnNoise,
				_distributionConfig.SpawnThreshold);
		}

		private ISamplerDecorator SetupTimedDecorator(ISamplerDecorator sampler)
		{
			if (!DebugPerformance)
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
			return _noiseConfig.Type == NoiseType.Unity
				? NoiseGenerator.UnityNoise ((int)_width, (int)_height)
				: NoiseGenerator.PerlinNoise((int)_width, (int)_height, 
					_noiseConfig.OctaveCount);
		}
	}
}
