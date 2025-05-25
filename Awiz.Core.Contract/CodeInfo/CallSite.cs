namespace Awiz.Core.Contract.CodeInfo
{
    public class CallSite
    {
        public CallSite(ClassInfo classInfo, MethodInfo method)
        {
            Class = classInfo;
            Method = method;
        }

        public ClassInfo Class { get; } = new ClassInfo();

        public MethodInfo Method { get; } = new MethodInfo();
    }
}
