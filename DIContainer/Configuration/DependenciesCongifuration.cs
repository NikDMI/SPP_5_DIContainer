using System;
using System.Collections.Generic;
using System.Reflection;

namespace DIContainer.Configuration
{
    public class DependenciesCongifuration : IConfiguration, ICloneableConfiguration
    {

        //Registrates new dependence in configurations
        public void Register(Type instanceInterface, Type instanceImplementation, IConfiguration.LifePeriod lifePeriod = IConfiguration.LifePeriod.PER_INSTANCE)
        {
            if (instanceInterface.IsValueType || instanceImplementation.IsValueType)
            {
                throw new ArgumentException("Can't register value types");
            }
            //Register dependency
            if (!instanceInterface.IsGenericType || instanceInterface.IsConstructedGenericType) //Register close type
            {
                RegisterNewDependency(_closedTypes, instanceInterface, instanceImplementation, lifePeriod);
            }
            else    //Register open generic type
            {
                RegisterNewDependency(_openGenericTypes, instanceInterface, instanceImplementation, lifePeriod);
            }
        }


        public void Register<TInterface, TRealization>(IConfiguration.LifePeriod lifePeriod = IConfiguration.LifePeriod.PER_INSTANCE)
        {
            Register(typeof(TInterface), typeof(TRealization), lifePeriod);
        }


        IConfiguration ICloneableConfiguration.MakeDeepCopy()
        {
            return null;
        }


        /// <summary>
        /// Register dependency in correespondance dictionary
        /// </summary>
        /// <param name="dependencyDictionary"></param>
        /// <param name="instanceInterface"></param>
        /// <param name="instanceImplementation"></param>
        private void RegisterNewDependency(Dictionary<Type, List<DependencyInfo>> dependencyDictionary, Type instanceInterface, Type instanceImplementation, IConfiguration.LifePeriod lifePeriod)
        {
            List<DependencyInfo> correspondenceDependencies = null;

            if (dependencyDictionary.TryGetValue(instanceInterface, out correspondenceDependencies))    //if this type also had some dependencies
            {
                Predicate<DependencyInfo> searchPredicate = dependencyInfo => { return (dependencyInfo.implementationType == instanceImplementation) ? true : false; };
                if (correspondenceDependencies.Find(searchPredicate) != null)
                {
                    throw new ArgumentException(String.Format("Dependency {0} was registrated in {1} earlier",
                        instanceImplementation.Name, instanceInterface.Name));
                }
                DependencyInfo newDependency = new DependencyInfo(instanceImplementation, lifePeriod);
                correspondenceDependencies.Add(newDependency);
            }
            else    //Add new type for dependencies
            {
                correspondenceDependencies = new List<DependencyInfo>();
                DependencyInfo newDependency = new DependencyInfo(instanceImplementation, lifePeriod);
                correspondenceDependencies.Add(newDependency);
                dependencyDictionary.Add(instanceInterface, correspondenceDependencies);
            }
        }


        public class DependencyInfo
        {
            public DependencyInfo(Type dependencyType, IConfiguration.LifePeriod lifePeriod)
            {
                this.lifePeriod = lifePeriod;
                this.implementationType = dependencyType;
            }

            public Type implementationType { get; private set; }
            public IConfiguration.LifePeriod lifePeriod { get; private set; }
        }


        private Dictionary<Type, List<DependencyInfo>> _closedTypes;  //correspondene between closed types

        private Dictionary<Type, List<DependencyInfo>> _openGenericTypes; //correspondence for open generics like List<>
    }
}
