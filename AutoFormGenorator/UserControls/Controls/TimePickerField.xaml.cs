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

namespace AutoFormGenerator.UserControls.Controls
{
    /// <summary>
    /// Interaction logic for TimePickerField.xaml
    /// </summary>
    public partial class TimePickerField : UserControl
    {
        private double HoldValue = 0;
        public bool HasUpdated { get; set; }

        public TimePickerField(string DisplayValue, double Value)
        {
            InitializeComponent();

            DisplayNameTextBlock.Text = DisplayValue.ToString();
            TimePicker.SelectedTime = new DateTime() + TimeSpan.FromSeconds(Value);

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
    }
}
