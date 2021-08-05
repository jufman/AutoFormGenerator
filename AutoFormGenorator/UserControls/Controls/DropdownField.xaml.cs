﻿using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using AutoFormGenerator.Events;
using AutoFormGenerator.Object;

namespace AutoFormGenerator.UserControls.Controls
{
    /// <summary>
    /// Interaction logic for DropdownField.xaml
    /// </summary>
    public partial class DropdownField : UserControl, Interfaces.IControlField
    {
        public event ControlModified OnControlModified;
        public event ControlFinishedEditing OnControlFinishedEditing;

        public DropdownField()
        {
            InitializeComponent();
        }

        public void BuildDisplay(FormControlSettings formControlSettings, List<DropdownItem> DropdownItems)
        {
            Width = formControlSettings.ControlWidth;
            Height = formControlSettings.ControlHeight;

            DisplayNameTextBlock.Text = formControlSettings.DisplayValue;

            foreach (var DropdownItem in DropdownItems)
            {
                var Cbi = new ComboBoxItem
                {
                    Content = DropdownItem.Name,
                    Tag = DropdownItem.Value
                };

                if (formControlSettings.Value != null && DropdownItem.Value.ToLower() == formControlSettings.Value.ToString().ToLower())
                {
                    Cbi.IsSelected = true;
                }

                SelectComboBox.Items.Add(Cbi);
            }

            if (formControlSettings.FixedWidth)
            {
                DisplayNameTextBlock.Width = formControlSettings.DisplayNameWidth;
            }
            else
            {
                Margin = new Thickness(0, 0, 30, 0);
            }

            DisplayNameTextBlock.Width = formControlSettings.ValueWidth;
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
                formControlSettings.SetValue(selectedItem.Tag.ToString());
                OnControlModified?.Invoke( selectedItem.Tag.ToString());
                OnControlFinishedEditing?.Invoke(selectedItem.Tag.ToString());
            };
        }

        public void BuildDisplay(FormControlSettings formControlSettings)
        {

        }

        public bool Validate()
        {
            return true;
        }

        public object GetValue()
        {
            var selectedItem = (ComboBoxItem)SelectComboBox.SelectedItem;
            return selectedItem.Tag;
        }


        public class DropdownItem
        {
            public string Name { get; set; }

            public string Value { get; set; }
        }
    }
}
