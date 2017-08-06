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
            
        }

        private void Start() {
            var rend = GetComponentInChildren<Renderer>();
            _minPos = rend.bounds.min;

            _width = rend.bounds.size.x;
            _height = rend.bounds.size.z;
            var radius = DistributionConfig.GetRadius();

            _sampler = new PoissonDiscSampler(_width, _height, radius * DistributionConfig.RadiusFactor);

            GenerateNoise();


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

        private void GenerateNoise() {
            var watch = new Stopwatch();
            watch.Start();
            _noise = NoiseConfig.Type == NoiseType.Unity 
                ? UnityNoise((int) _width, (int) _height)
                : NoiseGenerator.PerlinNoise((int)_width, (int)_height, NoiseConfig.OctaveCount);
            watch.Stop();
            Debug.Log("Noise generation took " + watch.Elapsed.TotalSeconds.ToString("0.##") + "s");

            watch.Reset();
            watch.Start();
            UIController.RenderNoise(NoiseConfig.Type, _noise, NoiseConfig.Threshold);
            watch.Stop();
            Debug.Log("Noise rendering took " + watch.Elapsed.TotalSeconds.ToString("0.##") + "s");
        }

        // TODO: Move to NoiseGenerator
        private Grid<float> UnityNoise(int width, int height) {
            float xOrigin = Random.Range(0f, 1000f);
            float yOrigin = Random.Range(0f, 1000f);
            Grid<float> noise = new Grid<float>(width, height);

            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    noise.Set(x, y, Mathf.PerlinNoise((xOrigin + x) / 10f, (yOrigin + y) / 10f));
                }
            }

            return noise;
        }

        private float GetNoiseVal(Vector2 sample) {
            return _noise.Get((int)sample.x, (int)sample.y);
            //return NoiseConfig.Type == NoiseType.Unity
            //    ? Mathf.PerlinNoise(sample.x, sample.y)
            //    : _noise.Get((int)sample.x, (int)sample.y);
        }
    }
}
