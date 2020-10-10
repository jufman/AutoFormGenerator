using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace AutoFormGenerator.UserControls.ListControls
{
    /// <summary>
    /// Interaction logic for ListGroupCard.xaml
    /// </summary>
    public partial class GroupCard : UserControl
    {
        public GroupCard()
        {
            InitializeComponent();
        }

        private void AddItemIcon_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Type Item = (Type)Tag;


        }
    }
}
