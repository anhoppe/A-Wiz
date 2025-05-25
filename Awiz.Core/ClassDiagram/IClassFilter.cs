using Awiz.Core.CSharpParsing;

namespace Awiz.Core.ClassDiagram
{
    public interface IClassFilter
    {
        /// <summary>
        /// Filters classes from the class provider that are not supposed to be in 
        /// the resulting graph
        /// </summary>
        /// <param name="classProvider">input class provider</param>
        /// <returns>Fitlered class provider that can be used to generate the graph</returns>
        ISourceCode Filter(ISourceCode classProvider);
    }
}
