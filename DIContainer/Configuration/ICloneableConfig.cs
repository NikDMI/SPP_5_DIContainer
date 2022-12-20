using System;
using System.Collections.Generic;
using System.Text;

namespace DIContainer.Configuration
{
    internal interface ICloneableConfig
    {
        public IConfiguration MakeDeepCopy();

        /// <summary>
        /// Returns type of registered interface
        /// </summary>
        /// <param name="interfaceType"></param>
        /// <returns>null - if no registered dependency</returns>
        public IEnumerable<object> GetDependency(Type interfaceType, bool onlyFirstDependency = true);
    }
}
