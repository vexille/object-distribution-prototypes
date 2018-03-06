using UnityEngine;

namespace DistributionPrototype.Config
{
	[CreateAssetMenu(fileName = "NoiseConfig", menuName = "Prototype/Noise Config")]
	public class NoiseConfig : ScriptableObject
	{
		public NoiseType Type;
		
		[Range(1, 15)]
		public int OctaveCount = 6;
	}
}