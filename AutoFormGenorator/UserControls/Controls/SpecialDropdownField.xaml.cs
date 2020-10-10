using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AutoFormGenerator.UserControls.Controls
{
    /// <summary>
    /// Interaction logic for DropdownField.xaml
    /// </summary>
    public partial class SpecialDropdownField : UserControl
    {
        public SpecialDropdownField(string DisplayValue)
        {
            InitializeComponent();

            DisplayNameTextBlock.Text = DisplayValue.ToString();
        }

        public void AddDropdownItems(List<Object.FormDropdownItem> DropdownItems, object value)
        {
            SelectComboBox.Items.Clear();
            var baseColour = (Brush) new BrushConverter().ConvertFromString("#303030");

            DropdownItems.ForEach(Item =>
            {
                var BoxItem = new ComboBoxItem()
                {
                    Content = Item,
                    Background = baseColour,
                    BorderBrush = baseColour
                };

                BoxItem.Resources.Add(SystemColors.HighlightBrushKey, baseColour);
                BoxItem.Resources.Add(SystemColors.WindowBrushKey, baseColour);

                if (value != null && Item.Value.ToString() == value.ToString())
                {
                    BoxItem.IsSelected = true;
                }

                SelectComboBox.Items.Add(BoxItem);
            });
        }

    }
}
