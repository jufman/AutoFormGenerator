using System;
using AutoFormGenerator.Events;

namespace AutoFormGenerator.Object
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class FormField : Attribute
    {
        public string DisplayName { get; set; } = string.Empty;
        public Types Type { get; set; } = Types.Prop;
        public ObjectTypes ObjectTypeName { get; set; } = ObjectTypes.Default;
        public string ToolTip { get; set; } = string.Empty;
        public bool Required { get; set; } = false;
        public int Order { get; set; } = 999;

        public double ControlWidth { get; set; } = Double.NaN;

        public bool CanEdit { get; set; } = true;
        public bool IsVisible { get; set; } = true;

        public string Regex { get; set; } = string.Empty;

        public Type CustomControl { get; set; }

        public Type NestedClassType { get; set; } = null;
        public Type NestedListClassType { get; set; } = null;
    }

    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    public class FieldCondition : Attribute
    {
        public string Field { get; set; }
        public Operators Operator { get; set; } = Operators.Equals;
        public object Value { get; set; }
        public bool IsOr { get; set; } = false;

    }


    public enum Operators
    {
        Equals,
        NotEquals
    }


    public enum Types
    {
        NestedClass,
        NestedList,
        Prop
    }

    public enum ObjectTypes
    {
        String,
        Password,
        Double,
        Int32,
        Single,
        Boolean,
        SpecialDropdown,
        FolderBrowser,
        TimePicker,
        Default,
        Custom
    }
}
