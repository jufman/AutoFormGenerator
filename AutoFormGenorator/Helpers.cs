using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using AutoFormGenerator.Object;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;

namespace AutoFormGenerator
{
    public static class Helpers
    {
        private static List<string> MDPacks = new List<string>
        {
            "pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.Defaults.xaml",
        };


        public static List<PropertyInfo> GetProperties(Type baseType, Types type)
        {
            var rawPropertyInfos = new List<PropertyInfo>(baseType.GetProperties());

            var propertyInfo = new List<PropertyInfo>(rawPropertyInfos.Where(a => a.GetCustomAttributes(typeof(FormField), true).Any(b => ((FormField)b).Type == type)));

            return propertyInfo;
        }

        public static List<PropertyInfo> GetFieldConditions(Type baseType, string fieldName)
        {
            var rawPropertyInfos = new List<PropertyInfo>(baseType.GetProperties());

            var propertyInfo = new List<PropertyInfo>(rawPropertyInfos.Where(a => a.Name == fieldName).Where(a => a.GetCustomAttributes(typeof(FieldCondition), true).Any()));

            return propertyInfo;
        }


        public static void HandleFieldConditions(FieldCondition FieldCondition, Type classType, string name, object value, UserControl control)
        {
            if (FieldCondition != null)
            {
                var filedName = classType.FullName + "." + FieldCondition.Field;
                if (name == filedName)
                {
                    if (Helpers.CanConditionalDisplay(FieldCondition, value) == false)
                    {
                        control.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        control.Visibility = Visibility.Visible;
                    }
                }
            }
        }

        private static bool CanConditionalDisplay(FieldCondition condition, object fieldValue)
        {
            if (fieldValue == null)
            {
                return false;
            }

            switch (condition.Operator)
            {
                case Operators.Equals:
                    if (condition.Value.ToString() != fieldValue.ToString())
                    {
                        return false;
                    }
                    break;
            }

            return true;
        }

        public static void ApplyMD(UserControls.FormControl formControl)
        {
            var md = formControl.Resources.MergedDictionaries;

            MDPacks.ForEach(pack =>
            {
                var resourceDictionary = md.Any(dictionary => dictionary.Source.AbsoluteUri == pack);

                if (resourceDictionary == false)
                {
                    var mr1 = new ResourceDictionary
                    {
                        Source = new Uri(
                            pack,
                            UriKind.Absolute)
                    };
                    formControl.Resources.MergedDictionaries.Add(mr1);
                }
            });

            var BI = new BundledTheme
            {
                BaseTheme = BaseTheme.Dark,
                PrimaryColor = PrimaryColor.Grey,
                SecondaryColor = SecondaryColor.Orange
            };
            formControl.Resources.MergedDictionaries.Add(BI);
        }

    }
}
