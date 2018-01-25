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
    public partial class DropdownField : UserControl
    {
        public DropdownField(string DisplayValue, List<DropdownItem> DropdownItems, string SelectedValue)
        {
            InitializeComponent();

            DisplayNameTextBlock.Text = DisplayValue.ToString();

            foreach (DropdownItem DropdownItem in DropdownItems)
            {
                ComboBoxItem Cbi = new ComboBoxItem();

                Cbi.Content = DropdownItem.Name;
                Cbi.Tag = DropdownItem.Value;

                if (SelectedValue != null && DropdownItem.Value.ToLower() == SelectedValue.ToLower())
                {
                    Cbi.IsSelected = true;
                }

                SelectComboBox.Items.Add(Cbi);
            }
        }

        public class DropdownItem
        {
            public string Name { get; set; }

            public string Value { get; set; }
        }
    }
}
