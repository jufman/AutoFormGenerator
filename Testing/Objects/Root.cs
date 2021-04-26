using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFormGenerator.Object;

namespace Testing.Objects
{
    public class Root
    {
        [FormField]
        public string Test1 { get; set; }
        [FormField]
        public string Test2 { get; set; }
        [FormField]
        public string Test3 { get; set; }
        [FormField]
        public string Test4 { get; set; }

        [FormField]
        public int TestI1 { get; set; }
        [FormField]
        public int TestI2 { get; set; }
        [FormField]
        public int TestI3 { get; set; }
        [FormField]
        public int TestI4 { get; set; }

        [FormField]
        public bool TestB1 { get; set; }
        [FormField]
        public bool TestB2 { get; set; }
        [FormField]
        public bool TestB3 { get; set; }
        [FormField]
        public bool TestB4 { get; set; }

        [FormField(Type = Types.NestedSettings)]
        public NestedSetting NestedSetting1 { get; set; } = new NestedSetting();
        [FormField(Type = Types.NestedSettings)]
        public NestedSetting NestedSetting2 { get; set; } = new NestedSetting();
        [FormField(Type = Types.NestedSettings)]
        public NestedSetting NestedSetting3 { get; set; } = new NestedSetting();
        [FormField(Type = Types.NestedSettings)]
        public NestedSetting NestedSetting4 { get; set; } = new NestedSetting();


    }
}
