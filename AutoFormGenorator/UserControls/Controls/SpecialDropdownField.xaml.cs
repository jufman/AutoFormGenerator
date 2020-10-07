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

            DropdownItems.ForEach(Item =>
            {
                var BoxItem = new ComboBoxItem()
                {
                    Content = Item
                };


                if (value != null && Item.Value.ToString() == value.ToString())
                {
                    BoxItem.IsSelected = true;
                }

                SelectComboBox.Items.Add(BoxItem);
            });
        }

    }
}
