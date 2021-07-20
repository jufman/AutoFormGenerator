using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFormGenerator.Object;

namespace TestApp.Objects
{
    [FormClass(WindthOveride = true, DisplayName = "Shrink Class", FormValueWidth = 50)]
    public class ShrinkClass
    {
        [FormField(Required = true, ControlWidth = 200, DisplayName = "Test String")]
        public string TestString { get; set; } = "";
        [FormField(ToolTip = "You might need this ")]
        public bool TestBool { get; set; }

        [FormField]
        public int TestInt { get; set; }
        [FormField]
        public double TestDouble { get; set; }
        [FormField]
        public float TestFloat { get; set; }
    }
}
