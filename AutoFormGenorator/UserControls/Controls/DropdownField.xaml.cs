using System.Collections.Generic;
using System.Windows.Controls;

namespace AutoFormGenerator.UserControls.Controls
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
