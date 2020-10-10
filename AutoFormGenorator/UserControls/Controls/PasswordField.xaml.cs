using System.Windows.Controls;
using System.Windows.Media;

namespace AutoFormGenerator.UserControls.Controls
{
    /// <summary>
    /// Interaction logic for PasswordField.xaml
    /// </summary>
    public partial class PasswordField : UserControl
    {
        public PasswordField(string DisplayName, string Value)
        {
            InitializeComponent();

            DisplayNameTextBlock.Text = DisplayName;

            ValuePasswordBox.Password = Value;
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
