using UnityEngine;

namespace DistributionPrototype.Config {
    [CreateAssetMenu(fileName = "ObjectDistributionConfig", menuName = "Prototype/Object Distribution Config")]
    public class ObjectDistributionConfig : ScriptableObject {
        public GameObject Prefab;
        public GameObject[] PrefabList;
        public Strategy DistributionStrategy;
        public float RadiusFactor = 1.5f;

        public static float GetRadius(GameObject prefab) {
            return prefab.GetComponentInChildren<SphereCollider>().radius;
        }

        public float GetLargestRadius() {
            float largest = 0f;

            foreach (var prefab in PrefabList) {
                var currentRadius = GetRadius(prefab);
                if (currentRadius > largest) largest = currentRadius;
            }

            return largest;
        }

        public enum Strategy {
            PoissonSampler,
            UniformPoissonSampler,
            NonUniformPoissonSampler
        }
    }
}