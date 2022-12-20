using System;
using System.Collections.Generic;
using DIContainer.Configuration;

namespace DIContainer.Container
{
    public class DependencyContainer : IContainer
    {
        public DependencyContainer(IConfiguration config)
        {
            DependenciesCongifuration dependencyConfig = config as DependenciesCongifuration;
            if (dependencyConfig == null)
            {
                throw new ArgumentException("Invalid config type");
            }
        }


        public TInterface Resolve<TInterface>() where TInterface : class
        {

            return null;
        }


        private DependenciesCongifuration _dependenciesCongifuration;

    }
}
