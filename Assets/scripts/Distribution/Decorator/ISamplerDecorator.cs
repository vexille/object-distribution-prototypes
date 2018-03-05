using UnityEngine;

namespace DistributionPrototype.Distribution.Decorator
{
	// TODO: Doc
	public delegate void SampleGeneratedDelegate(Vector2 sample);

	public interface ISamplerDecorator
	{
		void Prepare(object data);

		int Generate(SampleGeneratedDelegate generationDelegate);
	}
}