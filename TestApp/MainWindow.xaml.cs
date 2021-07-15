using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using AutoFormGenerator;
using AutoFormGenerator.Object;

namespace TestApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<AutoFormGenerator.Logic> afgLogics = new List<Logic>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ClassesSlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (ClassesNumberLabel == null)
            {
                return;
            }
            ClassesNumberLabel.Content = ClassesSlider.Value.ToString();
        }

        private void StartTestButton_Click(object sender, RoutedEventArgs e)
        {
            ContentStackPanel.Children.Clear();
            afgLogics.Clear();

            var numberOfClasses = ClassesSlider.Value;

            var StopW = Stopwatch.StartNew();


            for (int i = 0; i < numberOfClasses; i++)
            {
                var AFG = new AutoFormGenerator.Logic();
                AFG.Debug = true;

                AFG.formControl.Loaded += (o, args) =>
                {
                    LoadTimeLabel.Content = $"Load Time: {StopW.ElapsedMilliseconds}ms";
                };

                AFG.BuildFormControl(new Objects.MainClass());

                AFG.PopulateSpecialDropdown<Objects.MainClass>("TestDropDown", new List<FormDropdownItem>
                {
                    new FormDropdownItem()
                    {
                        Value = "Car",
                        DisplayValue = "Car"
                    },
                    new FormDropdownItem()
                    {
                        Value = "Bus",
                        DisplayValue = "Bus"
                    },
                    new FormDropdownItem()
                    {
                        Value = "Bike",
                        DisplayValue = "Bike"
                    },
                    new FormDropdownItem()
                    {
                        Value = "Plane",
                        DisplayValue = "Plane"
                    }
                });

                AFG.PopulateSpecialDropdown<Objects.MainClass>("Act", new List<FormDropdownItem>
                {
                    new FormDropdownItem()
                    {
                        Value = 1.0,
                        DisplayValue = "Car"
                    }
                });

                AFG.PopulateFieldInsertItems<Objects.MainClass>("TestString", new List<FieldInsert>
                {
                    new FieldInsert()
                    {
                        Value = "Test",
                        ToolTip = "This is a test item"
                    }
                });


                AFG.SubscribeToOnPropertyFinishedEditing<Objects.MainClass>("TestString", (name, value) =>
                {
                    MessageBox.Show("Changed!!");
                });

                ContentStackPanel.Children.Add(AFG.formControl);

                afgLogics.Add(AFG);
            }

            StopW.Stop();
            LoadTimeLabel.Content = $"Load Time: {StopW.ElapsedMilliseconds}ms";
        }


    }
}
