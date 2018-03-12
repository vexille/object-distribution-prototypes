using DistributionPrototype.Config;
using DistributionPrototype.Distribution;
using UnityEngine;
using Zenject;

namespace DistributionPrototype.Installers
{
	public class MainInstaller : MonoInstaller<MainInstaller>
	{
		[SerializeField]
		private NoiseConfig _noiseConfig;

		[SerializeField]
		private ObjectDistributionConfig _distributionConfig;

		[SerializeField]
		private bool _debugPerformance;

		public override void InstallBindings()
		{
			// Bind configuration data exposed in the editor
			Container.BindInstance(_noiseConfig).AsSingle();
			Container.BindInstance(_distributionConfig).AsSingle();
			Container.Bind<bool>()
				.WithId("debugPerformance")
				.FromInstance(_debugPerformance).AsSingle();
			
			Container.Bind<ConfigFacade>().AsSingle();
			Container.Bind<SamplerDecoratorFactory>().AsSingle();

			// Bind signals
			SignalsInstaller.Install(Container);
		}
	}
}