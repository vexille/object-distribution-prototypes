using UnityEngine;

namespace DistributionPrototype.Distribution.Decorator
{
	public delegate void SampleGeneratedDelegate(Vector2 sample);

	public interface ISamplerDecorator
	{
		void Prepare(object data);

		int Generate(SampleGeneratedDelegate generationDelegate);
	}
}