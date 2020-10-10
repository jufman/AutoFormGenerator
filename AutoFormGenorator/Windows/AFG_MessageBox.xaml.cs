using System.Windows;

namespace AutoFormGenerator.Windows
{
    /// <summary>
    /// Interaction logic for AFG_MessageBox.xaml
    /// </summary>
    public partial class AFG_MessageBox : Window
    {
        public MessageBoxResult MessageBoxResult = MessageBoxResult.No;

        public AFG_MessageBox(string Title, string Message)
        {
            InitializeComponent();

            this.Title = Title;
            MessageTextBlock.Text = Message;
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult = MessageBoxResult.Yes;
            this.Close();
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult = MessageBoxResult.No;
            this.Close();
        }
    }
}
