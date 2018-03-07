using DistributionPrototype.Config;
using DistributionPrototype.UI.Components;
using DistributionPrototype.Util;
using Frictionless;
using UnityEngine;
using UnityEngine.UI;

namespace DistributionPrototype.UI
{
	public class NoiseConfigPanel : MonoBehaviour
	{
		[SerializeField] private Dropdown _noiseTypeDropdown;
		[SerializeField] private IntSliderField _octaveCountSliderField;

		private ConfigFacade _configFacade;

		private void Awake()
		{
			UiUtils.PopulateDropboxWithEnum(_noiseTypeDropdown, typeof(NoiseType));
		}

		private void Start()
		{
			_configFacade = ServiceFactory.Instance.Resolve<ConfigFacade>();

			// Setup noise type dropdown
			NoiseType noiseType = _configFacade.NoiseConfig.Type;
			_noiseTypeDropdown.value = (int) noiseType;
			_noiseTypeDropdown.onValueChanged.AddListener(OnNoiseTypeDropdownChanged);
			OnNoiseTypeUpdated(noiseType);

			// Setup octave count field
			int octaveCount = _configFacade.NoiseConfig.OctaveCount;
			_octaveCountSliderField.Initialize(octaveCount, 1, 10,
				delegate(int newOctaveCount)
				{
					_configFacade.NoiseConfig.OctaveCount = newOctaveCount;
				});
		}

		private void OnNoiseTypeDropdownChanged(int value)
		{
			var newNoiseType = (NoiseType) value;
			_configFacade.NoiseConfig.Type = newNoiseType;
			OnNoiseTypeUpdated(newNoiseType);
		}

		private void OnNoiseTypeUpdated(NoiseType type)
		{
			_octaveCountSliderField.gameObject.SetActive(type == NoiseType.Custom);
		}
	}
}
