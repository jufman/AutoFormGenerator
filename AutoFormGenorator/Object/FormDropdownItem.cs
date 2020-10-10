using System;

namespace AutoFormGenerator.Object
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
