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
		[SerializeField]
		private NoiseConfig _noiseConfig;

		[SerializeField]
		private ObjectDistributionConfig _distributionConfig;

		[SerializeField]
		private bool _debugPerformance;

		private MessageRouter _messageRouter;
		private List<GameObject> _spawnedObjects;
		private Vector3 _minPos;
		private float _width;
		private float _height;

		private void Start()
		{
			_spawnedObjects = new List<GameObject>();

			var rend = GetComponentInChildren<Renderer>();
			_minPos = rend.bounds.min;

			_width = rend.bounds.size.x;
			_height = rend.bounds.size.z;

			_messageRouter = ServiceFactory.Instance.Resolve<MessageRouter>();
			_messageRouter.AddHandler<GenerationRequestedMessage>(m => Generate());

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
			var decoratorFactory = new SamplerDecoratorFactory(_noiseConfig, _distributionConfig);
			decoratorFactory.DebugPerformance = _debugPerformance;
			ISamplerDecorator sampler = decoratorFactory.GetSamplerDecorator(_width, _height);
			
			sampler.Prepare();
			sampler.Generate(delegate(Vector2 sample)
			{
				var pos = sample.ToVector3();
				var spawned = Instantiate(
					_distributionConfig.GetRandomPrefab(), 
					pos + _minPos,
					Quaternion.identity);

				_spawnedObjects.Add(spawned);
			});
		}
	}
}