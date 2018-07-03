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

namespace TestApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private AutoFormGenorator.Logic AFG;

        public MainWindow()
        {
            InitializeComponent();
            AFG = new AutoFormGenorator.Logic();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ImARootClass Root = new ImARootClass();
            MainSP.Children.Add(AFG.BuildFormControl(Root));
            AFG.SubsceribeToPropertyModified<Nuested>("Exaple", () =>
           {
               MessageBox.Show("Hi");
           });
        }
        
    }

    public class ImARootClass
    {
        [AutoFormGenorator.Object.FormField(Type = AutoFormGenorator.Object.Types.NestedSettings)]
        public Nuested NestedClass { get; set; }
    }

    public class Nuested
    {
        [AutoFormGenorator.Object.FormField]
        public string Exaple { get; set; }
    }
}
