using System.Windows;
using System.Windows.Input;

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

            TitleLabel.Text = Title;
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

        private void UIElement_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void CloseIconGrid_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
    }
}
