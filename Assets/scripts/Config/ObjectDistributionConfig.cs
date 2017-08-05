using UnityEngine;

namespace DistributionPrototype.Config {
    [CreateAssetMenu(fileName = "ObjectDistributionConfig", menuName = "Prototype/Object Distribution Config")]
    public class ObjectDistributionConfig : ScriptableObject {
        public GameObject Prefab;
        public float RadiusFactor = 1.5f;

        public float GetRadius() {
            return Prefab.GetComponentInChildren<SphereCollider>().radius;
        }
    }
}