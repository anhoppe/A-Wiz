using Awiz.Core.CodeInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awiz.Core
{
    public interface IClassProvider
    {
        List<ClassInfo> Classes { get; }
    }
}
