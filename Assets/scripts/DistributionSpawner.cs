﻿using System;
using DistributionPrototype.Config;
using DistributionPrototype.Distribution.Decorator;
using DistributionPrototype.Signals;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace DistributionPrototype.Distribution
{
	/// <summary>
	/// Will spawn objects based on the data fetched from <see cref="ConfigFacade"/>
	/// and the points sampled by an <see cref="ISamplerDecorator"/>.
	/// <seealso cref="SamplerDecoratorFactory"/>
	/// </summary>
	public class DistributionSpawner : MonoBehaviour, IDisposable
	{
		private GenerationStartedSignal _generationStartedSignal;
		private GenerationRequestedSignal _generationRequestedSignal;

		private SamplerDecoratorFactory _decoratorFactory;
		private ConfigFacade _configFacade;

		private List<GameObject> _spawnedObjects;
		private GameObject _spawnRoot;
		private Vector3 _minPos;
		private float _width;
		private float _height;

		[Inject]
		public void Init(
			ConfigFacade configFacade, 
			SamplerDecoratorFactory decoratorFactory,
			GenerationStartedSignal generationStartedSignal,
			GenerationRequestedSignal generationRequestedSignal)
		{
			_configFacade = configFacade;
			_decoratorFactory = decoratorFactory;
			_generationStartedSignal = generationStartedSignal;
			_generationRequestedSignal = generationRequestedSignal;
			_generationRequestedSignal.Listen(Generate);
		}

		public void Dispose()
		{
			_generationRequestedSignal.Unlisten(Generate);
		}

		private void Start()
		{
			_spawnRoot = new GameObject("SpawnedObjects");
			_spawnedObjects = new List<GameObject>();

			// Get child renderer that will define the spawn area
			var rend = GetComponentInChildren<Renderer>();
			_minPos = rend.bounds.min;
			_width = rend.bounds.size.x;
			_height = rend.bounds.size.z;

			Generate();
		}

		private void Generate()
		{
			_generationStartedSignal.Fire();

			// Clear spawned objects
			if (_spawnedObjects.Count > 0)
			{
				foreach (var obj in _spawnedObjects)
				{
					Destroy(obj);
				}

				_spawnedObjects.Clear();
			}

			// Create new sampler and spawn objects
			ISamplerDecorator sampler = _decoratorFactory.GetSamplerDecorator(_width, _height);

			sampler.Prepare();
			sampler.Generate(delegate (Vector2 sample)
			{
				var pos = sample.ToVector3();
				var spawned = Instantiate(
					_configFacade.DistributionConfig.GetRandomPrefab(),
					_spawnRoot.transform);

				spawned.transform.position = pos + _minPos;
				_spawnedObjects.Add(spawned);
			});

			// Setup all spawned objects for static batching
			StaticBatchingUtility.Combine(_spawnRoot);
		}
	}
}