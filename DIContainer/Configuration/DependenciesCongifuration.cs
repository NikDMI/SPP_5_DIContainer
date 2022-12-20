using System;
using System.Collections.Generic;
using System.Reflection;

namespace DIContainer.Configuration
{
    public class DependenciesCongifuration : IConfiguration, ICloneableConfig
    {

        //Registrates new dependence in configurations
        public void Register(Type instanceInterface, Type instanceImplementation, IConfiguration.LifePeriod lifePeriod = IConfiguration.LifePeriod.PER_INSTANCE)
        {
            if (instanceInterface.IsValueType || instanceImplementation.IsValueType)
            {
                throw new ArgumentException("Can't register value types");
            }
            //Check if class is abstract
            if (instanceImplementation.IsAbstract)
            {
                throw new ArgumentException("You can't register abstract class");
            }
            //Check does implementation has required interface
            bool isImplemented = false;
            foreach (var interfaceType in instanceImplementation.GetInterfaces())
            {
                if (interfaceType.Equals(instanceInterface))
                {
                    isImplemented = true;
                    break;
                }
            }
            if (!isImplemented)
            {
                if (!instanceInterface.IsGenericType || instanceInterface.IsConstructedGenericType)
                {
                    throw new ArgumentException("Interface wasn't implemented");
                }
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
                Predicate<DependencyInfo> searchPredicate = dependencyInfo => { return (dependencyInfo.implementationType.Equals(instanceImplementation)) ? true : false; };
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


        ICloneableConfig ICloneableConfig.MakeDeepCopy()
        {
            DependenciesCongifuration copyConfig = (DependenciesCongifuration)this.MemberwiseClone();
            //Create copy of dictionaries
            copyConfig._closedTypes = new Dictionary<Type, List<DependencyInfo>>();
            var closedKeys = _closedTypes.Keys;
            foreach(var key in closedKeys)
            {
                copyConfig._closedTypes.Add(key, _closedTypes[key]);
            }
            copyConfig._openGenericTypes = new Dictionary<Type, List<DependencyInfo>>();
            var openKeys = _openGenericTypes.Keys;
            foreach (var key in openKeys)
            {
                copyConfig._openGenericTypes.Add(key, _openGenericTypes[key]);
            }
            return copyConfig;
        }


        public IEnumerable<object> GetDependency(Type interfaceType, bool onlyFirstDependency = true)
        {
            List<object> dependencyObjects = new List<object>();
            List<DependencyInfo> dependenciesList = null;
            //Search in closed types
            _closedTypes.TryGetValue(interfaceType, out dependenciesList);
            //Search in open generic types
            if (interfaceType.IsGenericType)
            {
                if (dependenciesList == null || !onlyFirstDependency)   //If no close type match, or user want all dependencies
                {
                    var genericOpenType = interfaceType.GetGenericTypeDefinition();
                    List<DependencyInfo> openTypeDependencies = null;
                    if (_openGenericTypes.TryGetValue(genericOpenType, out openTypeDependencies))
                    {
                        //Add new dependencies to list
                        if (dependenciesList != null)
                        {
                            dependenciesList.AddRange(openTypeDependencies);
                        }
                        else
                        {
                            dependenciesList = openTypeDependencies;
                        }
                    }
                }
            }
            //Create instances
            if (dependenciesList != null)
            {
                if (onlyFirstDependency)
                {
                    dependencyObjects.Add(CreateDependencyInstance(interfaceType, dependenciesList[0]));
                }
                else
                {
                    foreach(var dependencyInfo in dependenciesList)
                    {
                        dependencyObjects.Add(CreateDependencyInstance(interfaceType, dependencyInfo));
                    }
                }
            }
            return dependencyObjects;
        }


        private object CreateDependencyInstance(Type interfaceType, DependencyInfo dependencyInfo)
        {
            object dependencyObject = null;
            Type dependencyType = dependencyInfo.implementationType;
            //Check if type is open generic (create close generic)
            if (dependencyType.IsGenericType && !dependencyType.IsConstructedGenericType)
            {
                var genericArguments = interfaceType.GetGenericArguments();
                dependencyType = dependencyType.MakeGenericType(genericArguments);
            }
            //Try get object from singeltons, if it was created
            if (dependencyInfo.lifePeriod == IConfiguration.LifePeriod.SINGELTON)
            {
                if(_singeltonsObjects.TryGetValue(dependencyType, out dependencyObject))
                {
                    return dependencyObject;
                }
            }
            //Create new object (from first suitable public constructor)
            var publicConstructurs = dependencyType.GetConstructors();
            foreach (var constructor in publicConstructurs)
            {
                var constructorParameterTypes = constructor.GetParameters();
                List<object> constructorParameters = new List<object>();
                //Create parameters for constructor
                foreach (var parameterType in constructorParameterTypes)
                {
                    if (parameterType.ParameterType.IsValueType)//can't init value types
                    {
                        break;
                    }
                    var paramDependency = GetDependency(parameterType.ParameterType);
                    if (paramDependency == null)
                    {
                        break;
                    }
                    constructorParameters.Add(paramDependency);
                }
                //Create object if all constructor params was init
                if (constructorParameters.Count == constructorParameterTypes.Length)
                {
                    dependencyObject = constructor.Invoke(constructorParameters.ToArray());
                    break;
                }
            }
            //Update singelton table
            if (dependencyObject != null && dependencyInfo.lifePeriod == IConfiguration.LifePeriod.SINGELTON)
            {
                _singeltonsObjects.Add(dependencyType, dependencyObject);
            }
            return dependencyObject;
        }


        internal class DependencyInfo
        {
            public DependencyInfo(Type dependencyType, IConfiguration.LifePeriod lifePeriod)
            {
                this.lifePeriod = lifePeriod;
                this.implementationType = dependencyType;
            }

            public Type implementationType { get; private set; }
            public IConfiguration.LifePeriod lifePeriod { get; private set; }
        }




        //correspondene between closed types
        private Dictionary<Type, List<DependencyInfo>> _closedTypes = new Dictionary<Type, List<DependencyInfo>>();

        //correspondence for open generics like List<>
        private Dictionary<Type, List<DependencyInfo>> _openGenericTypes = new Dictionary<Type, List<DependencyInfo>>(); 

        private Dictionary<Type, object> _singeltonsObjects = new Dictionary<Type, object>();
    }
}
