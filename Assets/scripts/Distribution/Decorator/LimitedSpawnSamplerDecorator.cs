using DistributionPrototype.Util;
using UnityEngine;

namespace DistributionPrototype.Distribution.Decorator
{
	public class LimitedSpawnSamplerDecorator : ISamplerDecorator
	{
		private readonly ISamplerDecorator _decorator;
		private readonly Grid2D<float> _noise;
		private readonly float _spawnThreshold;

		public LimitedSpawnSamplerDecorator(ISamplerDecorator decorator, 
			Grid2D<float> noise, float spawnThreshold)
		{
			_decorator = decorator;
			_noise = noise;
			_spawnThreshold = spawnThreshold;
		}

		public void Prepare()
		{
			_decorator.Prepare();
		}

		public int Generate(SampleGeneratedDelegate generationDelegate)
		{
			int spawnCount = 0;
			_decorator.Generate(delegate(Vector2 sample)
			{
				float value = _noise.Get((int)sample.x, (int)sample.y);
				if (value <= _spawnThreshold)
				{
					generationDelegate(sample);
					spawnCount++;
				}
			});

			return spawnCount;
		}
	}
}
