using DistributionPrototype.Util;
using Zenject;

namespace DistributionPrototype.Signals
{
	/// <summary>
	/// Signals that noise was generated for a sampler.
	/// 
	/// Parameters:
	/// Grid2D&lt;float&gt; - Generated noise
	/// </summary>
	public class SamplerNoiseGeneratedSignal
		: Signal<SamplerNoiseGeneratedSignal, Grid2D<float>>
	{ }
}
