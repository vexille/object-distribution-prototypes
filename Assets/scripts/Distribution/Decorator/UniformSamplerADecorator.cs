using DistributionPrototype.Distribution.Sampler;
using UnityEngine;

namespace DistributionPrototype.Distribution.Decorator
{
	/// <summary>
	/// Decorator for a uniform poisson-disc sampler. Will produce a similar
	/// result as <see cref="UniformSamplerBDecorator"/>, but with a different
	/// underlying algorithm.
	/// </summary>
	public class UniformSamplerADecorator : SamplerDecorator
	{
		private float _radius;

		public UniformSamplerADecorator(float width, float height, float radius)
			: base(width, height)
		{
			_radius = radius;
		}

		public override int Generate(SampleGeneratedDelegate generationDelegate)
		{
			var points = UniformPoissonDiskSampler.SampleRectangle(
				new Vector2(0f, 0f),
				new Vector2(_width, _height),
				_radius);

			foreach (Vector2 sample in points)
			{
				generationDelegate(sample);
			}

			return points.Count;
		}
	}
}