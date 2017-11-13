using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AutoFormGenorator.UserControls.Controls
{
    /// <summary>
    /// Interaction logic for IntField.xaml
    /// </summary>
    public partial class IntField : UserControl
    {
        public IntField(string DisplayValue, int Value)
        {
            InitializeComponent();

            DisplayNameTextBlock.Text = DisplayValue.ToString();
            ValueTextBox.Text = Value.ToString();
        }

        private void ValueTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }

        public bool Viladate()
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
