using Awiz.Core.Contract.CodeInfo;

namespace Awiz.Core.CSharpClassGenerator
{
    public interface IClassProvider
    {
        List<ClassInfo> Classes { get; }
    }
}
