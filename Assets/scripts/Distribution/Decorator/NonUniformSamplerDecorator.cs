using DistributionPrototype.Distribution.Sampler;
using DistributionPrototype.Util;
using UnityEngine;

namespace DistributionPrototype.Distribution.Decorator
{
	/// <summary>
	/// Decorator for a non-uniform poisson-disc sampler. This sampler uses 
	/// a noise grid to drive the distances between the sampled points.
	/// </summary>
	public class NonUniformSamplerDecorator : SamplerDecorator
	{
		private readonly float _radius;
		private readonly Grid2D<float> _distanceNoise;
		private Grid2D<float> _distances;

		public NonUniformSamplerDecorator(float width, float height, float radius, 
			Grid2D<float> distanceNoise) : base(width, height)
		{
			_radius = radius;
			_distanceNoise = distanceNoise;
		}

		public override void Prepare()
		{
			base.Prepare();

			_distances = new Grid2D<float>(_distanceNoise.Width, _distanceNoise.Height);
			for (int i = 0; i < _distanceNoise.Count; i++)
			{
				// Calculates the current distance value based on the given radius
				var distanceValue = Mathf.Lerp(
					_radius * 1.1f,
					_radius * 2.8f,
					_distanceNoise.Get(i));

				_distances.Set(i, distanceValue);
			}
		}

		public override int Generate(SampleGeneratedDelegate generationDelegate)
		{
			var points = NonUniformPoissonDiskSampler.SampleRectangle(
				new Vector2(0f, 0f),
				new Vector2(_width, _height),
				_distances);

			foreach (Vector2 sample in points)
			{
				generationDelegate(sample);
			}

			return points.Count;
		}
	}
}