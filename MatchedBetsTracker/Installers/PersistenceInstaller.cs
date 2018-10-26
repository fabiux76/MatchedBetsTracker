using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using MatchedBetsTracker.BusinessLogic;
using MatchedBetsTracker.Models;

namespace MatchedBetsTracker.Installers
{
    public class PersistenceInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<ApplicationDbContext>()
                .ImplementedBy<ApplicationDbContext>()
                .LifestylePerWebRequest()
            );

            container.Register(Component.For<IMatchedBetsRepository>()
                .ImplementedBy<MatchedBetsRepository>()
                .LifestylePerWebRequest()
            );
        }
    }
}
