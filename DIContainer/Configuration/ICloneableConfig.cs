using System;
using System.Collections.Generic;
using System.Text;

namespace DIContainer.Configuration
{
    internal interface ICloneableConfig
    {
        public ICloneableConfig MakeDeepCopy();

        /// <summary>
        /// Returns type of registered interface
        /// </summary>
        /// <param name="interfaceType"></param>
        /// <returns>
        /// zero elements - if no registered dependency
        /// List can contains null objects (because can't invoke constuctors of implementation type)
        /// </returns>
        public IEnumerable<object> GetDependency(Type interfaceType, bool onlyFirstDependency = true);
    }
}
