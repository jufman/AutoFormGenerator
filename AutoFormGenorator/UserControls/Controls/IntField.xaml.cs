using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using AutoFormGenerator.Events;
using AutoFormGenerator.Object;

namespace AutoFormGenerator.UserControls.Controls
{
    /// <summary>
    /// Interaction logic for IntField.xaml
    /// </summary>
    public partial class IntField : Interfaces.IControlField
    {
        private string HoldValue = "";
        public bool HasUpdated { get; set; }

        public event ControlModified OnControlModified;
        public event ControlFinishedEditing OnControlFinishedEditing;

        public IntField()
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

        private void ValueTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }

        public void BuildDisplay(FormControlSettings formControlSettings)
        {
            SetVisibility(formControlSettings.IsVisible);

            Width = formControlSettings.ControlWidth;
            Height = formControlSettings.ControlHeight;

            DisplayNameTextBlock.Text = formControlSettings.DisplayValue;
            ValueTextBox.Text = formControlSettings.Value.ToString();

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
                if (int.TryParse(ValueTextBox.Text, out var intValue))
                {
                    formControlSettings.SetValue(intValue);
                    OnControlModified?.Invoke(intValue);
                }
            };

            ValueTextBox.LostKeyboardFocus += (sender, args) =>
            {
                if (HasUpdated && int.TryParse(ValueTextBox.Text, out var intValue))
                {
                    OnControlFinishedEditing?.Invoke(intValue);
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

            int dummy = 0;
            if (!int.TryParse(ValueTextBox.Text, out dummy))
            {
                Valid = false;
            }

            if (Valid)
            {
                ValueTextBox.BorderBrush = (Brush)new BrushConverter().ConvertFromString("#FFABABAB");
            }
            else
            {
                ValueTextBox.BorderBrush = Brushes.Red;
            }

            return Valid;
        }

        public object GetValue()
        {
            return ValueTextBox.Text;
        }


        public void SetVisibility(bool IsVisible)
        {
            this.Visibility = IsVisible ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
