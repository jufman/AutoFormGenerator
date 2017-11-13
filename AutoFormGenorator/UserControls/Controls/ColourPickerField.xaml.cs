using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// Interaction logic for ColourPickerField.xaml
    /// </summary>
    public partial class ColourPickerField : UserControl
    {
        public ColourPickerField(string DisplayName, string Value)
        {
            InitializeComponent();

            DisplayNameTextBlock.Text = DisplayName;
            try
            {
                ValueColourPicker.SelectedColor = ((SolidColorBrush)(new BrushConverter().ConvertFrom(Value))).Color;
            }
            catch (Exception e)
            {
                e.ToString();
            }
        }

        public bool Viladate()
        {

            return true;
        }
    }
}
