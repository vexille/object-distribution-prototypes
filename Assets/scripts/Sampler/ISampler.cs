using System.Collections.Generic;
using UnityEngine;

namespace DistributionPrototype
{
	public interface ISampler
	{
		IEnumerable<Vector2> Samples();
	}
}