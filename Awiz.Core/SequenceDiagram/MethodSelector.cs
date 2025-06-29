using Awiz.Core.Contract.CodeInfo;
using Awiz.Core.CSharpParsing;
using Gwiz.Core.Contract;

namespace Awiz.Core.SequenceDiagram
{
    internal class MethodSelector : IMethodSelector
    {
        internal ISourceCode? SourceCode { private get; set; }

        public IList<ContextMenuItem> CreateAddMethodCallSelection(MethodInfo calledMethod, Action<ClassInfo, MethodInfo> addMethodCall)
        {
            if (SourceCode == null)
            {
                throw new NullReferenceException("SourceCode is not set.");
            }

            var callSites = SourceCode.GetCallSites(calledMethod);

            return callSites.Select(p => new ContextMenuItem()
            {
                Callback = () =>
                {
                    ClassInfo implClass = p.Class;
                    MethodInfo implMethod = p.Method;

                    if (p.Class.Type == ClassType.Interface)
                    {
                        var implementations = SourceCode.GetImplementations(p.Class);

                        if (implementations.Count > 0)
                        {
                            implClass = implementations[0]; // For now just take the first implementation
                            foreach (var method in implClass.Methods)
                            {
                                // ToDo: also compare parameters to cover overloads
                                if (method.Name == p.Method.Name)
                                {
                                    implMethod = method;
                                    break;
                                }
                            }
                        }
                    }

                    addMethodCall(implClass, implMethod);
                },
                Name = $"{p.Class.Name}.{p.Method.Name}",
            }).ToList();
        }

        public IList<ContextMenuItem> CreateStartSequenceSelection(IList<MethodInfo> methods, Action<MethodInfo> startCallSequence)
        {
            return methods.Select(p => new ContextMenuItem()
            {
                Callback = () =>
                {
                    startCallSequence(p);
                },
                Name = p.Name,
            }).ToList();
        }

        public IList<ContextMenuItem> CreateStartSequenceSelection(IList<ClassInfo> classesInDiagram, Action<ClassInfo, MethodInfo> startCallSequence)
        {
            return classesInDiagram.Select(classInfo => new ContextMenuItem() 
            {
                Name = classInfo.Id(),
                SubMenuItems = classInfo.Methods.Select(methodInfo => new ContextMenuItem()
                {
                    Name = methodInfo.Name,
                    Callback = () => startCallSequence(classInfo, methodInfo)
                }).ToList()
            }).ToList();
        }
    }
}
