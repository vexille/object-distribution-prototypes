using System.Collections.Generic;
using UnityEngine;

namespace DistributionPrototype.Sampler
{
	public interface ISampler {
		IEnumerable<Vector2> Samples();
	}
}