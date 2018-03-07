using DistributionPrototype.Messages;
using Frictionless;
using UnityEngine;

namespace DistributionPrototype.UI
{
	public class UIController : MonoBehaviour
	{
		public void OnGenerateClick()
		{
			ServiceFactory.Instance.Resolve<MessageRouter>()
				.RaiseMessage(new GenerationRequestedMessage());
		}
	}
}