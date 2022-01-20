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

        //public object Value => PropInfo.GetValue(Class);
        public object Value { get; set; }
        public string FieldFullName => ClassType.FullName + "." + FieldName;
        //public FormField FormField => (FormField)PropInfo.GetCustomAttributes(typeof(FormField), true).FirstOrDefault();
        public FormField FormField { get; set; }
        public bool FixedWidth => !double.IsNaN(DisplayNameWidth);
        public bool Required => FormField.Required;
        public bool CanEdit => FormField.CanEdit;
        public bool IsVisible => FormField.IsVisible;
        public string ToolTip => FormField.ToolTip;
        public double ControlWidth => (DisplayNameWidth + ValueWidth) + 50;
        public string Regex => FormField.Regex;


        //public PropertyInfo PropInfo { get; set; }
        //public object Class { get; set; }
        public Type ClassType { get; set; }
        public double DisplayNameWidth { get; set; } = 90;
        public double ControlHeight { get; set; } = 40;
        public ObjectTypes ObjectType { get; set; }
        public string FieldName { get; set; }

        public string RequiredText => "This is a Required Field";

        public delegate void ValueChanged(object value);
        public event ValueChanged OnValueChanged;


        public string DisplayValue
        {
            get
            {
                var displayValue = FieldName;

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
        

        private double _ValueWidth = 100;

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
            this.Value = Value;
            OnValueChanged?.Invoke(Value);
            return true;
            /*
            try
            {
                PropInfo.SetValue(Class, Value);
            }
            catch
            {
                return false;
            }

            return true;
            */
        }
    }
}
