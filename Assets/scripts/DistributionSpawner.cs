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

            GenerateNoise();

            switch (DistributionConfig.DistributionStrategy) {
                case ObjectDistributionConfig.Strategy.PoissonSampler:
                    GeneratePoisson(radius);
                    break;
                case ObjectDistributionConfig.Strategy.UniformPoissonSampler:
                    GenerateUniformPoisson(rend.bounds, radius);
                    break;
                case ObjectDistributionConfig.Strategy.NonUniformPoissonSampler:
                    GenerateNonUniformPoisson(rend.bounds, radius);
                    break;
                default:
                    break;
            }
        }

        private void GenerateUniformPoisson(Bounds bounds, float radius) {
            var watch = new Stopwatch();
            watch.Start();

            var points = UniformPoissonDiskSampler.SampleRectangle(
                new Vector2f(0f, 0f),
                new Vector2f(_width, _height),
                radius * DistributionConfig.RadiusFactor);

            var spawned = 0;
            
            foreach (var sample2f in points) {
                var sample = new Vector2(sample2f.x, sample2f.y);
                if (SpawnSample(sample)) spawned++;
            }
            watch.Stop();

            Debug.Log("Spawned " + spawned + " entities, took " + watch.Elapsed.TotalSeconds.ToString("0.##") + "s");
        }

        private void GenerateNonUniformPoisson(Bounds bounds, float radius) {
            radius *= DistributionConfig.RadiusFactor;
            var watch = new Stopwatch();
            watch.Start();
            var min = 100f;
            Grid<float> distances = new Grid<float>(_noise.Width, _noise.Height);
            for (int i = 0; i < _noise.Count; i++) {
                var val = Mathf.Lerp(radius * 1.1f, radius * 2f, _noise.Get(i));
                distances.Set(i, val);
                if (val < min) min = val;
            }
            watch.Stop();
            Debug.Log("Adjusted " + _noise.Count + " noise values, took " + watch.Elapsed.TotalSeconds.ToString("0.##") + "s" + ", min was " + min +", with radius " + radius);
            watch.Reset();
            watch.Start();
            
            var points = NonUniformPoissonDiskSampler.SampleRectangle(
                new Vector2f(0f, 0f),
                new Vector2f(_width, _height),
                distances);

            var spawned = 0;
            foreach (var sample2f in points) {
                var sample = new Vector2(sample2f.x, sample2f.y);
                if (SpawnSample(sample, true, true)) spawned++;
            }
            watch.Stop();
            Debug.Log("Spawned " + spawned + " entities, took " + watch.Elapsed.TotalSeconds.ToString("0.##") + "s");
        }

        private void GeneratePoisson(float radius) {
            _sampler = new PoissonDiscSampler(_width, _height, radius * DistributionConfig.RadiusFactor);

            var spawned = 0;
            var watch = new Stopwatch();
            watch.Start();
            foreach (var sample in _sampler.Samples()) {
                if (SpawnSample(sample)) spawned++;
            }
            watch.Stop();

            Debug.Log("Spawned " + spawned + " entities, took " + watch.Elapsed.TotalSeconds.ToString("0.##") + "s");
        }

        private bool SpawnSample(Vector2 sample, bool basedOnNoise = true, bool adHoc = false) {
            if (basedOnNoise) {
                var noiseVal = adHoc ? Mathf.PerlinNoise(sample.x, sample.y) : GetNoiseVal(sample);
                if (noiseVal > NoiseConfig.Threshold) return false;
            }

            var pos = sample.ToVector3();

            Object.Instantiate(DistributionConfig.Prefab, pos + _minPos, Quaternion.identity);
            return true;
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
