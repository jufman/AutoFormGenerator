using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFormGenerator.Object;

namespace TestApp.Objects
{
    [FormClass]
    public class MainClass : ExtendClass
    {
        [FormField]
        public string TestString { get; set; } = "";

        [FormField] 
        public bool TestBool { get; set; }

        [FormField]
        public int TestInt { get; set; }
        [FormField]
        public double TestDouble { get; set; }
        [FormField]
        public float TestFloat { get; set; }

        [FormField(ObjectTypeName = ObjectTypes.Custom, CustomControl = typeof(Controls.CustomControl))]
        public float Slider { get; set; } = 0.5f;


        [FormField(ObjectTypeName = ObjectTypes.TimePicker)]
        public double TestDoubleTime { get; set; } = 300;

        [FormField(ObjectTypeName = ObjectTypes.Password)]
        [FieldCondition(Field = "TestBool", Operator = Operators.Equals, Value = true)]
        public string TestPassword { get; set; } = "";

        [FormField(ObjectTypeName = ObjectTypes.SpecialDropdown)]
        public string TestDropDown { get; set; }

        [FormField(Type = Types.NestedSettings)]
        public NestedClass NestedClass { get; set; }

        [FormField]
        public List<ExtendClass> ExtendClasses { get; set; } = new List<ExtendClass>();

    }
}
