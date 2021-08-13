using System.Windows;
using System.Windows.Controls;
using AutoFormGenerator.Events;
using AutoFormGenerator.Interfaces;
using AutoFormGenerator.Object;

namespace AutoFormGenerator.UserControls.Controls
{
    /// <summary>
    /// Interaction logic for BooleanField.xaml
    /// </summary>
    public partial class BooleanField : UserControl, Interfaces.IControlField
    {
        public event ControlModified OnControlModified;
        public event ControlFinishedEditing OnControlFinishedEditing;

        public BooleanField()
        {
            InitializeComponent();
        }

        public void BuildDisplay(FormControlSettings formControlSettings)
        {
            SetVisibility(formControlSettings.IsVisible);

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
                OnControlModified?.Invoke(ValueCheckBox.IsChecked);
                OnControlFinishedEditing?.Invoke(ValueCheckBox.IsChecked);
            };

            ValueCheckBox.Unchecked += (sen, e) =>
            {
                formControlSettings.SetValue(ValueCheckBox.IsChecked);
                OnControlModified?.Invoke(ValueCheckBox.IsChecked);
                OnControlFinishedEditing?.Invoke(ValueCheckBox.IsChecked);
            };
        }

        public bool Validate()
        {
            return true;
        }

        public object GetValue()
        {
            return ValueCheckBox.IsChecked;
        }

        public void SetVisibility(bool IsVisible)
        {
            this.Visibility = IsVisible ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
