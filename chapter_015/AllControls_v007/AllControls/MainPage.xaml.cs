using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace AllControls
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        Double currentCharges;
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void AppBarToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            DisplayToggleDialog();
        }

        private async void DisplayToggleDialog()
        {
            ContentDialog toggleDialog = new ContentDialog
            {
                Title = "You've Toggled the Item",
                Content = "The Item is On",
                PrimaryButtonText = "OK"
            };

            ContentDialogResult result = await toggleDialog.ShowAsync();
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
        }

        private void rectSizeSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (DarkBlueRect != null)
            {
                DarkBlueRect.Width = DarkBlueRect.Height = rectSizeSlider.Value;
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var selRadio = sender as RadioButton;
            // early initialization can cause an odd problem
            if (FinalCostTextBox == null) { return; }
            switch (selRadio.Name)
            {
                
                case "AlignLeftRadio":
                    {
                        FinalCostTextBox.HorizontalAlignment = HorizontalAlignment.Left;
                        break;
                    }
                case "AlignCenterRadio":
                    {
                        FinalCostTextBox.HorizontalAlignment = HorizontalAlignment.Center;
                        break;
                    }
                case "AlignRightRadio":
                    {
                        FinalCostTextBox.HorizontalAlignment = HorizontalAlignment.Right;
                        break;
                    }
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var combobox = sender as ComboBox;
            
            currentCharges = Convert.ToDouble(combobox.SelectedItem);
            if ((bool)AddFeeCheckBox.IsChecked)
            {
                currentCharges += 5.0D;
            }
            if ((bool)AddTaxesCheckBox.IsChecked)
            {
                currentCharges = Math.Round(currentCharges * 1.075,2);
            }
            FinalCostTextBox.Text = currentCharges.ToString();
        }



        private void AddFeeCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)AddFeeCheckBox.IsChecked)
            {
                currentCharges += 5.0D;
            }
            else
            {
                currentCharges -= 5.0D;
            }

            FinalCostTextBox.Text = currentCharges.ToString();
        }

        private void AddTaxesCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (PriceComboBox.SelectedIndex < 0) { return; }
            if ((bool)AddTaxesCheckBox.IsChecked)
            {
                currentCharges += Math.Round(Convert.ToDouble(PriceComboBox.SelectedItem) * .075, 2);
            }
            else
            {
                currentCharges -= Math.Round(Convert.ToDouble(PriceComboBox.SelectedItem) * .075, 2);
            }

            FinalCostTextBox.Text = currentCharges.ToString();
        }
    }
}
