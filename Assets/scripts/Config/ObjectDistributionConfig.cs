using UnityEngine;

namespace DistributionPrototype.Config {
    [CreateAssetMenu(fileName = "ObjectDistributionConfig", menuName = "Prototype/Object Distribution Config")]
    public class ObjectDistributionConfig : ScriptableObject {
        public GameObject Prefab;
        public Strategy DistributionStrategy;
        public float RadiusFactor = 1.5f;

        public float GetRadius() {
            return Prefab.GetComponentInChildren<SphereCollider>().radius;
        }

        public enum Strategy {
            PoissonSampler,
            UniformPoissonSampler,
            NonUniformPoissonSampler
        }
    }
}