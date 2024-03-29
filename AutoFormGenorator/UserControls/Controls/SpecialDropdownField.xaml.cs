﻿using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using AutoFormGenerator.Events;
using AutoFormGenerator.Object;

namespace AutoFormGenerator.UserControls.Controls
{
    /// <summary>
    /// Interaction logic for DropdownField.xaml
    /// </summary>
    public partial class SpecialDropdownField : Interfaces.IControlField
    {
        public event ControlModified OnControlModified;
        public event ControlFinishedEditing OnControlFinishedEditing;

        public SpecialDropdownField()
        {
            InitializeComponent();
        }

        public void AddDropdownItems(List<Object.FormDropdownItem> DropdownItems, object value)
        {
            SelectComboBox.Items.Clear();
            var baseColour = (Brush) new BrushConverter().ConvertFromString("#303030");

            DropdownItems.ForEach(Item =>
            {
                var BoxItem = new ComboBoxItem()
                {
                    Content = Item,
                    Background = baseColour,
                    BorderBrush = baseColour
                };

                BoxItem.Resources.Add(SystemColors.HighlightBrushKey, baseColour);
                BoxItem.Resources.Add(SystemColors.WindowBrushKey, baseColour);

                if (value != null && Item.Value.ToString() == value.ToString())
                {
                    BoxItem.IsSelected = true;
                }

                SelectComboBox.Items.Add(BoxItem);
            });
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

            SelectComboBox.Width = formControlSettings.ValueWidth;

            if (!formControlSettings.CanEdit)
            {
                SelectComboBox.IsEnabled = false;
            }

            if (formControlSettings.Required)
            {
                DisplayNameTextBlock.ToolTip = formControlSettings.RequiredText;
            }

            if (formControlSettings.ToolTip != string.Empty)
            {
                SelectComboBox.ToolTip = formControlSettings.ToolTip;
            }

            SelectComboBox.SelectionChanged += (sen, e) =>
            {
                var selectedItem = (ComboBoxItem)SelectComboBox.SelectedItem;
                if (selectedItem != null)
                {
                    var DropdownItem = (FormDropdownItem)selectedItem.Content;
                    formControlSettings.SetValue(DropdownItem.Value);
                    OnControlModified?.Invoke(DropdownItem.Value);
                    OnControlFinishedEditing?.Invoke(DropdownItem.Value);
                }
            };
        }

        public bool Validate()
        {
            var selectedItem = (ComboBoxItem)SelectComboBox.SelectedItem;
            var Valid = selectedItem != null;

            if (Valid)
            {
                SelectComboBox.BorderBrush = (Brush)new BrushConverter().ConvertFromString("#FFABABAB");
            }
            else
            {
                SelectComboBox.BorderBrush = Brushes.Red;
            }

            return Valid;
        }

        public object GetValue()
        {
            var selectedItem = (ComboBoxItem)SelectComboBox.SelectedItem;
            var DropdownItem = (FormDropdownItem) selectedItem?.Content;
            return DropdownItem?.Value;
        }

        public void SetVisibility(bool IsVisible)
        {
            this.Visibility = IsVisible ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
