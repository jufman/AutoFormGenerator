using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace AutoFormGenerator.UserControls.Controls
{
    /// <summary>
    /// Interaction logic for IntField.xaml
    /// </summary>
    public partial class IntField : UserControl
    {
        private string HoldValue = "";
        public bool HasUpdated { get; set; }

        public IntField(string DisplayValue, int Value)
        {
            InitializeComponent();

            DisplayNameTextBlock.Text = DisplayValue.ToString();
            ValueTextBox.Text = Value.ToString();

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
