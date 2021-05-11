using System;
using System.Collections.Generic;
using System.Globalization;
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
using AutoFormGenerator.Events;

namespace TestApp.Controls
{
    /// <summary>
    /// Interaction logic for CustomControl.xaml
    /// </summary>
    public partial class CustomControl : AutoFormGenerator.Interfaces.ICustomControl
    {
        public CustomControl()
        {
            InitializeComponent();
        }

        public event PropertyModified OnPropertyModified;
        public event PropertyFinishedEditing OnPropertyFinishedEditing;
        public void SetValue(object Value)
        {
            var floatValue = Value as float? ?? 0;

            if (floatValue >= 0.0 && 1.0 >= floatValue)
            {
                Slider.Value = floatValue * 100;
            }
        }

        private void Slider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider.ToolTip = e.NewValue.ToString(CultureInfo.InvariantCulture) + "%";
            if (DisplayLabel != null)
            {
                DisplayLabel.Content = e.NewValue.ToString(CultureInfo.InvariantCulture) + "%";
            }

            OnPropertyModified?.Invoke("",Convert.ToSingle(e.NewValue / 100));
        }

        private void Slider_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            OnPropertyFinishedEditing?.Invoke("", Convert.ToSingle(Slider.Value / 100));
        }
    }
}
