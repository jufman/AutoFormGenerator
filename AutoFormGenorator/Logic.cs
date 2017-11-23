using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using MaterialDesignThemes.Wpf;
using System.Windows;

namespace AutoFormGenorator
{
    public class Logic
    {
        private event Events.Viladate OnViladate;

        public UserControls.FormControl BuildFormControl<T>(T RootClass)
        {
            UserControls.FormControl FormControl = new UserControls.FormControl();

            FormControl.Tag = RootClass;

            ProcressRootClass(RootClass, FormControl);

            return FormControl;
        }

        public bool Compile()
        {
            bool Valid = true;
            foreach (Delegate d in OnViladate.GetInvocationList())
            {
                if (!(bool)d.DynamicInvoke(null))
                {
                    Valid = false;
                }
               
            }
            return Valid;
        }

        private void ProcressRootClass(object RootClass, UserControls.FormControl FormControl)
        {
            ProcressClass(RootClass).ForEach(UC => {
                UserControls.Card Parrent = new UserControls.Card();
                Parrent.ControlGrid.Children.Add(UC);
                FormControl.DisplayStactPanel.Children.Add(Parrent);
            });
        }

        private List<UserControl> ProcressClass(object RootClass)
        {
            List<UserControl> UserControls = new List<UserControl>();

            UserControls.FieldGroupCard RootFieldGroupCard = new UserControls.FieldGroupCard();

            BuildUserControls(RootClass, GetProprites(RootClass.GetType(), Object.Types.Prop)).ForEach(Control =>
            {
                RootFieldGroupCard.ControlsWrapPanel.Children.Add(Control);
            });

            Object.FormClass FormClass = RootClass.GetType().GetCustomAttribute<Object.FormClass>();
            string DisplayName = RootClass.GetType().Name;

            if (FormClass != null)
            {
                DisplayName = FormClass.DisplayName;
            }

            RootFieldGroupCard.DisplayNameTextBlock.Text = DisplayName;

            UserControls.Add(RootFieldGroupCard);

            UserControls.AddRange(HandleNestedSettings(RootClass));
            UserControls.AddRange(HandleNestedList(RootClass));

            return UserControls;
        }

        private List<UserControl> HandleNestedList(object RootClass)
        {
            List<PropertyInfo> NestedLists = GetProprites(RootClass.GetType(), Object.Types.NestedList);

            List<UserControl> UserControls = new List<UserControl>();

            NestedLists.ForEach(PropInfo =>
            {
                Object.FormClass FormClass = RootClass.GetType().GetCustomAttribute<Object.FormClass>();

                UserControls.ListControls.GroupCard RootFieldGroupCard = new UserControls.ListControls.GroupCard();

                System.Collections.IList IList = (System.Collections.IList)PropInfo.GetValue(RootClass, null);

                Type ListType = PropInfo.PropertyType.GetGenericArguments()[0];
                string DisplayName = ListType.Name;

                if (FormClass != null)
                {
                    DisplayName = FormClass.DisplayName;
                }

                RootFieldGroupCard.AddItemIcon.MouseLeftButtonUp += (s, e) =>
                {
                    object NestedItem = Activator.CreateInstance(ListType);

                    IList.Add(NestedItem);

                    RootFieldGroupCard.ControlsWrapPanel.Children.Add(AddNewListItem(NestedItem, IList, RootFieldGroupCard));
                };

                RootFieldGroupCard.DisplayNameTextBlock.Text = DisplayName;
 
                foreach (object item in IList)
                {
                    RootFieldGroupCard.ControlsWrapPanel.Children.Add(AddNewListItem(item, IList, RootFieldGroupCard));
                }
                UserControls.Add(RootFieldGroupCard);
            });

            return UserControls;
        }

        private UserControls.ListControls.Item AddNewListItem(object item, System.Collections.IList IList, UserControls.ListControls.GroupCard RootFieldGroupCard)
        {
            UserControls.ListControls.Item ListControlItem = new UserControls.ListControls.Item();

            ProcressClass(item).ForEach(Control =>
            {
                ListControlItem.ControlWrapPanel.Children.Add(Control);
            });

            ListControlItem.DeleteItemIcon.MouseLeftButtonUp += (s, e) =>
            {
                if (MessageBox.Show("Are you sure you want to remove this?", "Remove Item?", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    RootFieldGroupCard.ControlsWrapPanel.Children.Remove(ListControlItem);
                    IList.Remove(item);
                }
            };

            return ListControlItem;
        }

        private List<UserControl> HandleNestedSettings(object RootClass)
        {
            List<PropertyInfo> NestedSettings = GetProprites(RootClass.GetType(), Object.Types.NestedSettings);

            List<UserControl> Controls = new List<UserControl>();

            foreach (PropertyInfo PropInfo in NestedSettings)
            {
                object NestedSettingsClass = PropInfo.GetValue(RootClass);

                if (NestedSettingsClass == null)
                {
                    Type ListType = PropInfo.PropertyType;
                    NestedSettingsClass = Activator.CreateInstance(ListType);
                }

                Controls.AddRange(ProcressClass(NestedSettingsClass));
            }

            return Controls;
        }

        private List<UserControl> BuildUserControls(object Class, List<PropertyInfo> props)
        {
            List<UserControl> UserControls = new List<UserControl>();

            double DisplayNameWidth = 0;
            double ValueWidth = 100;
            props.ForEach(PropInfo =>
            {
                Object.FormField FormField = (Object.FormField)PropInfo.GetCustomAttributes(typeof(Object.FormField), true).FirstOrDefault();

                string DisplayValue = PropInfo.Name;

                if (FormField.DisplayName != string.Empty)
                {
                    DisplayValue = FormField.DisplayName;
                }
                
                if ((DisplayValue.Length * 10) > DisplayNameWidth)
                {
                    DisplayNameWidth = (DisplayValue.Length * 10);
                }
            });

            if (Attribute.IsDefined(Class.GetType(), typeof(Object.FormClass)))
            {
                Object.FormClass FormClass = Class.GetType().GetCustomAttribute<Object.FormClass>();

                if (FormClass.FormValueWidth != -1)
                {
                    ValueWidth = FormClass.FormValueWidth;
                }
            }
            
            props.ForEach(PropInfo =>
            {
                UserControl Control = BuildControl(PropInfo, Class, DisplayNameWidth, ValueWidth);

                if (Control != null)
                {
                    UserControls.Add(Control);
                }
            });

            return UserControls;
        }
            
        private UserControl BuildControl(PropertyInfo PropInfo, object Class, double DisplayNameWidth = 90, double ValueWidth = 100)
        {
            double ControlWidth = (DisplayNameWidth + ValueWidth) + 50;
            double ControlHeight = 40;

            Object.FormField FormField = (Object.FormField)PropInfo.GetCustomAttributes(typeof(Object.FormField), true).FirstOrDefault();

            string DisplayValue = PropInfo.Name;

            if (FormField.DisplayName != string.Empty)
            {
                DisplayValue = FormField.DisplayName;
            }

            if (FormField.Required)
            {
                DisplayValue += "*";
            }

            string PropertyType = PropInfo.PropertyType.Name;

            if (FormField.ObjectTypeName != string.Empty)
            {
                PropertyType = FormField.ObjectTypeName;
            }

            //var obj = Activator.CreateInstance(Class.GetType());
            //object jh = PropInfo.GetValue(obj, new object[] { });

            switch (PropertyType)
            {
                case "String":
                    UserControls.Controls.StringField StringField = new UserControls.Controls.StringField(DisplayValue, (string)PropInfo.GetValue(Class));
                    StringField.Width = ControlWidth;
                    StringField.Height = ControlHeight;
                    StringField.DisplayNameTextBlock.Width = DisplayNameWidth;
                    StringField.ValueTextBox.Width = ValueWidth;
                    if (FormField.Required)
                    {
                        StringField.DisplayNameTextBlock.ToolTip = "This is a Required Field";
                        OnViladate += StringField.Viladate;
                    }
                    if (FormField.ToolTip != string.Empty)
                    {
                        StringField.ValueTextBox.ToolTip = FormField.ToolTip;
                    }
                    StringField.ValueTextBox.TextChanged += (sen, e) =>
                    {
                        PropInfo.SetValue(Class, StringField.ValueTextBox.Text);
                    };
                    return StringField;
                case "Double":
                    UserControls.Controls.DoubleField DoubleField = new UserControls.Controls.DoubleField(DisplayValue, (Double)PropInfo.GetValue(Class));
                    DoubleField.Width = ControlWidth;
                    DoubleField.Height = ControlHeight;
                    DoubleField.DisplayNameTextBlock.Width = DisplayNameWidth;
                    DoubleField.ValueTextBox.Width = ValueWidth;
                    if (FormField.Required)
                    {
                        DoubleField.DisplayNameTextBlock.ToolTip = "This is a Required Field";
                        OnViladate += DoubleField.Viladate;
                    }
                    if (FormField.ToolTip != string.Empty)
                    {
                        DoubleField.ValueTextBox.ToolTip = FormField.ToolTip;
                    }
                    DoubleField.ValueTextBox.TextChanged += (sen, e) =>
                    {
                        double DoubleValue = 0;
                        if (double.TryParse(DoubleField.ValueTextBox.Text, out DoubleValue))
                        {
                            PropInfo.SetValue(Class, DoubleValue);
                        }
                    };
                    return DoubleField;
                case "Int32":
                    UserControls.Controls.IntField IntField = new UserControls.Controls.IntField(DisplayValue, (int)PropInfo.GetValue(Class));
                    IntField.Width = ControlWidth;
                    IntField.Height = ControlHeight;
                    IntField.DisplayNameTextBlock.Width = DisplayNameWidth;
                    IntField.ValueTextBox.Width = ValueWidth;
                    if (FormField.Required)
                    {
                        IntField.DisplayNameTextBlock.ToolTip = "This is a Required Field";
                        OnViladate += IntField.Viladate;
                    }
                    if (FormField.ToolTip != string.Empty)
                    {
                        IntField.ValueTextBox.ToolTip = FormField.ToolTip;
                    }
                    IntField.ValueTextBox.TextChanged += (sen, e) =>
                    {
                        int IntValue = 0;
                        if (int.TryParse(IntField.ValueTextBox.Text, out IntValue))
                        {
                            PropInfo.SetValue(Class, IntValue);
                        }
                    };
                    return IntField;
                case "Single":
                    UserControls.Controls.FloatField FloatField = new UserControls.Controls.FloatField(DisplayValue, (float)PropInfo.GetValue(Class));
                    FloatField.Width = ControlWidth;
                    FloatField.Height = ControlHeight;
                    FloatField.DisplayNameTextBlock.Width = DisplayNameWidth;
                    FloatField.ValueTextBox.Width = ValueWidth;
                    if (FormField.Required)
                    {
                        FloatField.DisplayNameTextBlock.ToolTip = "This is a Required Field";
                        OnViladate += FloatField.Viladate;
                    }
                    if (FormField.ToolTip != string.Empty)
                    {
                        FloatField.ValueTextBox.ToolTip = FormField.ToolTip;
                    }
                    FloatField.ValueTextBox.TextChanged += (sen, e) =>
                    {
                        float FloatValue = 0;
                        if (float.TryParse(FloatField.ValueTextBox.Text, out FloatValue))
                        {
                            PropInfo.SetValue(Class, FloatValue);
                        }
                    };
                    return FloatField;
                case "Boolean":
                    UserControls.Controls.BooleanField BooleanField = new UserControls.Controls.BooleanField(DisplayValue, (bool)PropInfo.GetValue(Class));
                    BooleanField.Width = ControlWidth;
                    BooleanField.Height = ControlHeight;
                    BooleanField.DisplayNameTextBlock.Width = DisplayNameWidth;
                    if (FormField.Required)
                    {
                        BooleanField.DisplayNameTextBlock.ToolTip = "This is a Required Field";
                        OnViladate += BooleanField.Viladate;
                    }
                    if (FormField.ToolTip != string.Empty)
                    {
                        BooleanField.ValueCheckBox.ToolTip = FormField.ToolTip;
                    }
                    BooleanField.ValueCheckBox.Checked += (sen, e) =>
                    {
                        PropInfo.SetValue(Class, BooleanField.ValueCheckBox.IsChecked);
                    };
                    BooleanField.ValueCheckBox.Unchecked += (sen, e) =>
                    {
                        PropInfo.SetValue(Class, BooleanField.ValueCheckBox.IsChecked);
                    };
                    return BooleanField;
                case "ColourPicker":
                    UserControls.Controls.ColourPickerField ColourPickerField = new UserControls.Controls.ColourPickerField(DisplayValue, (string)PropInfo.GetValue(Class));
                    ColourPickerField.Width = ControlWidth;
                    ColourPickerField.Height = ControlHeight;
                    ColourPickerField.DisplayNameTextBlock.Width = DisplayNameWidth;
                    if (FormField.Required)
                    {
                        ColourPickerField.DisplayNameTextBlock.ToolTip = "This is a Required Field";
                        OnViladate += ColourPickerField.Viladate;
                    }
                    if (FormField.ToolTip != string.Empty)
                    {
                        ColourPickerField.ValueColourPicker.ToolTip = FormField.ToolTip;
                    }
                    ColourPickerField.ValueColourPicker.SelectedColorChanged += (sen, e) =>
                    {
                        PropInfo.SetValue(Class, ColourPickerField.ValueColourPicker.SelectedColor.ToString());
                    };
                    return ColourPickerField;
                case "ObjectDropdown":
                    UserControls.Controls.DropdownField DropdownField = new UserControls.Controls.DropdownField(DisplayValue, BuildDropdownItems(FormField.DropDownClass), (string)PropInfo.GetValue(Class));
                    DropdownField.Width = ControlWidth;
                    DropdownField.Height = ControlHeight;
                    DropdownField.DisplayNameTextBlock.Width = DisplayNameWidth;
                    DropdownField.SelectComboBox.Width = ValueWidth;
                    DropdownField.SelectComboBox.SelectionChanged += (sen, e) =>
                    {
                        ComboBoxItem SelectedItem = (ComboBoxItem) DropdownField.SelectComboBox.SelectedItem;
                        PropInfo.SetValue(Class, SelectedItem.Tag.ToString());
                    };
                    return DropdownField;
            }

            return null;
        }

        private List<UserControls.Controls.DropdownField.DropdownItem> BuildDropdownItems(Type Class)
        {
            object DropClass = Activator.CreateInstance(Class);

            List<UserControls.Controls.DropdownField.DropdownItem> DropdownItems = new List<UserControls.Controls.DropdownField.DropdownItem>();

            List<PropertyInfo> RawPropertyInfos = new List<PropertyInfo>(Class.GetProperties().Where(A => A.GetCustomAttributes<Object.FormDropdownItem>().Count() != 0));

            RawPropertyInfos.ForEach(Prop =>
            {
                Object.FormDropdownItem FormDropdownItem = (Object.FormDropdownItem)Prop.GetCustomAttribute< Object.FormDropdownItem>();

                string DisplayName = Prop.Name;
                string Value = Prop.GetValue(DropClass).ToString();

                if (FormDropdownItem.DisplayValue != string.Empty)
                {
                    DisplayName = FormDropdownItem.DisplayValue;
                }

                if (FormDropdownItem.Value != string.Empty)
                {
                    Value = FormDropdownItem.Value;
                }

                DropdownItems.Add(new UserControls.Controls.DropdownField.DropdownItem()
                {
                    Name = DisplayName,
                    Value = Value
                });
            });

            return DropdownItems;
        }

        private List<PropertyInfo> GetProprites(Type BaseType, Object.Types Type)
        {
            List<PropertyInfo> Props = new List<PropertyInfo>();

            List<PropertyInfo> RawPropertyInfos = new List<PropertyInfo>(BaseType.GetProperties());

            List<PropertyInfo> PropertyInfo = new List<PropertyInfo>(RawPropertyInfos.Where(A => A.GetCustomAttributes(typeof(Object.FormField), true).Where(B => ((Object.FormField)B).Type == Type).Any()));

            return PropertyInfo;
        }

    }
}
