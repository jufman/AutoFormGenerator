using System.Windows.Controls;

namespace AutoFormGenerator.UserControls.Controls
{
    /// <summary>
    /// Interaction logic for BooleanField.xaml
    /// </summary>
    public partial class BooleanField : UserControl
    {
        public BooleanField(string DisplayName, bool IsChecked)
        {
            InitializeComponent();

            DisplayNameTextBlock.Text = DisplayName;
            ValueCheckBox.IsChecked = IsChecked;
        }

        public bool Validate()
        {

            return true;
        }
    }
}
