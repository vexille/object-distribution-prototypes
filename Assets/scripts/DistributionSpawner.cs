using DistributionPrototype.Config;
using DistributionPrototype.Distribution.Decorator;
using DistributionPrototype.Distribution.Sampler;
using DistributionPrototype.UI;
using System.Collections.Generic;
using System.Diagnostics;
using DistributionPrototype.Util;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace DistributionPrototype.Distribution
{
	public class DistributionSpawner : MonoBehaviour
	{
		public NoiseConfig NoiseConfig;
		public ObjectDistributionConfig DistributionConfig;
		public bool DebugPerformance;
		public UIController UIController;

		private PoissonDiscSampler _sampler;

		private Vector3 _minPos;
		private float _width;
		private float _height;
		private Grid2D<float> _noise;
		private List<GameObject> _spawnedObjects;

		private void Start()
		{
			_spawnedObjects = new List<GameObject>();

			var rend = GetComponentInChildren<Renderer>();
			_minPos = rend.bounds.min;

			_width = rend.bounds.size.x;
			_height = rend.bounds.size.z;

			UIController.OnGenerate += Generate;

			Generate();
		}

		public void Generate()
		{
			ClearSpawned();
			GenerateNoise();
			SpawnObjects();
		}

		private void ClearSpawned()
		{
			if (_spawnedObjects.Count == 0) return;

			foreach (var obj in _spawnedObjects)
			{
				Destroy(obj);
			}

			_spawnedObjects.Clear();
		}

		public void SpawnObjects()
		{
			ISamplerDecorator sampler = InstantiateSampler(DistributionConfig.DistributionStrategy);

			if (DebugPerformance)
			{
				sampler = new TimedSamplerDecorator(
					sampler,
					elapsed => Debug.Log("Sampler preparation took " + elapsed.ToString("0.##") + " seconds"),
					elapsed => Debug.Log("Sampler execution took " + elapsed.ToString("0.##") + " seconds"));
			}

			object samplerData = GetSamplerData();
			sampler.Prepare(samplerData);
			sampler.Generate(SpawnSample);
		}

		private ISamplerDecorator InstantiateSampler(ObjectDistributionConfig.Strategy strategy)
		{
			var radius = DistributionConfig.GetLargestRadius() * DistributionConfig.RadiusFactor;

			switch (strategy)
			{
				case ObjectDistributionConfig.Strategy.PoissonSampler:
					return new UniformSamplerDecorator(_width, _height, radius, false);

				case ObjectDistributionConfig.Strategy.UniformPoissonSampler:
					return new UniformSamplerDecorator(_width, _height, radius, true);

				case ObjectDistributionConfig.Strategy.NonUniformPoissonSampler:
					return new NonUniformSamplerDecorator(_width, _height, radius, _noise);

				default:
					throw new System.ArgumentException("Unsupported distribution strategy");
			}
		}

		private object GetSamplerData()
		{
			return null;
		}

		private void SpawnSample(Vector2 sample)
		{
			//if (basedOnNoise) {
			//    var noiseVal = adHoc ? Mathf.PerlinNoise(sample.x, sample.y) : GetNoiseVal(sample);
			//    if (noiseVal > NoiseConfig.Threshold) return false;
			//}

			var pos = sample.ToVector3();
			var spawned = Object.Instantiate(DistributionConfig.GetRandomPrefab(), pos + _minPos, Quaternion.identity);
			_spawnedObjects.Add(spawned);
		}

		private void GenerateNoise()
		{
			var watch = new Stopwatch();
			watch.Start();
			_noise = NoiseConfig.Type == NoiseType.Unity
				? UnityNoise((int) _width, (int) _height)
				: NoiseGenerator.PerlinNoise((int) _width, (int) _height, NoiseConfig.OctaveCount);
			watch.Stop();
			Debug.Log("Noise generation took " + watch.Elapsed.TotalSeconds.ToString("0.##") + "s");

			watch.Reset();
			watch.Start();
			UIController.RenderNoise(NoiseConfig.Type, _noise, NoiseConfig.Threshold);
			watch.Stop();
			Debug.Log("Noise rendering took " + watch.Elapsed.TotalSeconds.ToString("0.##") + "s");
		}

		// TODO: Move to NoiseGenerator
		private Grid2D<float> UnityNoise(int width, int height)
		{
			float xOrigin = Random.Range(0f, 1000f);
			float yOrigin = Random.Range(0f, 1000f);
			Grid2D<float> noise = new Grid2D<float>(width, height);

			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					noise.Set(x, y, Mathf.PerlinNoise((xOrigin + x) / 10f, (yOrigin + y) / 10f));
				}
			}

			return noise;
		}

		private float GetNoiseVal(Vector2 sample)
		{
			return _noise.Get((int) sample.x, (int) sample.y);
			//return NoiseConfig.Type == NoiseType.Unity
			//    ? Mathf.PerlinNoise(sample.x, sample.y)
			//    : _noise.Get((int)sample.x, (int)sample.y);
		}
	}
}