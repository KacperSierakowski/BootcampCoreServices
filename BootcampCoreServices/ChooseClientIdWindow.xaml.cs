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
    public partial class ChooseClientIdWindow : Window
    {
        private OrderDB db = new OrderDB();
        public ChooseClientIdWindow()
        {
            InitializeComponent();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (ClientIdComboBox.Text == "")
            {
                MessageBox.Show("Nie wybrano indentyfikatora klienta!");
            }
            else
            {
                ((MainWindow)Application.Current.MainWindow).ChosenClientTextBlock.Text = ClientIdComboBox.Text.ToString();
                this.Close();
            }
        }
    }
}
