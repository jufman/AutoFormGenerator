using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoFormGenorator.Object
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class FormField : Attribute
    {
        public string DisplayName { get; set; } = string.Empty;
        public Types Type { get; set; } = Types.Prop;
        public string ObjectTypeName { get; set; } = string.Empty;
        public string ToolTip { get; set; } = string.Empty;
        public bool Required { get; set; } = false;

        public Type DropDownClass { get; set; }
    }

    public enum Types
    {
        NestedSettings,
        NestedList,
        NestedMulitNode,
        Prop,
        MultiProp,
        NestedItems
    }
}
