using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awiz.Core.Test.Assets.ExtendsImplements
{
    public interface Interface1
    {
        int Test { get; }

        Interface2 Association { get; }
    }
}
