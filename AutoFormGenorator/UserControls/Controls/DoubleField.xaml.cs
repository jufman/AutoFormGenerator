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
    /// Interaction logic for DoubleField.xaml
    /// </summary>
    public partial class DoubleField : UserControl
    {
        public DoubleField(string DisplayValue, double Value)
        {
            InitializeComponent();

            DisplayNameTextBlock.Text = DisplayValue.ToString();
            ValueTextBox.Text = Value.ToString();
        }

        private void ValueTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            bool NotBlock = true;

            NotBlock = Regex.IsMatch(e.Text, "[^0-9.]+");

            double num = 0;
            NotBlock = !double.TryParse(ValueTextBox.Text + e.Text, out num);

            e.Handled = NotBlock;
        }

        public bool Viladate()
        {
            bool Valid = true;
            if (ValueTextBox.Text.Length == 0)
            {
                Valid = false;
            }

            double dummy = 0;
            if (!double.TryParse(ValueTextBox.Text, out dummy))
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
