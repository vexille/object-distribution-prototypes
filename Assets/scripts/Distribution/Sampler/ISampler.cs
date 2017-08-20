using System.Collections.Generic;
using UnityEngine;

namespace DistributionPrototype.Distribution.Sampler
{
	public interface ISampler {
		IEnumerable<Vector2> Samples();
	}
}