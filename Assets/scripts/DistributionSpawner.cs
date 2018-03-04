using DistributionPrototype.Config;
using DistributionPrototype.Distribution.Decorator;
using DistributionPrototype.UI;
using DistributionPrototype.Util;
using System.Collections.Generic;
using System.Diagnostics;
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
			float radius = DistributionConfig.GetLargestRadius() * DistributionConfig.RadiusFactor;

			switch (strategy)
			{
				case ObjectDistributionConfig.Strategy.UniformPoissonSamplerA:
					return new UniformSamplerADecorator(_width, _height, radius);

				case ObjectDistributionConfig.Strategy.UniformPoissonSamplerB:
					return new UniformSamplerADecorator(_width, _height, radius);

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
			// TODO: Cleanup?
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
				? NoiseGenerator.UnityNoise ((int) _width, (int) _height)
				: NoiseGenerator.PerlinNoise((int) _width, (int) _height, NoiseConfig.OctaveCount);
			watch.Stop();

			watch.Reset();
			watch.Start();
			UIController.RenderNoise(NoiseConfig.Type, _noise, NoiseConfig.Threshold);
			watch.Stop();
		}
	}
}