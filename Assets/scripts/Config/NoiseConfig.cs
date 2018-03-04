﻿using UnityEngine;

namespace DistributionPrototype.Config {
    [CreateAssetMenu(fileName = "NoiseConfig", menuName = "Prototype/Noise Config")]
    public class NoiseConfig : ScriptableObject {
        public NoiseType Type;

        [Range(0f, 1f)]
        public float Threshold = 0.5f;

        [Range(1, 15)]
        public int OctaveCount = 6;
    }
}