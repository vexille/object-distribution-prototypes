using DistributionPrototype.Signals;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace DistributionPrototype.UI
{
	public class ButtonControlsPanel : MonoBehaviour
	{
		[SerializeField] private Button _generateButton;
		[SerializeField] private Button _saveChangesButton;

		private GenerationRequestedSignal _generationRequestedSignal;
		private SaveChangesRequestedSignal _saveChangesRequestedSignal;

		[Inject]
		public void Init(
			GenerationRequestedSignal generationRequestedSignal,
			SaveChangesRequestedSignal saveChangesRequestedSignal)
		{
			_generationRequestedSignal = generationRequestedSignal;
			_saveChangesRequestedSignal = saveChangesRequestedSignal;
			
			_generateButton.onClick.AddListener(() =>
			{
				_generationRequestedSignal.Fire();
			});

			// Define guard the save changes button, as it should only be used in the editor
#if UNITY_EDITOR
			_saveChangesButton.onClick.AddListener(() =>
			{
				_saveChangesRequestedSignal.Fire();
			});
#else
			_saveChangesButton.gameObject.SetActive(false);
#endif
		}
	}
}
