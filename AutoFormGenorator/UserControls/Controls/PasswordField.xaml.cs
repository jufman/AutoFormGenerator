﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using AutoFormGenerator.Object;
using MaterialDesignThemes.Wpf;

namespace AutoFormGenerator.UserControls.Controls
{
    /// <summary>
    /// Interaction logic for PasswordField.xaml
    /// </summary>
    public partial class PasswordField : UserControl
    {
        private string HoldValue = "";

        private bool ShowPassword = false;

        public bool HasUpdated { get; set; }

        public delegate void PropertyModified(string Value);
        public event PropertyModified OnPropertyModified;

        public delegate void PropertyFinishedEditing(string Value);
        public event PropertyFinishedEditing OnPropertyFinishedEditing;

        public PasswordField()
        {
            InitializeComponent();

            ValuePasswordBox.GotKeyboardFocus += (sender, args) =>
            {
                HoldValue = ValuePasswordBox.Password;
                HasUpdated = false;
            };

            ValuePasswordBox.LostKeyboardFocus += (sender, args) =>
            {
                if (HoldValue != ValuePasswordBox.Password)
                {
                    HasUpdated = true;
                }
            };
        }

        public void BuildDisplay(FormControlSettings formControlSettings)
        {
            Width = formControlSettings.ControlWidth;
            Height = formControlSettings.ControlHeight;

            DisplayNameTextBlock.Text = formControlSettings.DisplayValue;
            ValuePasswordBox.Password = (string) formControlSettings.Value;
            ValueTextBox.Text = (string)formControlSettings.Value;

            if (formControlSettings.FixedWidth)
            {
                DisplayNameTextBlock.Width = formControlSettings.DisplayNameWidth;
            }
            else
            {
                Margin = new Thickness(0, 0, 30, 0);
            }

            ValuePasswordBox.Width = formControlSettings.ValueWidth;
            ValueTextBox.Width = formControlSettings.ValueWidth;

            if (!formControlSettings.CanEdit)
            {
                ValuePasswordBox.IsEnabled = false;
            }

            if (formControlSettings.Required)
            {
                DisplayNameTextBlock.ToolTip = formControlSettings.RequiredText;
            }

            if (formControlSettings.ToolTip != string.Empty)
            {
                ValuePasswordBox.ToolTip = formControlSettings.ToolTip;
                ValueTextBox.ToolTip = formControlSettings.ToolTip;
            }

            ValuePasswordBox.PasswordChanged += (sen, e) =>
            {
                formControlSettings.SetValue(ValuePasswordBox.Password);

                OnPropertyModified?.Invoke(ValuePasswordBox.Password);

                ValueTextBox.Text = ValuePasswordBox.Password;
            };

            ValuePasswordBox.LostKeyboardFocus += (sender, args) =>
            {
                if (HasUpdated)
                {
                    OnPropertyFinishedEditing?.Invoke(ValuePasswordBox.Password);
                }
            };
        }

        public bool Validate()
        {
            bool Valid = true;
            if (ValuePasswordBox.Password.Length == 0)
            {
                Valid = false;
            }

            if (Valid)
            {
                ValuePasswordBox.BorderBrush = Brushes.Black;
            }
            else
            {
                ValuePasswordBox.BorderBrush = Brushes.Red;
            }

            return Valid;
        }

        private void UIElement_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (ShowPassword)
            {
                ValueTextBox.Visibility = Visibility.Collapsed;
                ValuePasswordBox.Visibility = Visibility.Visible;

                ShowPasswordIcon.Kind = PackIconKind.Eye;
                ShowPassword = false;
            }
            else
            {
                ValueTextBox.Visibility = Visibility.Visible;
                ValuePasswordBox.Visibility = Visibility.Collapsed;

                ShowPasswordIcon.Kind = PackIconKind.EyeOff;
                ShowPassword = true;
            }
        }
    }
}
