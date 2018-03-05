using DistributionPrototype.Config;
using DistributionPrototype.Util;

namespace DistributionPrototype.Messages
{
	public class NoiseGeneratedMessage
	{
		public NoiseType Type;
		public Grid2D<float> Noise;
		public float Threshold;
	}
}
