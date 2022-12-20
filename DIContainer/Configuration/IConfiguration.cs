using System;
using System.Reflection;

namespace DIContainer.Configuration
{
    /// <summary>
    /// Interface for dependencies configuration classes
    /// </summary>
    public interface IConfiguration
    {
        //How container should create instances
        enum LifePeriod { PER_INSTANCE, SINGELTON };

        //Registrates new dependence in configurations
        public void Register(Type instanceInterface, Type instanceImplementation, LifePeriod lifePeriod = LifePeriod.PER_INSTANCE);

        public void Register<TInterface, TRealization>(LifePeriod lifePeriod = LifePeriod.PER_INSTANCE);

       
    }
}
