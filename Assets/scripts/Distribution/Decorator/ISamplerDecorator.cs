using UnityEngine;

namespace DistributionPrototype.Distribution.Decorator
{
	/// <summary>
	/// Delegate to be called for every sampled point
	/// </summary>
	/// <param name="sample">Vector2 position of the sampled point</param>
	public delegate void SampleGeneratedDelegate(Vector2 sample);

	/// <summary>
	/// Interface for decorators that encapsulates a distribution sampler.
	/// </summary>
	public interface ISamplerDecorator
	{
		void Prepare();

		/// <summary>
		/// Generates a set of sampled points using the underlying implementation.
		/// </summary>
		/// <param name="generationDelegate">
		/// Callback fired for every sampled point
		/// </param>
		/// <returns>Count of generated points</returns>
		int Generate(SampleGeneratedDelegate generationDelegate);
	}
}