using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testing.Objects
{
    public class NestedSetting
    {
        [AutoFormGenerator.Object.FormField]
        public string Test1 { get; set; }
        [AutoFormGenerator.Object.FormField]
        public string Test2 { get; set; }
        [AutoFormGenerator.Object.FormField]
        public string Test3 { get; set; }
        [AutoFormGenerator.Object.FormField]
        public string Test4 { get; set; }

        [AutoFormGenerator.Object.FormField]
        public int TestI1 { get; set; }
        [AutoFormGenerator.Object.FormField]
        public int TestI2 { get; set; }
        [AutoFormGenerator.Object.FormField]
        public int TestI3 { get; set; }
        [AutoFormGenerator.Object.FormField]
        public int TestI4 { get; set; }

        [AutoFormGenerator.Object.FormField]
        public bool TestB1 { get; set; }
        [AutoFormGenerator.Object.FormField]
        public bool TestB2 { get; set; }
        [AutoFormGenerator.Object.FormField]
        public bool TestB3 { get; set; }
        [AutoFormGenerator.Object.FormField]
        public bool TestB4 { get; set; }
    }
}
