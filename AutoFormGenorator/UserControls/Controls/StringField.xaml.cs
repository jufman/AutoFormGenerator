using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using AutoFormGenerator.Object;

namespace AutoFormGenerator.UserControls.Controls
{
    /// <summary>
    /// Interaction logic for TextField.xaml
    /// </summary>
    public partial class StringField : UserControl
    {
        private string HoldValue = "";

        public bool HasUpdated { get; set; }

        public delegate void PropertyModified(string Value);
        public event PropertyModified OnPropertyModified;

        public delegate void PropertyFinishedEditing(string Value);
        public event PropertyFinishedEditing OnPropertyFinishedEditing;

        public StringField()
        {
            InitializeComponent();

            ValueTextBox.GotKeyboardFocus += (sender, args) =>
            {
                HoldValue = ValueTextBox.Text;
                HasUpdated = false;
            };

            ValueTextBox.LostKeyboardFocus += (sender, args) =>
            {
                if (HoldValue != ValueTextBox.Text)
                {
                    HasUpdated = true;
                }
            };
        }

        public void BuildDisplay(FormControlSettings formControlSettings)
        {
            Width = formControlSettings.ControlWidth;
            Height = formControlSettings.ControlHeight;

            DisplayNameTextBlock.Text = formControlSettings.DisplayValue;
            ValueTextBox.Text = (string) formControlSettings.Value;

            if (formControlSettings.FixedWidth)
            {
                DisplayNameTextBlock.Width = formControlSettings.DisplayNameWidth;
            }
            else
            {
                Margin = new Thickness(0, 0, 30, 0);
            }

            ValueTextBox.Width = formControlSettings.ValueWidth;
            if (!formControlSettings.CanEdit)
            {
                ValueTextBox.IsEnabled = false;
            }

            if (formControlSettings.Required)
            {
                DisplayNameTextBlock.ToolTip = formControlSettings.RequiredText;
            }

            if (formControlSettings.ToolTip != string.Empty)
            {
                ValueTextBox.ToolTip = formControlSettings.ToolTip;
            }

            ValueTextBox.TextChanged += (sen, e) =>
            {
                formControlSettings.SetValue(ValueTextBox.Text);

                OnPropertyModified?.Invoke(ValueTextBox.Text);
            };

            ValueTextBox.LostKeyboardFocus += (sender, args) =>
            {
                if (HasUpdated)
                {
                    OnPropertyFinishedEditing?.Invoke(ValueTextBox.Text);
                }
            };
        }

        private void BuildContextMenu()
        {
            ValueTextBox.ContextMenu = new ContextMenu();

            ValueTextBox.ContextMenu.Items.Add(new MenuItem()
            {
                Header = "Cut",
                Command = ApplicationCommands.Cut
            });
            ValueTextBox.ContextMenu.Items.Add(new MenuItem()
            {
                Header = "Copy",
                Command = ApplicationCommands.Copy
            });
            ValueTextBox.ContextMenu.Items.Add(new MenuItem()
            {
                Header = "Paste",
                Command = ApplicationCommands.Paste
            });

            ValueTextBox.ContextMenu.Items.Add(new Separator());

            ValueTextBox.ContextMenu.Items.Add(new MenuItem()
            {
                Header = "SelectAll",
                Command = ApplicationCommands.SelectAll
            });

            ValueTextBox.ContextMenu.Items.Add(new Separator());
        }

        public void AddDropdownItems(List<Object.FieldInsert> FieldInsertItems, object value)
        {
            BuildContextMenu();

            FieldInsertItems.ForEach(s =>
            {
                var item = new MenuItem
                {
                    Header = s.Value,
                    ToolTip = s.ToolTip
                };

                item.Click += (sender, args) =>
                {
                    var cusIndex = ValueTextBox.CaretIndex;
                    var text = ValueTextBox.Text.Insert(cusIndex, s.Value);
                    ValueTextBox.Text = text;

                    ValueTextBox.CaretIndex = cusIndex + s.Value.Length;
                };

                ValueTextBox.ContextMenu?.Items.Add(item);
            });
        }

        public bool Validate()
        {
            bool Valid = true;
            if (ValueTextBox.Text.Length == 0)
            {
                Valid = false;
            }

            if (Valid)
            {
                ValueTextBox.BorderBrush = Brushes.Black;
            }
            else
            {
                ValueTextBox.BorderBrush = Brushes.Red;
            }

            return Valid;
        }

    }
}
