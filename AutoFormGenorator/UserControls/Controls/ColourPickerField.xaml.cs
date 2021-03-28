using System;
using System.Windows.Controls;
using System.Windows.Media;

namespace AutoFormGenerator.UserControls.Controls
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
                //ValueColourPicker.SelectedColor = ((SolidColorBrush)(new BrushConverter().ConvertFrom(Value))).Color;
            }
            catch (Exception e)
            {
                e.ToString();
            }
        }

        public bool Validate()
        {

            return true;
        }
    }
}
