using DistributionPrototype.Messages;
using Frictionless;
using UnityEngine;
using UnityEngine.UI;

namespace DistributionPrototype.UI
{
	public class ButtonControlsPanel : MonoBehaviour
	{
		[SerializeField] private Button _generateButton;
		[SerializeField] private Button _saveChangesButton;

		private void Awake()
		{
			var messageRouter = ServiceFactory.Instance.Resolve<MessageRouter>();
			_generateButton.onClick.AddListener(() =>
			{
				messageRouter.RaiseMessage(new GenerationRequestedMessage());
			});

			// Define guard the save changes button, as it should only be used in the editor
#if UNITY_EDITOR
			_saveChangesButton.onClick.AddListener(() =>
			{
				messageRouter.RaiseMessage(new SaveChangesRequestMessage());
			});
#else
			_saveChangesButton.gameObject.SetActive(false);
#endif
		}
	}
}
