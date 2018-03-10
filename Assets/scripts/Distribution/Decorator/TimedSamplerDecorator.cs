using System.Diagnostics;

namespace DistributionPrototype.Distribution.Decorator
{
	/// <summary>
	/// Measures the elapsed time the underlying delegate spends on both
	/// <see cref="ISamplerDecorator.Prepare"/> and <see cref="ISamplerDecorator.Generate"/>.
	/// The resulting times will be provided through <see cref="PrepareTimeDelegate"/> and
	/// <see cref="GenerateTimeDelegate"/>.
	/// </summary>
	public class TimedSamplerDecorator : ISamplerDecorator
	{
		public delegate void PrepareTimeDelegate(double elapsed);
		public delegate void GenerateTimeDelegate(double elapsed);

		private readonly ISamplerDecorator _decorator;
		private readonly PrepareTimeDelegate _prepareTimeDelegate;
		private readonly GenerateTimeDelegate _generateTimeDelegate;

		public TimedSamplerDecorator(ISamplerDecorator decorator, 
			PrepareTimeDelegate prepareTimeDelegate, 
			GenerateTimeDelegate generateTimeDelegate)
		{
			_decorator = decorator;
			_prepareTimeDelegate = prepareTimeDelegate;
			_generateTimeDelegate = generateTimeDelegate;
		}

		public void Prepare()
		{
			var watch = new Stopwatch();
			watch.Start();

			_decorator.Prepare();

			watch.Stop();

			_prepareTimeDelegate(watch.Elapsed.TotalSeconds);
		}

		public int Generate(SampleGeneratedDelegate generationDelegate)
		{
			var watch = new Stopwatch();
			watch.Start();

			int result = _decorator.Generate(generationDelegate);

			watch.Stop();

			_generateTimeDelegate(watch.Elapsed.TotalSeconds);

			return result;
		}
	}
}