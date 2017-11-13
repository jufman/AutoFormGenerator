using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoFormGenorator.Object
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class FormClass : Attribute
    {
        public string DisplayName { get; set; } = string.Empty;

        public double FormValueWidth { get; set; } = -1;
    }
}
