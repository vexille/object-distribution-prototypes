using UnityEngine;

namespace DistributionPrototype {
    public static class ExtensionMethods {
        public static Vector3 ToVector3(this Vector2 vec) {
            return new Vector3(vec.x, 0f, vec.y);
        }
    }
}
