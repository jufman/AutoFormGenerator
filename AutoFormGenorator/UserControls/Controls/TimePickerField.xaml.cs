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
using AutoFormGenerator.Object;

namespace AutoFormGenerator.UserControls.Controls
{
    /// <summary>
    /// Interaction logic for TimePickerField.xaml
    /// </summary>
    public partial class TimePickerField : UserControl
    {
        public delegate void PropertyModified(object Value);
        public event PropertyModified OnPropertyModified;

        public delegate void PropertyFinishedEditing(object Value);
        public event PropertyFinishedEditing OnPropertyFinishedEditing;

        private double HoldValue = 0;
        public bool HasUpdated { get; set; }

        public TimePickerField()
        {
            InitializeComponent();

            TimePicker.GotKeyboardFocus += (sender, args) =>
            {
                HoldValue = TimePicker.SelectedTime == null ? TimePicker.SelectedTime.Value.TimeOfDay.TotalSeconds : 0;
                HasUpdated = false;
            };

            TimePicker.LostKeyboardFocus += (sender, args) =>
            {
                var CurrentValue = TimePicker.SelectedTime == null ? TimePicker.SelectedTime.Value.TimeOfDay.TotalSeconds : 0;
                if (HoldValue != CurrentValue)
                {
                    HasUpdated = true;
                }
            };

        }

        public void BuildDisplay(FormControlSettings formControlSettings)
        {
            Width = formControlSettings.ControlWidth;
            Height = formControlSettings.ControlHeight;

            DisplayNameTextBlock.Text = formControlSettings.DisplayValue;

            DisplayNameTextBlock.Text = formControlSettings.DisplayValue.ToString();
            TimePicker.SelectedTime = new DateTime() + TimeSpan.FromSeconds((double) formControlSettings.Value);

            if (formControlSettings.FixedWidth)
            {
                DisplayNameTextBlock.Width = formControlSettings.DisplayNameWidth;
            }
            else
            {
                Margin = new Thickness(0, 0, 30, 0);
            }

            TimePicker.Width = formControlSettings.ValueWidth;

            if (!formControlSettings.CanEdit)
            {
                TimePicker.IsEnabled = false;
            }

            if (formControlSettings.Required)
            {
                DisplayNameTextBlock.ToolTip = formControlSettings.RequiredText;
            }

            if (formControlSettings.ToolTip != string.Empty)
            {
                TimePicker.ToolTip = formControlSettings.ToolTip;
            }

            TimePicker.SelectedTimeChanged += (sender, args) =>
            {
                var CurrentValue = TimePicker.SelectedTime?.TimeOfDay.TotalSeconds ?? 0;

                formControlSettings.SetValue(CurrentValue);

                OnPropertyModified?.Invoke(CurrentValue);
                OnPropertyFinishedEditing?.Invoke(CurrentValue);
            };
        }
    }
}
