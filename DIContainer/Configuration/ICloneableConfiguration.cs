using System;
using System.Collections.Generic;
using System.Text;

namespace DIContainer.Configuration
{
    internal interface ICloneableConfiguration
    {
        /// <summary>
        /// Creates deep copy of configuration dependencies
        /// </summary>
        /// <returns></returns>
        public IConfiguration MakeDeepCopy();
    }
}
