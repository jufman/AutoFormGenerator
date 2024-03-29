﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
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
using AutoFormGenerator.UserControls.ListControls;
using TestApp.Objects;

namespace TestApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<AutoFormGenerator.Logic> afgLogics = new List<Logic>();
        private AutoFormGenerator.Logic AFG_root = new AutoFormGenerator.Logic();

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

                var formControl = AFG.BuildFormControl(new Objects.MainClass());

                formControl.Loaded += (o, args) =>
                {
                    LoadTimeLabel.Content = $"Load Time: {StopW.ElapsedMilliseconds}ms";
                };

               

                var CarItems = new List<FormDropdownItem>();

                Cars.ForEach(s =>
                {
                    CarItems.Add(new FormDropdownItem()
                    {
                        DisplayValue = s,
                        Value = s
                    });
                });

                AFG.PopulateSpecialDropdown<Objects.MainClass>(MainClass => MainClass.TestDropDown, CarItems);

                AFG.PopulateSpecialDropdown<Objects.MainClass>(MainClass => MainClass.Act, new List<FormDropdownItem>
                {
                    new FormDropdownItem()
                    {
                        Value = "Car",
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

                AFG.PopulateSpecialDropdown<Objects.ExtendClass>(ExtendClass => ExtendClass.Act, ActItems);

                AFG.PopulateFieldInsertItems<Objects.MainClass>(MainClass => MainClass.TestString, new List<FieldInsert>
                {
                    new FieldInsert()
                    {
                        Value = "Test",
                        ToolTip = "This is a test item"
                    }
                });


                AFG.SubscribeToFieldFinishedEditing<Objects.MainClass>(MainClass => MainClass.TestString, (name, value) =>
                {
                    MessageBox.Show("Changed!!");
                });


                AFG.SubscribeToFieldModified<Objects.MainClass>(MainClass => MainClass.TestString, (name, value) =>
                {
                    MessageBox.Show("Edit");
                });

                AFG.SubscribeToFieldsFinishedEditing<Objects.MainClass>((name, value) =>
                {
                    MessageBox.Show("Multi Changed");

                }, MainClass => MainClass.TestString, MainClass => MainClass.TestInt);

                ContentStackPanel.Children.Add(formControl);

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

            Test<Objects.NestedClass>(aw => aw.TestDouble);

            Test<Objects.MainClass>(MainClass => MainClass.Act);
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

                AFG.PopulateSpecialDropdown<Objects.ExtendClass>(ExtendClass => ExtendClass.Act, ActItems);

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

        private void DisplayFieldsButton_OnClick(object sender, RoutedEventArgs e)
        {
            afgLogics.ForEach(AFG =>
            {
                AFG.SetFieldVisibility<Objects.MainClass>(Item => Item.DontShowMe, true);
            });
        }

        private void Test<T>(Expression<Func<T, object>> expression)
        {
            var fullname = "";
            switch (expression.Body.NodeType)
            {
                case ExpressionType.Convert:
                case ExpressionType.MemberAccess:
                {
                    var memberExpression = (expression.Body as MemberExpression ?? ((UnaryExpression)expression.Body).Operand as MemberExpression)?.Member;
                    var expressionRoot = (expression.Body as MemberExpression ?? ((UnaryExpression)expression.Body).Operand as MemberExpression)?.Expression;
                    if (memberExpression?.DeclaringType is null || expressionRoot == null)
                    {
                        return;
                    }

                    fullname = expressionRoot.Type.FullName + "." + memberExpression.Name;
                    break;
                }
                case ExpressionType.Constant:
                {
                    var sa = expression.Body as ConstantExpression;
                    fullname = typeof(T).FullName + "." + sa?.Value;
                    break;
                }
            }

            Console.WriteLine(fullname);
        }

        private void TestIssueButton_OnClick(object sender, RoutedEventArgs e)
        {
            ContentStackPanel.Children.Clear();
            AFG_root.Debug = true;
            var formControl = AFG_root.BuildFormControl(new Objects.MainClass());

            ContentStackPanel.Children.Add(formControl);
        }
    }
}
