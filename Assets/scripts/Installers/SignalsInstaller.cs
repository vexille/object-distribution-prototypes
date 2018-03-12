using DistributionPrototype.Config;
using DistributionPrototype.Signals;
using Zenject;

namespace DistributionPrototype.Installers
{
	public class SignalsInstaller : Installer<SignalsInstaller>
	{
		public override void InstallBindings()
		{
			Container.DeclareSignal<GenerationRequestedSignal>();
			Container.DeclareSignal<GenerationStartedSignal>();
			Container.DeclareSignal<LimiterNoiseGeneratedSignal>();
			Container.DeclareSignal<SamplerNoiseGeneratedSignal>();
			Container.DeclareSignal<SaveChangesRequestedSignal>();
			Container.BindSignal<SaveChangesRequestedSignal>()
				.To<ConfigFacade>(configFacade => configFacade.PersistChanges())
				.AsSingle();
		}
	}
}
