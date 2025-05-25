using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awiz.Core.Test.Assets.ExtendsImplements
{
    public class Class1 : Interface1
    {
        private Class3 _class3Field = new Class3();

        public IList<int> Ints { get; set; }
        
        public IList<float> Floats { get; set; }

        internal Class2? Class2Prop { get; set; }

        internal Interface2 Interface2 { get; set; }

        public int Test => throw new NotImplementedException();\']

        public Interface2 Association => throw new NotImplementedException();

        public void MyFunc(float param)
        {
            Class2Prop.MyOtherFunc();

            for (int i = 0; i < 10; i++)
            {
                _class3Field.MyEvenMoreOtherFunc();
            }

            Interface2.ThisIsAnInterfaceMethod();
        }

        public bool MyBetterFunc(double param)
        {
            Class2Prop.MyOtherFunc();
            Class2Prop.MyOtherFunc();

            return true;
        }
    }
}
