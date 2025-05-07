using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awiz.Core.Test.Assets.ExtendsImplements
{
    public class Class1 : Interface1
    {
        public IList<int> Ints { get; set; }
        
        public IList<floats> Floats { get; set; }

        public Class2 Class2Prop { get; set; }

        public void MyFunc(float param)
        {

        }

        public bool MyBetterFunc(double param)
        {
            return true;
        }
    }
}
