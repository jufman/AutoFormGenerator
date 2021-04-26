using System.Windows.Controls;
using System.Windows.Media;

namespace AutoFormGenerator.UserControls.Controls
{
    /// <summary>
    /// Interaction logic for TextField.xaml
    /// </summary>
    public partial class StringField : UserControl
    {
        private string HoldValue = "";

        public bool HasUpdated { get; set; }

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
