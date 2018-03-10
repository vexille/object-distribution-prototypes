using UnityEngine;

namespace DistributionPrototype.Config
{
	[CreateAssetMenu(fileName = "ObjectDistributionConfig", menuName = "Prototype/Object Distribution Config")]
	public class ObjectDistributionConfig : ScriptableObject
	{
		public enum Strategy
		{
			UniformPoissonSamplerA,
			UniformPoissonSamplerB,
			NonUniformPoissonSampler
		}
		
		public GameObject[] PrefabList;
		public Strategy DistributionStrategy;
		public float RadiusFactor = 1.5f;
		public bool NoiseLimitedSpawn;
		[Range(0f, 1f)]
		public float SpawnThreshold = 0.5f;

		/// <summary>
		/// Gets the largest radius from the objects in <see cref="PrefabList"/>.
		/// </summary>
		/// <returns>The largest radius found in the list</returns>
		public float GetLargestRadius()
		{
			float largest = 0f;

			foreach (var prefab in PrefabList)
			{
				var currentRadius = GetRadius(prefab);
				if (currentRadius > largest) largest = currentRadius;
			}

			return largest;
		}

		/// <summary>
		/// Gets a random prefab from <see cref="PrefabList"/>.
		/// </summary>
		/// <returns>The selected prefab from the list</returns>
		public GameObject GetRandomPrefab()
		{
			return PrefabList[Random.Range(0, PrefabList.Length - 1)];
		}

		private static float GetRadius(GameObject prefab)
		{
			var sphereCollider = prefab.GetComponentInChildren<SphereCollider>();
			if (sphereCollider != null)
			{
				return sphereCollider.radius;
			}

			var meshCollider = prefab.GetComponentInChildren<MeshRenderer>();
			return Mathf.Max(meshCollider.bounds.extents.x, meshCollider.bounds.extents.z);
		}
	}
}