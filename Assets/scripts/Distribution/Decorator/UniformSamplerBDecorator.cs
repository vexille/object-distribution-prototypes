using DistributionPrototype.Distribution.Sampler;

namespace DistributionPrototype.Distribution.Decorator
{
	/// <summary>
	/// Decorator for a uniform poisson-disc sampler. Will produce a similar
	/// result as <see cref="UniformSamplerADecorator"/>, but with a different
	/// underlying algorhithm.
	/// </summary>
	public class UniformSamplerBDecorator : SamplerDecorator
	{
		private float _radius;

		public UniformSamplerBDecorator(float width, float height, float radius)
			: base(width, height)
		{
			_radius = radius;
		}

		public override int Generate(SampleGeneratedDelegate generationDelegate)
		{
			var sampler = new PoissonDiscSampler(_width, _height, _radius);

			var generated = 0;
			foreach (var sample in sampler.Samples())
			{
				generationDelegate(sample);
				generated++;
			}

			return generated;
		}
	}
}