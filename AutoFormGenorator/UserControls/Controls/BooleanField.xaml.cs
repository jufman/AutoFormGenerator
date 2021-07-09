using System.Windows;
using System.Windows.Controls;
using AutoFormGenerator.Object;

namespace AutoFormGenerator.UserControls.Controls
{
    /// <summary>
    /// Interaction logic for BooleanField.xaml
    /// </summary>
    public partial class BooleanField : UserControl
    {
        public delegate void PropertyModified(bool? Value);
        public event PropertyModified OnPropertyModified;

        public delegate void PropertyFinishedEditing(bool? Value);
        public event PropertyFinishedEditing OnPropertyFinishedEditing;

        public BooleanField()
        {
            InitializeComponent();
        }

        public void BuildDisplay(FormControlSettings formControlSettings)
        {
            Width = formControlSettings.ControlWidth;
            Height = formControlSettings.ControlHeight;

            DisplayNameTextBlock.Text = formControlSettings.DisplayValue;
            ValueCheckBox.IsChecked = (bool) formControlSettings.Value;

            if (formControlSettings.FixedWidth)
            {
                DisplayNameTextBlock.Width = formControlSettings.DisplayNameWidth;
            }
            else
            {
                Margin = new Thickness(0, 0, 30, 0);
            }

            //DisplayNameTextBlock.Width = formControlSettings.ValueWidth;
            if (!formControlSettings.CanEdit)
            {
                ValueCheckBox.IsEnabled = false;
            }

            if (formControlSettings.Required)
            {
                DisplayNameTextBlock.ToolTip = formControlSettings.RequiredText;
            }

            if (formControlSettings.ToolTip != string.Empty)
            {
                ValueCheckBox.ToolTip = formControlSettings.ToolTip;
            }

            ValueCheckBox.Checked += (sen, e) =>
            {
                formControlSettings.SetValue(ValueCheckBox.IsChecked);
                OnPropertyModified?.Invoke(ValueCheckBox.IsChecked);
                OnPropertyFinishedEditing?.Invoke(ValueCheckBox.IsChecked);
            };

            ValueCheckBox.Unchecked += (sen, e) =>
            {
                formControlSettings.SetValue(ValueCheckBox.IsChecked);
                OnPropertyModified?.Invoke(ValueCheckBox.IsChecked);
                OnPropertyFinishedEditing?.Invoke(ValueCheckBox.IsChecked);
            };
        }

        public bool Validate()
        {

            return true;
        }
    }
}
