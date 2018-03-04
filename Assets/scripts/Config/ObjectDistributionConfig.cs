using UnityEngine;

namespace DistributionPrototype.Config
{
	[CreateAssetMenu(fileName = "ObjectDistributionConfig", menuName = "Prototype/Object Distribution Config")]
	public class ObjectDistributionConfig : ScriptableObject
	{
		public GameObject Prefab;
		public GameObject[] PrefabList;
		public Strategy DistributionStrategy;
		public float RadiusFactor = 1.5f;

		public static float GetRadius(GameObject prefab)
		{
			var sphereCollider = prefab.GetComponentInChildren<SphereCollider>();
			if (sphereCollider != null)
			{
				return sphereCollider.radius;
			}

			var meshCollider = prefab.GetComponentInChildren<MeshRenderer>();
			return Mathf.Max(meshCollider.bounds.extents.x, meshCollider.bounds.extents.z);
		}

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

		public GameObject GetRandomPrefab()
		{
			return PrefabList[Random.Range(0, PrefabList.Length - 1)];
		}

		public enum Strategy
		{
			PoissonSampler,
			UniformPoissonSampler,
			NonUniformPoissonSampler
		}
	}
}