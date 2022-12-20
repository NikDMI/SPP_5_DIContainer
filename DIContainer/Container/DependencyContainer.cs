using System;
using System.Collections.Generic;
using DIContainer.Configuration;

namespace DIContainer.Container
{
    public class DependencyContainer : IContainer
    {
        public DependencyContainer(IConfiguration config)
        {
            _dependenciesCongifuration = config as ICloneableConfig;
            if (_dependenciesCongifuration == null)
            {
                throw new ArgumentException("Invalid config type");
            }
            _dependenciesCongifuration = _dependenciesCongifuration.MakeDeepCopy();//Create deep copy to prevent collisions
        }


        public TInterface Resolve<TInterface>() where TInterface : class
        {
            Type interfaceType = typeof(TInterface);
            //If user want all dependencies
            if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition().Equals(typeof(IEnumerable<>)))
            {
                //Get all dependencies
                IEnumerable<object> dependencies = _dependenciesCongifuration.GetDependency(interfaceType.GenericTypeArguments[0], false);
                if (!IsValidDependencies(dependencies))
                {
                    throw new Exception("Invalid dependencies");
                }
                return (TInterface)dependencies;//??
            }
            var firstDependency = _dependenciesCongifuration.GetDependency(typeof(TInterface));
            if (!IsValidDependencies(firstDependency))
            {
                throw new Exception("Invalid dependency");
            }
            var instanceEnumerator = firstDependency.GetEnumerator();
            instanceEnumerator.MoveNext();
            return (TInterface)instanceEnumerator.Current;
        }


        //Check if returned dependencies from configs are valid
        private bool IsValidDependencies(IEnumerable<object> dependencies)
        {
            IEnumerator<object> enumerator = dependencies.GetEnumerator();
            if (!enumerator.MoveNext()) //Zero elements
            {
                return false;
            }
            do
            {
                if (enumerator.Current == null)
                {
                    return false;
                }
            } while (enumerator.MoveNext());
            return true;
        }

        private ICloneableConfig _dependenciesCongifuration;

    }
}
