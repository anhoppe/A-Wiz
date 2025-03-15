using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awiz.Core
{
    public interface IClassFilter
    {
        /// <summary>
        /// Filters classes from the class provider that are not supposed to be in 
        /// the resulting graph
        /// </summary>
        /// <param name="classProvider">input class provider</param>
        /// <returns>Fitlered class provider that can be used to generate the graph</returns>
        IClassProvider Filter(IClassProvider classProvider);
    }
}
