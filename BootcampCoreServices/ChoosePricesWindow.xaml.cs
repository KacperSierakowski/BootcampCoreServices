using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BootcampCoreServices
{
    /// <summary>
    /// Interaction logic for ChoosePricesWindow.xaml
    /// </summary>
    public partial class ChoosePricesWindow : Window
    {
        public ChoosePricesWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (UpperLimitTextBox.Text == "")
            {
                MessageBox.Show("Nie wybrano górnego ograniczenia!");
            }
            else if (LowerLimitTextBox.Text == "")
            {
                MessageBox.Show("Nie wybrano dolnego ograniczenia!");
            }
            else if (LowerLimitTextBox.Text == "" || UpperLimitTextBox.Text == "")
            {
                MessageBox.Show("Nie wybrano ograniczeń!");
            }
            else
            {
                ((MainWindow)Application.Current.MainWindow).ChosenUpperLimitTextBlock.Text = UpperLimitTextBox.Text.ToString();
                ((MainWindow)Application.Current.MainWindow).ChosenLowerLimitTextBlock.Text = LowerLimitTextBox.Text.ToString();
                this.Close();
            }
        }

        private void LowerLimitTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(LowerLimitTextBox.Text, "[^0-9]"))
            {
                MessageBox.Show("Ograniczenie może zawierać tylko cyfry!");
                LowerLimitTextBox.Text = LowerLimitTextBox.Text.Remove(LowerLimitTextBox.Text.Length - 1);
            }
        }

        private void UpperLimitTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(UpperLimitTextBox.Text, "[^0-9]"))
            {
                MessageBox.Show("Ograniczenie może zawierać tylko cyfry!");
                UpperLimitTextBox.Text = UpperLimitTextBox.Text.Remove(UpperLimitTextBox.Text.Length - 1);
            }
        }
    }
}
