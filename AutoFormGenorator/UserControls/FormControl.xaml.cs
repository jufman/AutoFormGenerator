using System.Windows.Controls;

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
