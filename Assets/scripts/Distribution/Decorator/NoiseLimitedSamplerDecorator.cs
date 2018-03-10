using DistributionPrototype.Util;
using UnityEngine;

namespace DistributionPrototype.Distribution.Decorator
{
	/// <summary>
	/// Limits the output of sampled points based on a noise grid and
	/// a threshold values. For every sample, the corresponding noise value
	/// is checked against the threshold so that only the ones with a
	/// smaller value are passed to the <see cref="SampleGeneratedDelegate"/>.
	/// </summary>
	public class NoiseLimitedSamplerDecorator : ISamplerDecorator
	{
		private readonly ISamplerDecorator _decorator;
		private readonly Grid2D<float> _noise;
		private readonly float _spawnThreshold;

		public NoiseLimitedSamplerDecorator(ISamplerDecorator decorator, 
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
