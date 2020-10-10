using System.IO;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using Cursors = System.Windows.Input.Cursors;

namespace AutoFormGenerator.UserControls.Controls
{
    /// <summary>
    /// Interaction logic for FolderBrowserField.xaml
    /// </summary>
    public partial class FolderBrowserField : System.Windows.Controls.UserControl
    {
        public FolderBrowserField(string DisplayName, string Value)
        {
            InitializeComponent();

            DisplayNameTextBlock.Text = DisplayName;

            ValueTextBox.Text = Value;

            ValueTextBox.ToolTip = "Double Click for Folder Browser";
        }

        public bool Validate()
        {
            bool Valid = true;
            if (ValueTextBox.Text.Length == 0)
            {
                Valid = false;
            }

            if (Directory.Exists(ValueTextBox.Text) == false)
            {
                Valid = false;
            }

            if (Valid)
            {
                ValueTextBox.BorderBrush = Brushes.Black;
            }
            else
            {
                ValueTextBox.BorderBrush = Brushes.Red;
            }

            return Valid;
        }

        private void ValueTextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    ValueTextBox.Text = fbd.SelectedPath;
                }
            }
        }

        private void ValueTextBox_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ValueTextBox.Cursor = Cursors.Hand;
        }

        private void ValueTextBox_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ValueTextBox.Cursor = Cursors.Arrow;
        }
    }
}
