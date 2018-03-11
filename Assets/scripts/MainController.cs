using Frictionless;
using UnityEngine;

namespace DistributionPrototype
{
	public class MainController : MonoBehaviour
	{
		private void Awake()
		{
			ServiceFactory.Instance.RegisterSingleton<MessageRouter>();

			// TODO: Integrate this handling with Zenject's Signals system
			//ServiceFactory.Instance.Resolve<MessageRouter>()
			//	.AddHandler<SaveChangesRequestMessage>(message => configFacade.PersistChanges());
		}

		private void OnDestroy()
		{
			ServiceFactory.Instance.Reset();
		}
	}
}