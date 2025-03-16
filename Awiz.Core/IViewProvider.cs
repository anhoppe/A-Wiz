using Gwiz.Core.Contract;

namespace Awiz.Core
{
    /// <summary>
    /// Provides all views defined in a repo
    /// </summary>
    public interface IViewProvider
    {
        List<string> Views { get; }

        IGraph GetViewByName(string viewName);
    }
}
