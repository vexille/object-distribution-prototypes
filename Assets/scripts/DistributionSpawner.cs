using DistributionPrototype.Config;
using DistributionPrototype.Distribution.Decorator;
using DistributionPrototype.Messages;
using Frictionless;
using System.Collections.Generic;
using UnityEngine;

namespace DistributionPrototype.Distribution
{
	public class DistributionSpawner : MonoBehaviour
	{
		private MessageRouter _messageRouter;
		private ConfigFacade _configFacade;
		private List<GameObject> _spawnedObjects;
		private GameObject _spawnRoot;
		private Vector3 _minPos;
		private float _width;
		private float _height;

		private void Start()
		{
			_spawnRoot = new GameObject("SpawnedObjects");
			_spawnedObjects = new List<GameObject>();

			var rend = GetComponentInChildren<Renderer>();
			_minPos = rend.bounds.min;

			_width = rend.bounds.size.x;
			_height = rend.bounds.size.z;

			_messageRouter = ServiceFactory.Instance.Resolve<MessageRouter>();
			_messageRouter.AddHandler<GenerationRequestedMessage>(m => Generate());

			_configFacade = ServiceFactory.Instance.Resolve<ConfigFacade>();

			Generate();
		}

		public void Generate()
		{
			_messageRouter.RaiseMessage(new GenerationStartedMessage());

			ClearSpawned();
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
			var decoratorFactory = new SamplerDecoratorFactory(
				_configFacade.NoiseConfig,
				_configFacade.DistributionConfig);
			decoratorFactory.DebugPerformance = _configFacade.DebugPerformance;
			ISamplerDecorator sampler = decoratorFactory.GetSamplerDecorator(_width, _height);
			
			sampler.Prepare();
			sampler.Generate(delegate(Vector2 sample)
			{
				var pos = sample.ToVector3();
				var spawned = Instantiate(
					_configFacade.DistributionConfig.GetRandomPrefab(),
					_spawnRoot.transform);

				spawned.transform.position = pos + _minPos;
				_spawnedObjects.Add(spawned);
			});

			StaticBatchingUtility.Combine(_spawnRoot);
		}
	}
}