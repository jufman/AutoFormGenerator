using System.Windows;
using System.Windows.Input;

namespace AutoFormGenerator.Windows
{
    /// <summary>
    /// Interaction logic for AFG_MessageDisplayBox.xaml
    /// </summary>
    partial class AFG_MessageDisplayBoxWindow : Window
    {
        public AFG_MessageDisplayBoxWindow(string Title, string Message)
        {
            InitializeComponent();

            TitleLabel.Text = Title;
            MessageTextBlock.Text = Message;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public static void Show(string Message)
        {
            new AFG_MessageDisplayBoxWindow("Message", Message).ShowDialog();
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
