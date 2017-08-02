using DistributionPrototype;
using LuftSchloss;
using LuftSchloss.Util;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public enum NoiseType {
    Unity,
    Custom
}

public class DistributionSpawner : MonoBehaviour {
    public GameObject ObjectPrefab;
    public float RadiusFactor = 1.5f;

    public NoiseType NoiseType;

    [Range(0f, 1f)]
    public float ObjectSpawnThreshold = 0.5f;

    [Range(1, 10)]
    public int NoiseOctaveCount = 6;

    private PoissonDiscSampler _sampler;

    private Vector3 _minPos;
    private Grid<float> _noise;

    private void Awake() {
        var rend = GetComponentInChildren<Renderer>();
        _minPos = rend.bounds.min;

        var width = rend.bounds.size.x;
        var height = rend.bounds.size.z;
        var radius = ObjectPrefab.GetComponentInChildren<SphereCollider>().radius;

        _sampler = new PoissonDiscSampler(width, height, radius * RadiusFactor);

        var watch = new Stopwatch();
        watch.Start();
        _noise = NoiseGenerator.PerlinNoise((int) width, (int) height, NoiseOctaveCount);
        watch.Stop();
        Debug.Log("Noise generation took " + watch.Elapsed.TotalSeconds.ToString("0.##") + "s");
    }

    private void Start() {
        var spawned = 0;
        var watch = new Stopwatch();
        watch.Start();
        foreach (var sample in _sampler.Samples()) {
            var noiseVal = GetNoiseVal(sample);
            if (noiseVal > ObjectSpawnThreshold) continue;

            var pos = sample.ToVector3();
            
            Object.Instantiate(ObjectPrefab, pos + _minPos, Quaternion.identity);
            spawned++;
        }
        watch.Stop();

        Debug.Log("Spawned " + spawned + " entities, took " + watch.Elapsed.TotalSeconds.ToString("0.##") + "s");
    }

    private float GetNoiseVal(Vector2 sample) {
        return NoiseType == NoiseType.Unity 
            ? Mathf.PerlinNoise(sample.x, sample.y) 
            : _noise.Get((int)sample.x, (int)sample.y);
    }
}

public static class VecExtensions {
    public static Vector3 ToVector3(this Vector2 vec) {
        return new Vector3(vec.x, 0f, vec.y);
    }
}