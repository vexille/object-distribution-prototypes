using DistributionPrototype.Distribution.Sampler;
using DistributionPrototype.Util;
using UnityEngine;

namespace DistributionPrototype.Distribution.Decorator {
    public class NonUniformSamplerDecorator : SamplerDecorator {
        private float _radius;
        private bool _newAlgorithm;
        private Grid2D<float> _distances;
        private Grid2D<float> _distanceNoise;

        public NonUniformSamplerDecorator(float width, float height, float radius, Grid2D<float> distanceNoise)
            : base(width, height) {
                _radius = radius;
            _distanceNoise = distanceNoise;
        }

        public override void Prepare(object data) {
            base.Prepare(data);

            _distances = new Grid2D<float>(_distanceNoise.Width, _distanceNoise.Height);
            for (int i = 0; i < _distanceNoise.Count; i++) {
                var val = Mathf.Lerp(_radius * 1.1f, _radius * 2f, _distanceNoise.Get(i));
                _distances.Set(i, val);
            }
        }

        public override int Generate(SampleGeneratedDelegate generationDelegate) {
            var points = NonUniformPoissonDiskSampler.SampleRectangle(
                new Vector2(0f, 0f),
                new Vector2(_width, _height),
                _distances);

            foreach (var sample2f in points) {
                var sample = new Vector2(sample2f.x, sample2f.y);
                generationDelegate(sample);
            }

            return points.Count;
        }
    }
}
