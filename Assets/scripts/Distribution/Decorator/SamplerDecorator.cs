namespace DistributionPrototype.Distribution.Decorator
{
	/// <summary>
	/// Base class for sampler decorators implementing <see cref="ISamplerDecorator"/>.
	/// </summary>
	public abstract class SamplerDecorator : ISamplerDecorator
	{
		protected float _width;
		protected float _height;

		protected SamplerDecorator(float width, float height)
		{
			_width = width;
			_height = height;
		}

		public virtual void Prepare()
		{
		}

		public abstract int Generate(SampleGeneratedDelegate generationDelegate);
	}
}