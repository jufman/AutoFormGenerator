using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;

namespace AutoFormGenerator.Object
{
    public class FormControlSettings
    {

        public object Value => PropInfo.GetValue(Class);
        public string FieldName => ClassType.FullName + "." + PropInfo.Name;
        public FormField FormField => (FormField)PropInfo.GetCustomAttributes(typeof(FormField), true).FirstOrDefault();
        public bool FixedWidth => !double.IsNaN(DisplayNameWidth);
        public bool Required => FormField.Required;
        public bool CanEdit => FormField.CanEdit;
        public bool IsVisible => FormField.IsVisible;
        public string ToolTip => FormField.ToolTip;
        public double ControlWidth => (DisplayNameWidth + ValueWidth) + 50;


        public PropertyInfo PropInfo { get; set; }
        public object Class { get; set; }
        public Type ClassType { get; set; }
        public double DisplayNameWidth { get; set; } = 90;
        public double ControlHeight { get; set; } = 40;

        public string RequiredText => "This is a Required Field";


        public string DisplayValue
        {
            get
            {
                var displayValue = PropInfo.Name;

                if (FormField.DisplayName != string.Empty)
                {
                    displayValue = FormField.DisplayName;
                }

                if (FormField.Required)
                {
                    displayValue += "*";
                }

                return displayValue;
            }
        }
        public string PropertyType
        {
            get
            {
                var propertyType = PropInfo.PropertyType.Name;

                if (FormField.ObjectTypeName != ObjectTypes.Default)
                {
                    propertyType = FormField.ObjectTypeName.ToString();
                }

                return propertyType;
            }
        }

        private double _ValueWidth = 200;

        public double ValueWidth
        {
            get
            {
                double width = _ValueWidth;

                if (!double.IsNaN(FormField.ControlWidth))
                {
                    width = FormField.ControlWidth;
                }

                return width;
            }
            set
            {
                _ValueWidth = value;
            }
        }

        public bool SetValue(object Value)
        {
            PropInfo.SetValue(Class, Value);
            return true;
        }
    }
}
