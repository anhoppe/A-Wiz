using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awiz.Core.Test.Assets.GenericSupport
{
    public class Class1
    {
        public IList<string> StringList { get; set; } = new();

        public List<int> IntList { get; set; } = new();

        public List<Class2> Class2List { get; set; } = new();
    }
}
