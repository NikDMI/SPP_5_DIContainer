using System;
using System.Collections.Generic;
using System.Text;

namespace DIContainer.Container
{
    /// <summary>
    /// Intenrface for DI containers
    /// </summary>
    public interface IContainer
    {
        public TInterface Resolve<TInterface>() where TInterface : class;
    }
}
