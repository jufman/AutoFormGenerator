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
using AutoFormGenerator.Events;
using AutoFormGenerator.Object;

namespace AutoFormGenerator.UserControls.Controls
{
    /// <summary>
    /// Interaction logic for CustomControlBase.xaml
    /// </summary>
    public partial class CustomControlBase : UserControl, Interfaces.IControlField
    {
        public event ControlModified OnControlModified;
        public event ControlFinishedEditing OnControlFinishedEditing;

        private Interfaces.ICustomControl CustomControlClass;

        public CustomControlBase()
        {
            InitializeComponent();
        }

        public void BuildDisplay(FormControlSettings formControlSettings, Interfaces.ICustomControl customControlClass)
        {
            SetVisibility(formControlSettings.IsVisible);

            CustomControlClass = customControlClass;

            var customControl = (UserControl) customControlClass;

            Width = formControlSettings.ControlWidth;
            Height = formControlSettings.ControlHeight;

            customControl.Width = formControlSettings.ValueWidth;
            customControlClass.SetValue(formControlSettings.Value);


            DisplayNameTextBlock.Text = formControlSettings.DisplayValue;

            if (formControlSettings.FixedWidth)
            {
                DisplayNameTextBlock.Width = formControlSettings.DisplayNameWidth;
            }
            else
            {
                Margin = new Thickness(0, 0, 30, 0);
            }

            if (formControlSettings.Required)
            {
                DisplayNameTextBlock.ToolTip = formControlSettings.RequiredText;
            }

            CustomControlCanvas.Children.Add(customControl);

            customControlClass.OnPropertyFinishedEditing += (name, o) =>
            {
                OnControlFinishedEditing?.Invoke(o);
            };

            customControlClass.OnPropertyModified += (name, o) =>
            {
                formControlSettings.SetValue(o);
                OnControlModified?.Invoke(o);
            };
        }

        public object GetValue()
        {
            return CustomControlClass.GetValue();
        }

        public void BuildDisplay(FormControlSettings formControlSettings)
        {
            
        }

        public bool Validate()
        {
            return CustomControlClass.Validate();
        }
        public void SetVisibility(bool IsVisible)
        {
            this.Visibility = IsVisible ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
