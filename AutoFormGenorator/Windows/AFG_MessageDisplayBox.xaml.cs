using System.Windows;

namespace AutoFormGenerator.Windows
{
    /// <summary>
    /// Interaction logic for AFG_MessageDisplayBox.xaml
    /// </summary>
    public partial class AFG_MessageDisplayBoxWindow : Window
    {
        public AFG_MessageDisplayBoxWindow(string Title, string Message)
        {
            InitializeComponent();

            this.Title = Title;
            MessageTextBlock.Text = Message;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

    public static class AFG_MessageDisplayBox
    {
        public static void Show(string Message)
        {
            new AFG_MessageDisplayBoxWindow("Message", Message).ShowDialog();
        }
    }
}
