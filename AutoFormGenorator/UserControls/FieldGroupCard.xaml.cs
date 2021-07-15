using System.Windows.Controls;

namespace AutoFormGenerator.UserControls
{
    /// <summary>
    /// Interaction logic for FieldGroupCard.xaml
    /// </summary>
    public partial class FieldGroupCard : UserControl
    {
        public FieldGroupCard()
        {
            InitializeComponent();
        }

        public void AddControl(UserControl control)
        {
            ControlsWrapPanel.Children.Add(control);
        }

        public void SetText(string text, bool append = false)
        {
            if (append)
            {
                DisplayNameTextBlock.Text += text;
            }
            else
            {
                DisplayNameTextBlock.Text = text;
            }
        }
    }
}
