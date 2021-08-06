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

        private List<string> RandomList1 = new List<string>()
        {
            "approval",
            "honor",
            "confusion",
            "velvet",
            "wall",
            "issue",
            "problem",
            "quotation",
            "suggest",
            "ribbon"
        };

        private List<string> RandomList2 = new List<string>()
        {
            "chaunting",
            "autosymbolically",
            "teaspoonfuls",
            "spermatogemma",
            "charqui",
            "ophiuchus",
            "volutes",
            "intervisitation",
            "longingly",
            "unadjoined",
        };

        private List<string> Cars = new List<string>()
        {
            "Abarth",
            "Alfa Romeo",
            "Aston Martin",
            "Audi",
            "Bentley",
            "BMW",
            "Bugatti",
            "Cadillac",
            "Chevrolet",
            "Chrysler",
            "Citroën",
            "Dacia",
            "Daewoo",
            "Daihatsu",
            "Dodge",
            "Donkervoort",
            "DS",
            "Ferrari",
            "Fiat",
            "Fisker",
            "Ford",
            "Honda",
            "Hummer",
            "Hyundai",
            "Infiniti",
            "Iveco",
            "Jaguar",
            "Jeep",
            "Kia",
            "KTM",
            "Lada",
            "Lamborghini",
            "Lancia",
            "Land Rover",
            "Landwind",
            "Lexus",
            "Lotus",
            "Maserati",
            "Maybach",
            "Mazda",
            "McLaren",
            "Mercedes-Benz",
            "MG",
            "Mini",
            "Mitsubishi",
            "Morgan",
            "Nissan",
            "Opel",
            "Peugeot",
            "Porsche",
            "Renault",
            "Rolls-Royce",
            "Rover",
            "Saab",
            "Seat",
            "Skoda",
            "Smart",
            "SsangYong",
            "Subaru",
            "Suzuki",
            "Tesla",
            "Toyota",
            "Volkswagen",
            "Volvo"
        };


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

                var CarItems = new List<FormDropdownItem>();

                Cars.ForEach(s =>
                {
                    CarItems.Add(new FormDropdownItem()
                    {
                        DisplayValue = s,
                        Value = s
                    });
                });

                AFG.PopulateSpecialDropdown<Objects.MainClass>("TestDropDown", CarItems);

                AFG.PopulateSpecialDropdown<Objects.MainClass>("Act", new List<FormDropdownItem>
                {
                    new FormDropdownItem()
                    {
                        Value = 1.0,
                        DisplayValue = "Car"
                    }
                });

                var ActItems = new List<FormDropdownItem>();
                RandomList1.ForEach(s =>
                {
                    ActItems.Add(new FormDropdownItem()
                    {
                        DisplayValue = s,
                        Value = s
                    });
                });

                AFG.PopulateSpecialDropdown<Objects.ExtendClass>("Act", ActItems);

                AFG.PopulateFieldInsertItems<Objects.MainClass>("TestString", new List<FieldInsert>
                {
                    new FieldInsert()
                    {
                        Value = "Test",
                        ToolTip = "This is a test item"
                    }
                });


                AFG.SubscribeToFieldFinishedEditing<Objects.MainClass>("TestString", (name, value) =>
                {
                    MessageBox.Show("Changed!!");
                });


                AFG.SubscribeToFieldModified<Objects.MainClass>("TestString", (name, value) =>
                {
                    MessageBox.Show("Edit");
                });

                AFG.SubscribeToFieldsFinishedEditing<Objects.MainClass>((name, value) =>
                {
                    MessageBox.Show("Multi Changed");

                }, "TestString", "TestInt");

                ContentStackPanel.Children.Add(AFG.formControl);

                afgLogics.Add(AFG);
            }

            StopW.Stop();
            LoadTimeLabel.Content = $"Load Time: {StopW.ElapsedMilliseconds}ms";
        }


        private void ValidateButton_OnClick(object sender, RoutedEventArgs e)
        {
            afgLogics.ForEach(logic =>
            {
                logic.Compile();
            });
        }

        private void ExtendButton_OnClick(object sender, RoutedEventArgs e)
        {
            afgLogics.ForEach(AFG =>
            {
                var ActItems = new List<FormDropdownItem>();
                RandomList2.ForEach(s =>
                {
                    ActItems.Add(new FormDropdownItem()
                    {
                        DisplayValue = s,
                        Value = s
                    });
                });

                AFG.PopulateSpecialDropdown<Objects.ExtendClass>("Act", ActItems);

            });
        }


        private void MessageBoxButton_OnClick(object sender, RoutedEventArgs e)
        {
            var md = new AutoFormGenerator.Windows.AFG_MessageBox("Tester", "This is a message");
            md.ShowDialog();
        }

        private void DisplayBoxButton_OnClick(object sender, RoutedEventArgs e)
        {
            AutoFormGenerator.Windows.AFG_MessageDisplayBoxWindow.Show("This is a test");
        }
    }
}
