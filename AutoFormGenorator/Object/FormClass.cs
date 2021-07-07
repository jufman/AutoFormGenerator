using System;

namespace AutoFormGenerator.Object
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class FormClass : Attribute
    {
        public string DisplayName { get; set; } = string.Empty;

        public double FormValueWidth { get; set; } = -1;

        public bool WindthOveride { get; set; }
    }
}
