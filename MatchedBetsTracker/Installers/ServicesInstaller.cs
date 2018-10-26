using Castle.MicroKernel.Registration;
using MatchedBetsTracker.BusinessLogic;

namespace MatchedBetsTracker.Installers
{
    public class ServiceInstaller : IWindsorInstaller
    {
        public void Install(Castle.Windsor.IWindsorContainer container, Castle.MicroKernel.SubSystems.Configuration.IConfigurationStore store)
        {
            container.Register(
                Component
                    .For<ISayHelloService>()
                    .ImplementedBy<SayHelloService>()
                    .LifestyleSingleton());

            container.Register(
                Component
                    .For<IMatchedBetModelController>()
                    .ImplementedBy<MatchedBetModelController>()
                    .LifestylePerWebRequest());
        }
    }
}