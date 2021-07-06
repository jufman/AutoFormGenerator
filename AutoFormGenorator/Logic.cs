using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using AutoFormGenerator.Object;
using AutoFormGenerator.UserControls.Controls;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;

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
            }

            var PropInfoSorterClasses = SortedControls.OrderBy(a => a.Order).ToList();

            foreach (var propInfo in PropInfoSorterClasses)
            {
                var userControl = HandleUserControl(propInfo, Class, displayNameWidth, valueWidth, classType);
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

        private UserControl HandleUserControl(PropInfoSorterClass propInfo, object Class, double displayNameWidth, double valueWidth, Type classType)
        {
            var _propInfo = propInfo.PropertyInfo;

            var control = BuildControl(_propInfo, Class, classType, displayNameWidth, valueWidth);

            if (control != null)
            {
                var FieldConditions = GetFieldConditions(classType, propInfo.PropertyInfo.Name);

                FieldConditions.ForEach(info =>
                {
                    var FieldCondition = (FieldCondition)info.GetCustomAttributes(typeof(FieldCondition), true).FirstOrDefault();

                    OnPropertyModified += (name, value) =>
                    {
                        HandleFieldConditions(FieldCondition, classType, name, value, control);
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

        private UserControl BuildControl(PropertyInfo propInfo, object Class, Type classType, double displayNameWidth = 90, double valueWidth = 100)
        {
            var propertyType = string.Empty;
            UserControl userControl = null;

            var fieldName = string.Empty;
            object value = string.Empty;

            double controlWidth = 0;
            double controlHeight = 0;

            var displayValue = string.Empty;
            FormField formField = null;

            

            formField = (FormField) propInfo.GetCustomAttributes(typeof(FormField), true).FirstOrDefault();

            displayValue = propInfo.Name;

            if (!double.IsNaN(formField.ControlWidth))
            {
                valueWidth = formField.ControlWidth;
            }

            controlWidth = (displayNameWidth + valueWidth) + 50;
            controlHeight = 40;

            if (formField.DisplayName != string.Empty)
            {
                displayValue = formField.DisplayName;
            }

            if (formField.Required)
            {
                displayValue += "*";
            }

            propertyType = propInfo.PropertyType.Name;

            if (formField.ObjectTypeName != ObjectTypes.Default)
            {
                propertyType = formField.ObjectTypeName.ToString();
            }

            fieldName = classType.FullName + "." + propInfo.Name;

            value = propInfo.GetValue(Class);

            if (!Enum.IsDefined(typeof(ObjectTypes), propertyType))
            {
                return userControl;
            }

            switch (Enum.Parse(typeof(ObjectTypes), propertyType))
            {
                case ObjectTypes.String:
                    var stringField = new StringField(displayValue, (string) value)
                    {
                        Width = controlWidth,
                        Height = controlHeight
                    };
                    stringField.DisplayNameTextBlock.Width = displayNameWidth;
                    stringField.ValueTextBox.Width = valueWidth;
                    if (!formField.CanEdit)
                    {
                        stringField.ValueTextBox.IsEnabled = false;
                    }

                    if (formField.Required)
                    {
                        stringField.DisplayNameTextBlock.ToolTip = "This is a Required Field";
                        OnValidate += stringField.Validate;
                    }

                    if (formField.ToolTip != string.Empty)
                    {
                        stringField.ValueTextBox.ToolTip = formField.ToolTip;
                    }

                    stringField.ValueTextBox.TextChanged += (sen, e) =>
                    {
                        propInfo.SetValue(Class, stringField.ValueTextBox.Text);

                        OnPropertyModified?.Invoke(fieldName, stringField.ValueTextBox.Text);
                    };

                    stringField.ValueTextBox.LostKeyboardFocus += (sender, args) =>
                    {
                        if (stringField.HasUpdated)
                        {
                            OnPropertyFinishedEditing?.Invoke(fieldName, stringField.ValueTextBox.Text);
                        }
                    };

                    userControl = stringField;
                    break;
                case ObjectTypes.Password:
                    var passwordField = new PasswordField(displayValue, (string) value)
                    {
                        Width = controlWidth,
                        Height = controlHeight
                    };
                    passwordField.DisplayNameTextBlock.Width = displayNameWidth;
                    passwordField.ValuePasswordBox.Width = valueWidth;
                    if (!formField.CanEdit)
                    {
                        passwordField.ValuePasswordBox.IsEnabled = false;
                    }

                    if (formField.Required)
                    {
                        passwordField.DisplayNameTextBlock.ToolTip = "This is a Required Field";
                        OnValidate += passwordField.Validate;
                    }

                    if (formField.ToolTip != string.Empty)
                    {
                        passwordField.ValuePasswordBox.ToolTip = formField.ToolTip;
                    }

                    passwordField.ValuePasswordBox.PasswordChanged += (sen, e) =>
                    {
                        propInfo.SetValue(Class, passwordField.ValuePasswordBox.Password);

                        OnPropertyModified?.Invoke(fieldName, passwordField.ValuePasswordBox.Password);
                    };

                    passwordField.ValuePasswordBox.LostKeyboardFocus += (sender, args) =>
                    {
                        if (passwordField.HasUpdated)
                        {
                            OnPropertyFinishedEditing?.Invoke(fieldName, passwordField.ValuePasswordBox.Password);
                        }
                    };

                    userControl = passwordField;
                    break;
                case ObjectTypes.Double:
                    var doubleField = new DoubleField(displayValue, (Double) value)
                    {
                        Width = controlWidth,
                        Height = controlHeight
                    };
                    doubleField.DisplayNameTextBlock.Width = displayNameWidth;
                    doubleField.ValueTextBox.Width = valueWidth;
                    if (!formField.CanEdit)
                    {
                        doubleField.ValueTextBox.IsEnabled = false;
                    }

                    if (formField.Required)
                    {
                        doubleField.DisplayNameTextBlock.ToolTip = "This is a Required Field";
                        OnValidate += doubleField.Validate;
                    }

                    if (formField.ToolTip != string.Empty)
                    {
                        doubleField.ValueTextBox.ToolTip = formField.ToolTip;
                    }

                    doubleField.ValueTextBox.TextChanged += (sen, e) =>
                    {
                        if (double.TryParse(doubleField.ValueTextBox.Text, out var doubleValue))
                        {
                            propInfo.SetValue(Class, doubleValue);
                            OnPropertyModified?.Invoke(fieldName, doubleValue);
                        }
                    };

                    doubleField.ValueTextBox.LostKeyboardFocus += (sender, args) =>
                    {
                        if (doubleField.HasUpdated && double.TryParse(doubleField.ValueTextBox.Text, out var doubleValue))
                        {
                            OnPropertyFinishedEditing?.Invoke(fieldName, doubleValue);
                        }
                    };

                    userControl = doubleField;
                    break;
                case ObjectTypes.Int32:
                    var intField = new IntField(displayValue, (int) value)
                    {
                        Width = controlWidth,
                        Height = controlHeight
                    };
                    intField.DisplayNameTextBlock.Width = displayNameWidth;
                    intField.ValueTextBox.Width = valueWidth;
                    if (!formField.CanEdit)
                    {
                        intField.ValueTextBox.IsEnabled = false;
                    }

                    if (formField.Required)
                    {
                        intField.DisplayNameTextBlock.ToolTip = "This is a Required Field";
                        OnValidate += intField.Validate;
                    }

                    if (formField.ToolTip != string.Empty)
                    {
                        intField.ValueTextBox.ToolTip = formField.ToolTip;
                    }

                    intField.ValueTextBox.TextChanged += (sen, e) =>
                    {
                        if (int.TryParse(intField.ValueTextBox.Text, out var intValue))
                        {
                            propInfo.SetValue(Class, intValue);
                            OnPropertyModified?.Invoke(fieldName, intValue);
                        }
                    };

                    intField.ValueTextBox.LostKeyboardFocus += (sender, args) =>
                    {
                        if (intField.HasUpdated && int.TryParse(intField.ValueTextBox.Text, out var intValue))
                        {
                            OnPropertyFinishedEditing?.Invoke(fieldName, intValue);
                        }
                    };

                    userControl = intField;
                    break;
                case ObjectTypes.Single:
                    var floatField = new FloatField(displayValue, (float) value)
                    {
                        Width = controlWidth,
                        Height = controlHeight
                    };
                    floatField.DisplayNameTextBlock.Width = displayNameWidth;
                    floatField.ValueTextBox.Width = valueWidth;
                    if (!formField.CanEdit)
                    {
                        floatField.ValueTextBox.IsEnabled = false;
                    }

                    if (formField.Required)
                    {
                        floatField.DisplayNameTextBlock.ToolTip = "This is a Required Field";
                        OnValidate += floatField.Validate;
                    }

                    if (formField.ToolTip != string.Empty)
                    {
                        floatField.ValueTextBox.ToolTip = formField.ToolTip;
                    }

                    floatField.ValueTextBox.TextChanged += (sen, e) =>
                    {
                        if (float.TryParse(floatField.ValueTextBox.Text, out var floatValue))
                        {
                            propInfo.SetValue(Class, floatValue);
                            OnPropertyModified?.Invoke(fieldName, floatValue);
                        }
                    };

                    floatField.ValueTextBox.LostKeyboardFocus += (sender, args) =>
                    {
                        if (floatField.HasUpdated && float.TryParse(floatField.ValueTextBox.Text, out var floatValue))
                        {
                            OnPropertyFinishedEditing?.Invoke(fieldName, floatValue);
                        }
                    };

                    userControl = floatField;
                    break;
                case ObjectTypes.Boolean:
                    var booleanField = new BooleanField(displayValue, (bool) value)
                    {
                        Width = controlWidth,
                        Height = controlHeight
                    };
                    booleanField.DisplayNameTextBlock.Width = displayNameWidth;
                    if (!formField.CanEdit)
                    {
                        booleanField.ValueCheckBox.IsEnabled = false;
                    }

                    if (formField.Required)
                    {
                        booleanField.DisplayNameTextBlock.ToolTip = "This is a Required Field";
                        OnValidate += booleanField.Validate;
                    }

                    if (formField.ToolTip != string.Empty)
                    {
                        booleanField.ValueCheckBox.ToolTip = formField.ToolTip;
                    }

                    booleanField.ValueCheckBox.Checked += (sen, e) =>
                    {
                        propInfo.SetValue(Class, booleanField.ValueCheckBox.IsChecked);
                        OnPropertyModified?.Invoke(fieldName, booleanField.ValueCheckBox.IsChecked);
                        OnPropertyFinishedEditing?.Invoke(fieldName, booleanField.ValueCheckBox.IsChecked);
                    };
                    booleanField.ValueCheckBox.Unchecked += (sen, e) =>
                    {
                        propInfo.SetValue(Class, booleanField.ValueCheckBox.IsChecked);
                        OnPropertyModified?.Invoke(fieldName, booleanField.ValueCheckBox.IsChecked);
                        OnPropertyFinishedEditing?.Invoke(fieldName, booleanField.ValueCheckBox.IsChecked);
                    };
                    userControl = booleanField;
                    break;
                /*
                case ObjectTypes.ColourPicker:
                    var colourPickerField = new ColourPickerField(displayValue, (string)value)
                    {
                        Width = controlWidth,
                        Height = controlHeight
                    };
                    colourPickerField.DisplayNameTextBlock.Width = displayNameWidth;
                    if (!formField.CanEdit)
                    {
                        colourPickerField.ValueColourPicker.IsEnabled = false;
                    }
                    if (formField.Required)
                    {
                        colourPickerField.DisplayNameTextBlock.ToolTip = "This is a Required Field";
                        OnValidate += colourPickerField.Validate;
                    }
                    if (formField.ToolTip != string.Empty)
                    {
                        colourPickerField.ValueColourPicker.ToolTip = formField.ToolTip;
                    }
                    colourPickerField.ValueColourPicker.SelectedColorChanged += (sen, e) =>
                    {
                        propInfo.SetValue(Class, colourPickerField.ValueColourPicker.SelectedColor.ToString());
                        OnPropertyModified?.Invoke(fieldName, colourPickerField.ValueColourPicker.SelectedColor.ToString());
                    };
                    userControl = colourPickerField;
                    break;
                */
                case ObjectTypes.ObjectDropdown:
                    var dropdownField = new DropdownField(displayValue, BuildDropdownItems(formField.DropDownClass),
                        (string) value)
                    {
                        Width = controlWidth,
                        Height = controlHeight
                    };
                    dropdownField.DisplayNameTextBlock.Width = displayNameWidth;
                    dropdownField.SelectComboBox.Width = valueWidth;
                    if (!formField.CanEdit)
                    {
                        dropdownField.SelectComboBox.IsEnabled = false;
                    }

                    if (formField.ToolTip != string.Empty)
                    {
                        dropdownField.SelectComboBox.ToolTip = formField.ToolTip;
                    }

                    dropdownField.SelectComboBox.SelectionChanged += (sen, e) =>
                    {
                        var selectedItem = (ComboBoxItem) dropdownField.SelectComboBox.SelectedItem;
                        propInfo.SetValue(Class, selectedItem.Tag.ToString());
                        OnPropertyModified?.Invoke(fieldName, selectedItem.Tag.ToString());
                        OnPropertyFinishedEditing?.Invoke(fieldName, selectedItem.Tag.ToString());
                    };
                    userControl = dropdownField;
                    break;
                case ObjectTypes.SpecialDropdown:
                    var specialDropdown = new SpecialDropdownField(displayValue)
                    {
                        Width = controlWidth,
                        Height = controlHeight
                    };
                    specialDropdown.DisplayNameTextBlock.Width = displayNameWidth;
                    specialDropdown.SelectComboBox.Width = valueWidth;
                    if (formField.ToolTip != string.Empty)
                    {
                        specialDropdown.SelectComboBox.ToolTip = formField.ToolTip;
                    }

                    OnAddSpecialDropdownItems += (Name, Items) =>
                    {
                        if (Name == fieldName)
                        {
                            specialDropdown.AddDropdownItems(Items, value);
                        }
                    };

                    OnSpecialDropdownDisplaying?.Invoke(fieldName);

                    specialDropdown.SelectComboBox.SelectionChanged += (sen, e) =>
                    {
                        var selectedItem = (ComboBoxItem) specialDropdown.SelectComboBox.SelectedItem;
                        if (selectedItem != null)
                        {
                            var DropdownItem = (FormDropdownItem) selectedItem.Content;
                            propInfo.SetValue(Class, DropdownItem.Value);
                            OnPropertyModified?.Invoke(fieldName, DropdownItem.Value);
                            OnPropertyFinishedEditing?.Invoke(fieldName, DropdownItem.Value);
                        }
                    };
                    userControl = specialDropdown;
                    break;
                case ObjectTypes.FolderBrowser:
                    var folderStringField = new FolderBrowserField(displayValue, (string) value)
                    {
                        Width = controlWidth,
                        Height = controlHeight
                    };
                    folderStringField.DisplayNameTextBlock.Width = displayNameWidth;
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
                    TimePickerField.DisplayNameTextBlock.Width = displayNameWidth;
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
                            Height = controlHeight,
                            DisplayNameTextBlock = {Width = displayNameWidth}
                        };

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
