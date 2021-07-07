using System.Reflection;
using System.Windows;
using System.Windows.Controls;
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

        public StringField(string DisplayName, string Value)
        {
            InitializeComponent();

            DisplayNameTextBlock.Text = DisplayName;

            ValueTextBox.Text = Value;

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

        public void BuildDisplay(FormField formField, PropertyInfo propInfo, object Class, bool FixedWidth, double valueWidth, double displayNameWidth)
        {
            if (FixedWidth)
            {
                DisplayNameTextBlock.Width = displayNameWidth;
            }
            else
            {
                Margin = new Thickness(0, 0, 30, 0);
            }

            ValueTextBox.Width = valueWidth;
            if (!formField.CanEdit)
            {
                ValueTextBox.IsEnabled = false;
            }

            if (formField.Required)
            {
                DisplayNameTextBlock.ToolTip = "This is a Required Field";
            }

            if (formField.ToolTip != string.Empty)
            {
                ValueTextBox.ToolTip = formField.ToolTip;
            }

            ValueTextBox.TextChanged += (sen, e) =>
            {
                propInfo.SetValue(Class, ValueTextBox.Text);

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
