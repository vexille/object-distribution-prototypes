using DistributionPrototype.Util;
using Zenject;

namespace DistributionPrototype.Signals
{
	/// <summary>
	/// Signals that noise was generated for limited sampling.
	/// 
	/// Parameters:
	/// Grid2D&lt;float&gt; - Generated noise
	/// float				- threshold
	/// </summary>
	public class LimiterNoiseGeneratedSignal
		: Signal<LimiterNoiseGeneratedSignal, Grid2D<float>, float>
	{ }
}
