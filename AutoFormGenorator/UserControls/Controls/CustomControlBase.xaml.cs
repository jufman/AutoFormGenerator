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
    /// Interaction logic for CustomControlBase.xaml
    /// </summary>
    public partial class CustomControlBase : UserControl
    {
        public delegate void PropertyModified(object Value);
        public event PropertyModified OnPropertyModified;

        public delegate void PropertyFinishedEditing(object Value);
        public event PropertyFinishedEditing OnPropertyFinishedEditing;

        public CustomControlBase()
        {
            InitializeComponent();
        }

        public void BuildDisplay(FormControlSettings formControlSettings, Interfaces.ICustomControl customControlClass)
        {
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
                OnPropertyFinishedEditing?.Invoke(o);
            };

            customControlClass.OnPropertyModified += (name, o) =>
            {
                formControlSettings.SetValue(o);
                OnPropertyModified?.Invoke(o);
            };
        }

    }
}
