using DistributionPrototype.Config;
using DistributionPrototype.UI.Components;
using DistributionPrototype.Util;
using Frictionless;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace DistributionPrototype.UI
{
	public class DistributionConfigPanel : MonoBehaviour
	{
		[SerializeField] private Dropdown _distributionStrategyDropdown;
		[SerializeField] private FloatSliderField _radiusFactorSliderField;
		[SerializeField] private Toggle _noiseLimitedSpawnToggle;
		[SerializeField] private FloatSliderField _spawnThresholdSliderField;

		private ConfigFacade _configFacade;

		[Inject]
		private void Init(ConfigFacade configFacade)
		{
			_configFacade = configFacade;
		}

		private void Awake()
		{
			UiUtils.PopulateDropboxWithEnum(_distributionStrategyDropdown,
				typeof(ObjectDistributionConfig.Strategy));
		}

		private void Start()
		{
			// Setup distribution strategy dropdown
			ObjectDistributionConfig.Strategy distributionStrategy = 
				_configFacade.DistributionConfig.DistributionStrategy;
			_distributionStrategyDropdown.value = (int)distributionStrategy;
			_distributionStrategyDropdown.onValueChanged.AddListener(OnDistributionStrategyChange);

			// Setup radius factor field
			float radiusFactor = _configFacade.DistributionConfig.RadiusFactor;
			_radiusFactorSliderField.Initialize(radiusFactor, 1f, 10f,
				delegate(float newValue)
				{
					_configFacade.DistributionConfig.RadiusFactor = newValue;
				});

			// Setup noise limited toggle
			bool isNoiseLimited = _configFacade.DistributionConfig.NoiseLimitedSpawn;
			_noiseLimitedSpawnToggle.isOn = isNoiseLimited;
			_noiseLimitedSpawnToggle.onValueChanged.AddListener(OnNoiseLimitedSpawnChange);
			OnNoiseLimitedSpawnUpdated(isNoiseLimited);

			// Setup spawn threshold field
			float spawnThreshold = _configFacade.DistributionConfig.SpawnThreshold;
			_spawnThresholdSliderField.Initialize(spawnThreshold, 0f, 1f,
				delegate (float newValue)
				{
					_configFacade.DistributionConfig.SpawnThreshold = newValue;
				});
		}

		private void OnDistributionStrategyChange(int newValue)
		{
			var distributionStrategy = (ObjectDistributionConfig.Strategy) newValue;
			_configFacade.DistributionConfig.DistributionStrategy = distributionStrategy;
		}

		private void OnNoiseLimitedSpawnChange(bool newValue)
		{
			_configFacade.DistributionConfig.NoiseLimitedSpawn = newValue;
			OnNoiseLimitedSpawnUpdated(newValue);
		}

		private void OnNoiseLimitedSpawnUpdated(bool limitSpawn)
		{
			_spawnThresholdSliderField.gameObject.SetActive(limitSpawn);
		}
	}
}
