using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFormGenerator.Object;

namespace TestApp.Objects
{
    public class ExtendClass
    {
        [FormField(Order = 4, ObjectTypeName = ObjectTypes.SpecialDropdown)]
        public double Act { get; set; } = 1;

    }
}
