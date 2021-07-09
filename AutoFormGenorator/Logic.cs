using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using AutoFormGenerator.Object;
using AutoFormGenerator.UserControls.Controls;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using UserControl = System.Windows.Controls.UserControl;

namespace AutoFormGenerator
{
    public class Logic
    {
        private event Events.Viladate OnValidate;
        public event Events.PropertyModified OnPropertyModified;
        public event Events.PropertyFinishedEditing OnPropertyFinishedEditing;

        private delegate void AddSpecialDropdownItems(string FieldName, List<FormDropdownItem> DropdownItems);
        private event AddSpecialDropdownItems OnAddSpecialDropdownItems;

        private delegate void SpecialDropdownDisplaying(string FieldName);
        private event SpecialDropdownDisplaying OnSpecialDropdownDisplaying;

        public UserControls.FormControl formControl;
        public bool Debug { get; set; }
        public bool HasChanged { get; set; } = false;

        private List<string> MDPacks = new List<string>
        {
            "pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.Defaults.xaml",
        };

        public Logic()
        {
            formControl = new UserControls.FormControl();

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

        public UserControls.FormControl BuildFormControl<T>(T Class)
        {
            if (Debug)
            {
                formControl.DebugStatsWrapPanel.Visibility = Visibility.Visible;
            }

            var sw = Stopwatch.StartNew();
            object RootClass = Class;
            if (Class is System.Collections.IList)
            {
                var wrapperClass = new WrapperClass
                {
                    List = Class
                };
                RootClass = wrapperClass;
            }

            formControl.Tag = RootClass;

            ProcessRootClass(RootClass, formControl);

            OnPropertyModified += Logic_OnPropertyModified;
            sw.Stop();

            if (Debug)
            {
                formControl.LoadTimeLabel.Content = $"Load Time: {sw.ElapsedMilliseconds}ms";

                sw.Restart();

                formControl.Loaded += (sender, args) =>
                {
                    formControl.DisplayTimeLabel.Content = $"Display Time: {sw.ElapsedMilliseconds}ms";
                    sw.Stop();
                };
            }

            return formControl;
        }

        private void Logic_OnPropertyModified(string fieldName, object Value)
        {
            HasChanged = true;
        }

        public void SubscribeToPropertyModified<T>(string fieldName, Events.PropertyModified a)
        {
            OnPropertyModified += (localFieldName, value) =>
            {
                var objectType = typeof(T);
                if (objectType.FullName + "." + fieldName == localFieldName)
                {
                    a.Invoke(localFieldName, value);
                }
            };
        }

        public void SubscribeToOnPropertyFinishedEditing<T>(string fieldName, Events.PropertyModified a)
        {
            OnPropertyFinishedEditing += (localFieldName, value) =>
            {
                var objectType = typeof(T);
                if (objectType.FullName + "." + fieldName == localFieldName)
                {
                    a.Invoke(localFieldName, value);
                }
            };
        }

        public void SubscribeToFieldFinishedEditing<T>(Events.PropertyModified a, params string[] fieldNames)
        {
            OnPropertyFinishedEditing += (localFieldName, value) =>
            {
                var objectType = typeof(T);
                foreach (var fieldName in fieldNames)
                {
                    if (objectType.FullName + "." + fieldName == localFieldName)
                    {
                        a.Invoke(localFieldName, value);
                    }
                }
            };
        }

        public bool Compile()
        {
            var valid = true;
            if (OnValidate == null) return valid;
            foreach (var d in OnValidate.GetInvocationList())
            {
                if (!(bool) d.DynamicInvoke(null))
                {
                    valid = false;
                }
            }

            return valid;
        }

        private void ProcessRootClass(object rootClass, UserControls.FormControl formControl)
        {
            var processedRootClass = ProcessClass(rootClass);
            foreach (var uc in processedRootClass)
            {
                var parent = new UserControls.Card();

                parent.ControlGrid.Children.Add(uc);
                formControl.DisplayStackPanel.Children.Add(parent);
            }
        }

        private List<UserControl> ProcessClass(object rootClass)
        {
            var sw = Stopwatch.StartNew();

            var userControls = new List<UserControl>();
            var rootClassType = rootClass.GetType();

            var rootFieldGroupCard = new UserControls.FieldGroupCard();

            var builtUserControls = BuildUserControls(rootClass, GetProprites(rootClassType, Types.Prop));

            builtUserControls.ForEach(control =>
            {
                rootFieldGroupCard.ControlsWrapPanel.Children.Add(control);
            });

            var formClass = rootClassType.GetCustomAttribute<FormClass>();
            var displayName = rootClassType.Name;

            if (formClass != null)
            {
                displayName = formClass.DisplayName;
            }

            rootFieldGroupCard.DisplayNameTextBlock.Text = displayName;

            if (builtUserControls.Count != 0)
            {
                userControls.Add(rootFieldGroupCard);
            }

            sw.Stop();
            if (Debug)
            {
                rootFieldGroupCard.DisplayNameTextBlock.Text += $" - Process Time: {sw.ElapsedMilliseconds}ms";
            }

            var handledNestedSettings = HandleNestedSettings(rootClass);

            userControls.AddRange(handledNestedSettings);
            userControls.AddRange(HandleNestedList(rootClass));

            return userControls;
        }

        private IEnumerable<UserControl> HandleNestedList(object rootClass)
        {
            var rootClassType = rootClass.GetType();

            var nestedLists = GetProprites(rootClassType, Types.NestedList);

            var userControls = new List<UserControl>();


            nestedLists.ForEach(async propInfo =>
            {
                var formField = (FormField)propInfo.GetCustomAttributes(typeof(FormField), true).FirstOrDefault();

                var rootFieldGroupCard = new UserControls.ListControls.GroupCard();

                var list = (System.Collections.IList)propInfo.GetValue(rootClass, null);

                var listType = propInfo.PropertyType.GetGenericArguments()[0];
                if (formField.NestedListClassType != null)
                {
                    listType = formField.NestedListClassType;
                }

                if (list == null)
                {
                    var constructedListType = typeof(List<>).MakeGenericType(listType);

                    list = Activator.CreateInstance(constructedListType) as System.Collections.IList;
                }

                var displayName = propInfo.Name;

                var fieldName = rootClassType.FullName + "." + propInfo.Name;

                if (formField.DisplayName != string.Empty)
                {
                    displayName = formField.DisplayName;
                }

                rootFieldGroupCard.AddItemIcon.MouseLeftButtonUp += async (s, e) =>
                {
                    var nestedItem = Activator.CreateInstance(listType);

                    list.Add(nestedItem);

                    OnPropertyModified?.Invoke(fieldName, nestedItem);

                    var addNewItem = AddNewListItem(nestedItem, list, rootFieldGroupCard, fieldName);

                    rootFieldGroupCard.ControlsWrapPanel.Children.Add(addNewItem);
                };

                rootFieldGroupCard.DisplayNameTextBlock.Text = displayName;
 
                foreach (var item in list)
                {
                    var addNewItem = AddNewListItem(item, list, rootFieldGroupCard, fieldName);
                    rootFieldGroupCard.ControlsWrapPanel.Children.Add(addNewItem);
                }
                userControls.Add(rootFieldGroupCard);
            });

            return userControls;
        }

        private UserControls.ListControls.Item AddNewListItem(object item, System.Collections.IList list, UserControls.ListControls.GroupCard rootFieldGroupCard, string feildName)
        {
            var listControlItem = new UserControls.ListControls.Item();

            var ProcessdClass = ProcessClass(item);

            ProcessdClass.ForEach(control =>
            {
                listControlItem.ControlWrapPanel.Children.Add(control);
            });

            listControlItem.DeleteItemIcon.MouseLeftButtonUp += (s, e) =>
            {
                var deleteItemMessageBox = new Windows.AFG_MessageBox("Remove Item?", "Are you sure you want to remove this?");
                deleteItemMessageBox.ShowDialog();
                if (deleteItemMessageBox.MessageBoxResult == MessageBoxResult.Yes)
                {
                    rootFieldGroupCard.ControlsWrapPanel.Children.Remove(listControlItem);
                    list.Remove(item);
                    OnPropertyModified?.Invoke(feildName, null);
                }
            };

            return listControlItem;
        }

        private List<UserControl> HandleNestedSettings(object rootClass)
        {
            var nestedSettings = GetProprites(rootClass.GetType(), Types.NestedSettings);

            var controls = new List<UserControl>();

            foreach (var propInfo in nestedSettings)
            {
                var nestedSettingsClass = propInfo.GetValue(rootClass);

                if (nestedSettingsClass == null)
                {
                    var formField = (FormField)propInfo.GetCustomAttributes(typeof(FormField), true).FirstOrDefault();

                    var listType = propInfo.PropertyType;
                    if (formField.NestedClassType != null)
                    {
                        listType = formField.NestedClassType;
                    }
                    
                    nestedSettingsClass = Activator.CreateInstance(listType);
                    propInfo.SetValue(rootClass, nestedSettingsClass);
                }

                var ProcessedClass = ProcessClass(nestedSettingsClass);

                controls.AddRange(ProcessedClass);
            }

            return controls;
        }

        private List<UserControl> BuildUserControls(object Class, List<PropertyInfo> props)
        {
            var userControls = new List<UserControl>();

            var SortedControls = new List<PropInfoSorterClass>();

            var classType = Class.GetType();

            double displayNameWidth = 0;
            double valueWidth = 100;
            foreach (var propInfo in props)
            {
                var formField = (FormField)propInfo.GetCustomAttributes(typeof(FormField), true).FirstOrDefault();

                var displayValue = propInfo.Name;

                if (formField.DisplayName != string.Empty)
                {
                    displayValue = formField.DisplayName;
                }
                
                if ((displayValue.Length * 10) > displayNameWidth)
                {
                    displayNameWidth = (displayValue.Length * 10);
                }

                SortedControls.Add(new PropInfoSorterClass()
                {
                    Order = formField.Order,
                    PropertyInfo = propInfo
                });
            }

            if (Attribute.IsDefined(classType, typeof(FormClass)))
            {
                var formClass = classType.GetCustomAttribute<FormClass>();

                if (formClass.FormValueWidth != -1)
                {
                    valueWidth = formClass.FormValueWidth;
                }

                if (formClass.WindthOveride)
                {
                    displayNameWidth = Double.NaN;
                }
            }

            var PropInfoSorterClasses = SortedControls.OrderBy(a => a.Order).ToList();

            foreach (var propInfo in PropInfoSorterClasses)
            {
                var formControlSettings = new FormControlSettings
                {
                    DisplayNameWidth = displayNameWidth,
                    ValueWidth = valueWidth,
                    PropInfo = propInfo.PropertyInfo,
                    Class = Class,
                    ClassType = classType
                };

                var userControl = HandleUserControl(formControlSettings);
                if (userControl != null)
                {
                    userControls.Add(userControl);
                }
            }

            PropInfoSorterClasses.ForEach(clase =>
            {
                OnPropertyModified?.Invoke(classType.FullName + "." + clase.PropertyInfo.Name, clase.PropertyInfo.GetValue(Class));
            });

            return userControls;
        }

        private UserControl HandleUserControl(FormControlSettings formControlSettings)
        {
            var control = BuildControl(formControlSettings);

            if (control != null)
            {
                var FieldConditions = GetFieldConditions(formControlSettings.ClassType, formControlSettings.PropInfo.Name);

                FieldConditions.ForEach(info =>
                {
                    var FieldCondition = (FieldCondition)info.GetCustomAttributes(typeof(FieldCondition), true).FirstOrDefault();

                    OnPropertyModified += (name, value) =>
                    {
                        HandleFieldConditions(FieldCondition, formControlSettings.ClassType, name, value, control);
                    };
                });

                return control;
            }

            return null;
        }

        private void HandleFieldConditions(FieldCondition FieldCondition, Type classType, string name, object value, UserControl control)
        {
            if (FieldCondition != null)
            {
                var filedName = classType.FullName + "." + FieldCondition.Field;
                if (name == filedName)
                {
                    if (CanConditionalDisplay(FieldCondition, value.ToString()) == false)
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

        private UserControl BuildControl(FormControlSettings formControlSettings)
        {
            UserControl userControl = null;

            if (!Enum.IsDefined(typeof(ObjectTypes), formControlSettings.PropertyType))
            {
                return userControl;
            }

            switch (Enum.Parse(typeof(ObjectTypes), formControlSettings.PropertyType))
            {
                case ObjectTypes.String:
                    var stringField = new StringField();

                    stringField.BuildDisplay(formControlSettings);

                    if (formControlSettings.Required)
                    {
                        OnValidate += stringField.Validate;
                    }

                    stringField.OnPropertyModified += s => OnPropertyModified?.Invoke(formControlSettings.FieldName, s);
                    stringField.OnPropertyFinishedEditing += s => OnPropertyFinishedEditing?.Invoke(formControlSettings.FieldName, s);

                    userControl = stringField;
                    break;
                case ObjectTypes.Password:
                    var passwordField = new PasswordField();

                    passwordField.BuildDisplay(formControlSettings);

                    if (formControlSettings.Required)
                    {
                        OnValidate += passwordField.Validate;
                    }

                    passwordField.OnPropertyModified += s => OnPropertyModified?.Invoke(formControlSettings.FieldName, s);
                    passwordField.OnPropertyFinishedEditing += s => OnPropertyFinishedEditing?.Invoke(formControlSettings.FieldName, s);

                    userControl = passwordField;
                    break;
                case ObjectTypes.Double:
                    var doubleField = new DoubleField();

                    doubleField.BuildDisplay(formControlSettings);

                    if (formControlSettings.Required)
                    {
                        OnValidate += doubleField.Validate;
                    }

                    doubleField.OnPropertyModified += s => OnPropertyModified?.Invoke(formControlSettings.FieldName, s);
                    doubleField.OnPropertyFinishedEditing += s => OnPropertyFinishedEditing?.Invoke(formControlSettings.FieldName, s);

                    userControl = doubleField;
                    break;
                case ObjectTypes.Int32:
                    var intField = new IntField();

                    intField.BuildDisplay(formControlSettings);

                    if (formControlSettings.Required)
                    {
                        OnValidate += intField.Validate;
                    }

                    intField.OnPropertyModified += s => OnPropertyModified?.Invoke(formControlSettings.FieldName, s);
                    intField.OnPropertyFinishedEditing += s => OnPropertyFinishedEditing?.Invoke(formControlSettings.FieldName, s);

                    userControl = intField;
                    break;
                case ObjectTypes.Single:
                    var floatField = new FloatField();

                    floatField.BuildDisplay(formControlSettings);

                    if (formControlSettings.Required)
                    {
                        OnValidate += floatField.Validate;
                    }

                    floatField.OnPropertyModified += s => OnPropertyModified?.Invoke(formControlSettings.FieldName, s);
                    floatField.OnPropertyFinishedEditing += s => OnPropertyFinishedEditing?.Invoke(formControlSettings.FieldName, s);

                    userControl = floatField;
                    break;
                case ObjectTypes.Boolean:
                    var booleanField = new BooleanField();

                    booleanField.BuildDisplay(formControlSettings);

                    if (formControlSettings.Required)
                    {
                        OnValidate += booleanField.Validate;
                    }

                    booleanField.OnPropertyModified += s => OnPropertyModified?.Invoke(formControlSettings.FieldName, s);
                    booleanField.OnPropertyFinishedEditing += s => OnPropertyFinishedEditing?.Invoke(formControlSettings.FieldName, s);

                    userControl = booleanField;
                    break;
                case ObjectTypes.ObjectDropdown:
                    var dropdownField = new DropdownField();

                    dropdownField.BuildDisplay(formControlSettings, BuildDropdownItems(formControlSettings.FormField.DropDownClass));

                    dropdownField.OnPropertyModified += s => OnPropertyModified?.Invoke(formControlSettings.FieldName, s);
                    dropdownField.OnPropertyFinishedEditing += s => OnPropertyFinishedEditing?.Invoke(formControlSettings.FieldName, s);

                    userControl = dropdownField;
                    break;
                case ObjectTypes.SpecialDropdown:
                    var specialDropdown = new SpecialDropdownField();

                    specialDropdown.BuildDisplay(formControlSettings);

                    OnAddSpecialDropdownItems += (Name, Items) =>
                    {
                        if (Name == formControlSettings.FieldName)
                        {
                            specialDropdown.AddDropdownItems(Items, formControlSettings.Value);
                        }
                    };

                    OnSpecialDropdownDisplaying?.Invoke(formControlSettings.FieldName);

                    userControl = specialDropdown;
                    break;
                case ObjectTypes.FolderBrowser:
                    var folderStringField = new FolderBrowserField(displayValue, (string) value)
                    {
                        Width = controlWidth,
                        Height = controlHeight
                    };

                    if (FixedWidth)
                    {
                        folderStringField.DisplayNameTextBlock.Width = displayNameWidth;
                    }
                    else
                    {
                        folderStringField.Margin = new Thickness(0, 0, 30, 0);
                    }

                    folderStringField.ValueTextBox.Width = valueWidth;
                    if (formField.Required)
                    {
                        folderStringField.DisplayNameTextBlock.ToolTip = "This is a Required Field";
                        OnValidate += folderStringField.Validate;
                    }

                    if (formField.ToolTip != string.Empty)
                    {
                        folderStringField.ValueTextBox.ToolTip = formField.ToolTip;
                    }

                    folderStringField.ValueTextBox.TextChanged += (sen, e) =>
                    {
                        propInfo.SetValue(Class, folderStringField.ValueTextBox.Text);

                        OnPropertyModified?.Invoke(fieldName, folderStringField.ValueTextBox.Text);
                        OnPropertyFinishedEditing?.Invoke(fieldName, folderStringField.ValueTextBox.Text);
                    };
                    userControl = folderStringField;
                    break;
                case ObjectTypes.TimePicker:
                    var TimePickerField = new TimePickerField(displayValue, (double) value)
                    {
                        Width = controlWidth,
                        Height = controlHeight
                    };

                    if (FixedWidth)
                    {
                        TimePickerField.DisplayNameTextBlock.Width = displayNameWidth;
                    }
                    else
                    {
                        TimePickerField.Margin = new Thickness(0, 0, 30, 0);
                    }

                    TimePickerField.TimePicker.Width = valueWidth;
                    if (formField.Required)
                    {
                        TimePickerField.DisplayNameTextBlock.ToolTip = "This is a Required Field";
                        
                    }
                    if (formField.ToolTip != string.Empty)
                    {
                        TimePickerField.TimePicker.ToolTip = formField.ToolTip;
                    }

                    TimePickerField.TimePicker.SelectedTimeChanged += (sender, args) =>
                    {
                        var CurrentValue = TimePickerField.TimePicker.SelectedTime?.TimeOfDay.TotalSeconds ?? 0;

                        propInfo.SetValue(Class, CurrentValue);

                        OnPropertyModified?.Invoke(fieldName, CurrentValue);
                        OnPropertyFinishedEditing?.Invoke(fieldName, CurrentValue);
                    };

                    userControl = TimePickerField;
                    break;
                case ObjectTypes.Custom:
                    if (formField.CustomControl != null && formField.CustomControl.GetInterface(typeof(Interfaces.ICustomControl).FullName) != null) 
                    {
                        var customControlClass = (Interfaces.ICustomControl)Activator.CreateInstance(formField.CustomControl);
                        if (!(customControlClass is UserControl customControl))
                        {
                            break;
                        }

                        var customControlBase = new UserControls.Controls.CustomControlBase(displayValue)
                        {
                            Width = controlWidth,
                            Height = controlHeight
                        };

                        if (FixedWidth)
                        {
                            customControlBase.DisplayNameTextBlock.Width = displayNameWidth;
                        }
                        else
                        {
                            customControlBase.Margin = new Thickness(0, 0, 30, 0);
                        }

                        customControl.Width = valueWidth;
                        customControlClass.SetValue(value);

                        if (formField.Required)
                        {
                            customControlBase.DisplayNameTextBlock.ToolTip = "This is a Required Field";
                        }

                        customControlBase.CustomControlCanvas.Children.Add(customControl);

                        customControlClass.OnPropertyFinishedEditing += (name, o) =>
                        {
                            OnPropertyFinishedEditing?.Invoke(fieldName, o);
                        };

                        customControlClass.OnPropertyModified += (name, o) =>
                        {
                            propInfo.SetValue(Class, o);
                            OnPropertyModified?.Invoke(fieldName, o);
                        };

                        userControl = customControlBase;
                    }
                    break;
            }

            return userControl;
        }

        private bool CanConditionalDisplay(FieldCondition condition, string fieldValue)
        {
            switch (condition.Operator)
            {
                case Operators.Equals:
                    if (condition.Value.ToString() != fieldValue)
                    {
                        return false;
                    }
                    break;
            }

            return true;
        }

        private List<DropdownField.DropdownItem> BuildDropdownItems(Type Class)
        {
            var dropClass = Activator.CreateInstance(Class);

            var dropdownItems = new List<DropdownField.DropdownItem>();

            var rawPropertyInfos = new List<PropertyInfo>(Class.GetProperties().Where(a => a.GetCustomAttributes<FormDropdownItem>().Count() != 0));

            rawPropertyInfos.ForEach(prop =>
            {
                var formDropdownItem = prop.GetCustomAttribute<FormDropdownItem>();

                var displayName = prop.Name;

                if (formDropdownItem.DisplayValue != string.Empty)
                {
                    displayName = formDropdownItem.DisplayValue;
                }

                var value = (string) formDropdownItem.Value != string.Empty ? formDropdownItem.Value : prop.GetValue(dropClass).ToString();

                dropdownItems.Add(new DropdownField.DropdownItem()
                {
                    Name = displayName,
                    Value = value.ToString()
                });
            });

            return dropdownItems;
        }

        public void PopulateSpecialDropdown<T>(string FieldName, List<FormDropdownItem> DropdownItems)
        {
            var objectType = typeof(T);

            OnAddSpecialDropdownItems?.Invoke(objectType.FullName + "." + FieldName, DropdownItems);

            OnSpecialDropdownDisplaying += name =>
            {
                if (name == objectType.FullName + "." + FieldName)
                {
                    OnAddSpecialDropdownItems?.Invoke(objectType.FullName + "." + FieldName, DropdownItems);
                }
            };
            
        }

        private List<PropertyInfo> GetProprites(Type baseType, Types type)
        {
            var rawPropertyInfos = new List<PropertyInfo>(baseType.GetProperties());

            var propertyInfo = new List<PropertyInfo>(rawPropertyInfos.Where(a => a.GetCustomAttributes(typeof(FormField), true).Any(b => ((FormField)b).Type == type)));

            return propertyInfo;
        }

        private List<PropertyInfo> GetFieldConditions(Type baseType, string fieldName)
        {
            var rawPropertyInfos = new List<PropertyInfo>(baseType.GetProperties());

            var propertyInfo = new List<PropertyInfo>(rawPropertyInfos.Where(a => a.Name == fieldName).Where(a => a.GetCustomAttributes(typeof(FieldCondition), true).Any()));

            return propertyInfo;
        }

        private class WrapperClass
        {
            [FormField(Type = Types.NestedList)]
            public object List { get; set; }
        }

        private class PropInfoSorterClass
        {
            public int Order { get; set;}
            public PropertyInfo PropertyInfo { get; set; }
        }
    }
}
