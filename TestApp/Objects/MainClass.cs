using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFormGenerator.Object;

namespace TestApp.Objects
{
    public class MainClass
    {
        [FormField]
        public string TestString { get; set; }
        [FormField]
        public bool TestBool { get; set; }
        [FormField]
        public int TestInt { get; set; }
        [FormField]
        public double TestDouble { get; set; }
        [FormField(ObjectTypeName = ObjectTypes.Password)]
        public string TestPassword { get; set; }

        [FormField(ObjectTypeName = ObjectTypes.SpecialDropdown)]
        public string TestDropDown { get; set; }

        [FormField(Type = Types.NestedSettings)]
        public NestedClass NestedClass { get; set; } 

    }
}
