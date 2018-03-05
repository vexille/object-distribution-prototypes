using Frictionless;
using UnityEngine;

namespace DistributionPrototype
{
	public class MainController : MonoBehaviour
	{
		private void Awake()
		{
			ServiceFactory.Instance.RegisterSingleton<MessageRouter>();
		}

		private void OnDestroy()
		{
			ServiceFactory.Instance.Reset();
		}
	}
}