using DistributionPrototype.Config;
using DistributionPrototype.Distribution.Decorator;
using DistributionPrototype.Messages;
using DistributionPrototype.Util;
using Frictionless;
using UnityEngine;

namespace DistributionPrototype.Distribution
{
	public class SamplerDecoratorFactory
	{
		private readonly MessageRouter _messageRouter;
		private readonly NoiseConfig _noiseConfig;
		private readonly ObjectDistributionConfig _distributionConfig;
		private float _width;
		private float _height;

		public bool DebugPerformance { get; set; }

		public SamplerDecoratorFactory(
			NoiseConfig noiseConfig,
			ObjectDistributionConfig distributionConfig)
		{
			_noiseConfig = noiseConfig;
			_distributionConfig = distributionConfig;

			_messageRouter = ServiceFactory.Instance.Resolve<MessageRouter>();
		}

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

			return new LimitedSpawnSamplerDecorator(
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
