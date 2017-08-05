using DistributionPrototype.Config;
using DistributionPrototype.Sampler;
using DistributionPrototype.UI;
using LuftSchloss;
using LuftSchloss.Util;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace DistributionPrototype {
    public class DistributionSpawner : MonoBehaviour {

        public NoiseConfig NoiseConfig;
        public ObjectDistributionConfig DistributionConfig;
        public UIController UIController;

        private PoissonDiscSampler _sampler;

        private Vector3 _minPos;
        private float _width;
        private float _height;
        private Grid<float> _noise;

        private void Awake() {
            var rend = GetComponentInChildren<Renderer>();
            _minPos = rend.bounds.min;

            _width = rend.bounds.size.x;
            _height = rend.bounds.size.z;
            var radius = DistributionConfig.GetRadius();

            _sampler = new PoissonDiscSampler(_width, _height, radius * DistributionConfig.RadiusFactor);

            GenerateCustomNoise();
        }

        private void Start() {
            var spawned = 0;
            var watch = new Stopwatch();
            watch.Start();
            foreach (var sample in _sampler.Samples()) {
                var noiseVal = GetNoiseVal(sample);
                if (noiseVal > NoiseConfig.Threshold) continue;

                var pos = sample.ToVector3();

                Object.Instantiate(DistributionConfig.Prefab, pos + _minPos, Quaternion.identity);
                spawned++;
            }
            watch.Stop();

            Debug.Log("Spawned " + spawned + " entities, took " + watch.Elapsed.TotalSeconds.ToString("0.##") + "s");
        }

        private void GenerateCustomNoise() {
            var watch = new Stopwatch();
            watch.Start();
            _noise = NoiseGenerator.PerlinNoise((int)_width, (int)_height, NoiseConfig.OctaveCount);
            watch.Stop();
            Debug.Log("Noise generation took " + watch.Elapsed.TotalSeconds.ToString("0.##") + "s");

            watch.Reset();
            watch.Start();
            UIController.RenderNoise(_noise, NoiseConfig.Threshold);
            watch.Stop();
            Debug.Log("Noise rendering took " + watch.Elapsed.TotalSeconds.ToString("0.##") + "s");
        }

        private float GetNoiseVal(Vector2 sample) {
            return NoiseConfig.Type == NoiseType.Unity
                ? Mathf.PerlinNoise(sample.x, sample.y)
                : _noise.Get((int)sample.x, (int)sample.y);
        }
    }
}
