using System.Windows.Controls;
using System.Windows.Media;

namespace AutoFormGenerator.UserControls.Controls
{
    /// <summary>
    /// Interaction logic for PasswordField.xaml
    /// </summary>
    public partial class PasswordField : UserControl
    {
        private string HoldValue = "";

        public bool HasUpdated { get; set; }

        public PasswordField(string DisplayName, string Value)
        {
            InitializeComponent();

            DisplayNameTextBlock.Text = DisplayName;

            ValuePasswordBox.Password = Value;


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
