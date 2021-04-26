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

namespace Testing
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void GoButton_Click(object sender, RoutedEventArgs e)
        {
            BuildDisplay();
        }

        private void BuildDisplay()
        {
            Stopwatch sw = Stopwatch.StartNew();

            for (int i = 0; i < 30; i++)
            {
                var AFG = new AutoFormGenerator.Logic();

                var Root = new Objects.Root();

                AFG.BuildFormControl(Root);

                DisplayWindowStackPanel.Children.Add(AFG.formControl);

                Stopwatch swr = Stopwatch.StartNew();

                AFG.formControl.Loaded += (sender, args) =>
                {
                    swr.Stop();
                    Console.WriteLine("Control loaded: {0}ms", swr.ElapsedMilliseconds);
                };
            }

            sw.Stop();
            Console.WriteLine("Total Process Time: {0}ms", sw.ElapsedMilliseconds);
        }
    }
}
