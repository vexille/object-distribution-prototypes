using DistributionPrototype.Distribution.Sampler;
using UnityEngine;

namespace DistributionPrototype.Distribution.Decorator
{
	public class UniformSamplerDecorator : SamplerDecorator
	{
		private float _radius;
		private bool _newAlgorithm;

		public UniformSamplerDecorator(float width, float height, float radius, bool newAlgorithm)
			: base(width, height)
		{
			_radius = radius;
			_newAlgorithm = newAlgorithm;
		}

		public override int Generate(SampleGeneratedDelegate generationDelegate)
		{
			if (_newAlgorithm) return GenerateNew(generationDelegate);
			return GenerateOld(generationDelegate);
		}

		public int GenerateOld(SampleGeneratedDelegate generationDelegate)
		{
			var points = UniformPoissonDiskSampler.SampleRectangle(
				new Vector2(0f, 0f),
				new Vector2(_width, _height),
				_radius);

			foreach (var sample2f in points)
			{
				var sample = new Vector2(sample2f.x, sample2f.y);
				generationDelegate(sample);
			}

			return points.Count;
		}

		public int GenerateNew(SampleGeneratedDelegate generationDelegate)
		{
			var _sampler = new PoissonDiscSampler(_width, _height, _radius);

			var generated = 0;
			foreach (var sample in _sampler.Samples())
			{
				generationDelegate(sample);
				generated++;
			}

			return generated;
		}
	}
}