﻿using AutoFormGenerator.Object;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using AutoFormGenerator.Events;
using Cursors = System.Windows.Input.Cursors;

namespace AutoFormGenerator.UserControls.Controls
{
    /// <summary>
    /// Interaction logic for FolderBrowserField.xaml
    /// </summary>
    public partial class FolderBrowserField : Interfaces.IControlField
    {
        public event ControlModified OnControlModified;
        public event ControlFinishedEditing OnControlFinishedEditing;

        public FolderBrowserField()
        {
            InitializeComponent();

            ValueTextBox.ToolTip = "Double Click for Folder Browser";
        }

        public void BuildDisplay(FormControlSettings formControlSettings)
        {
            SetVisibility(formControlSettings.IsVisible);

            Width = formControlSettings.ControlWidth;
            Height = formControlSettings.ControlHeight;

            DisplayNameTextBlock.Text = formControlSettings.DisplayValue;

            if (formControlSettings.FixedWidth)
            {
                DisplayNameTextBlock.Width = formControlSettings.DisplayNameWidth;
            }
            else
            {
                Margin = new Thickness(0, 0, 30, 0);
            }

            ValueTextBox.Width = formControlSettings.ValueWidth;

            if (!formControlSettings.CanEdit)
            {
                ValueTextBox.IsEnabled = false;
            }

            if (formControlSettings.Required)
            {
                DisplayNameTextBlock.ToolTip = formControlSettings.RequiredText;
            }

            if (formControlSettings.ToolTip != string.Empty)
            {
                ValueTextBox.ToolTip = formControlSettings.ToolTip;
            }

            ValueTextBox.TextChanged += (sen, e) =>
            {
                formControlSettings.SetValue(ValueTextBox.Text);

                OnControlModified?.Invoke(ValueTextBox.Text);
                OnControlFinishedEditing?.Invoke(ValueTextBox.Text);
            };
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
                ValueTextBox.BorderBrush = (Brush)new BrushConverter().ConvertFromString("#FFABABAB");
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

        public object GetValue()
        {
            return ValueTextBox.Text;
        }

        public void SetVisibility(bool IsVisible)
        {
            this.Visibility = IsVisible ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
