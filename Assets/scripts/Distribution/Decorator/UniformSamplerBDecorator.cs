using DistributionPrototype.Distribution.Sampler;

namespace DistributionPrototype.Distribution.Decorator
{
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