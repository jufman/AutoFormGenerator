using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoFormGenorator.Object
{
    public class FormDropdownItem : Attribute 
    {
        public string DisplayValue { get; set; } = string.Empty;

        public object Value { get; set; } = string.Empty;

        public override string ToString()
        {
            return DisplayValue;
        }
    }
}
