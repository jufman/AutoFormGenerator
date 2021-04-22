using System.Windows.Controls;
using FontAwesome.WPF;

namespace AutoFormGenerator.UserControls
{
    /// <summary>
    /// Interaction logic for FormControl.xaml
    /// </summary>
    public partial class FormControl : UserControl
    {
        public FormControl()
        {
            InitializeComponent();
        }

        public T Compile<T>()
        {
            return (T)Tag;
        }
    }
}
