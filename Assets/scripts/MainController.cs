using DistributionPrototype.Config;
using DistributionPrototype.Messages;
using Frictionless;
using UnityEngine;

namespace DistributionPrototype
{
	public class MainController : MonoBehaviour
	{
		private void Awake()
		{
			ServiceFactory.Instance.RegisterSingleton<MessageRouter>();
			var configFacade = GameObject.FindObjectOfType<ConfigFacade>();
			if (configFacade == null)
			{
				throw new System.Exception("ConfigFacade not found in scene");
			}

			ServiceFactory.Instance.RegisterSingleton(configFacade);
			ServiceFactory.Instance.Resolve<MessageRouter>()
				.AddHandler<SaveChangesRequestMessage>(message => configFacade.PersistChanges());
		}

		private void OnDestroy()
		{
			ServiceFactory.Instance.Reset();
		}
	}
}