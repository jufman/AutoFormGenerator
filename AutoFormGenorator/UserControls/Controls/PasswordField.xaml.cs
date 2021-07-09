using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using AutoFormGenerator.Object;

namespace AutoFormGenerator.UserControls.Controls
{
    /// <summary>
    /// Interaction logic for PasswordField.xaml
    /// </summary>
    public partial class PasswordField : UserControl
    {
        private string HoldValue = "";

        public bool HasUpdated { get; set; }

        public delegate void PropertyModified(string Value);
        public event PropertyModified OnPropertyModified;

        public delegate void PropertyFinishedEditing(string Value);
        public event PropertyFinishedEditing OnPropertyFinishedEditing;

        public PasswordField()
        {
            InitializeComponent();

            ValuePasswordBox.GotKeyboardFocus += (sender, args) =>
            {
                HoldValue = ValuePasswordBox.Password;
                HasUpdated = false;
            };

            ValuePasswordBox.LostKeyboardFocus += (sender, args) =>
            {
                if (HoldValue != ValuePasswordBox.Password)
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
            ValuePasswordBox.Password = formControlSettings.Value.ToString();

            if (formControlSettings.FixedWidth)
            {
                DisplayNameTextBlock.Width = formControlSettings.DisplayNameWidth;
            }
            else
            {
                Margin = new Thickness(0, 0, 30, 0);
            }

            ValuePasswordBox.Width = formControlSettings.ValueWidth;
            if (!formControlSettings.CanEdit)
            {
                ValuePasswordBox.IsEnabled = false;
            }

            if (formControlSettings.Required)
            {
                DisplayNameTextBlock.ToolTip = formControlSettings.RequiredText;
            }

            if (formControlSettings.ToolTip != string.Empty)
            {
                ValuePasswordBox.ToolTip = formControlSettings.ToolTip;
            }

            ValuePasswordBox.PasswordChanged += (sen, e) =>
            {
                formControlSettings.SetValue(ValuePasswordBox.Password);

                OnPropertyModified?.Invoke(ValuePasswordBox.Password);
            };

            ValuePasswordBox.LostKeyboardFocus += (sender, args) =>
            {
                if (HasUpdated)
                {
                    OnPropertyFinishedEditing?.Invoke(ValuePasswordBox.Password);
                }
            };
        }

        public bool Validate()
        {
            bool Valid = true;
            if (ValuePasswordBox.Password.Length == 0)
            {
                Valid = false;
            }

            if (Valid)
            {
                ValuePasswordBox.BorderBrush = Brushes.Black;
            }
            else
            {
                ValuePasswordBox.BorderBrush = Brushes.Red;
            }

            return Valid;
        }
    }
}
