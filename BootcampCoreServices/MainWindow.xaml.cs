using CsvHelper;
using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace BootcampCoreServices
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private OrderDB db = new OrderDB();
        String[] fileName;
        public MainWindow()
        {
            InitializeComponent();
            db.Database.Initialize(true);
        }
        private void Window_Drop(object sender, DragEventArgs e)
        {
            try
            {
                fileName = (String[])e.Data.GetData(DataFormats.FileDrop, true);
                if (fileName.Length > 0)
                {
                    String filePath = fileName[0].ToString();
                    string whatExtension;
                    if (CheckExtension(filePath, out whatExtension))
                    {
                        ShowRaportButtons();
                        HeaderTypeOfFileTextBlock.Visibility = Visibility.Visible;
                        if (whatExtension == ".csv")
                        {
                            TypeOfFileTextBlock.Text = whatExtension;
                            LoadCSV(filePath);
                        }
                        else if (whatExtension == ".json")
                        {
                            TypeOfFileTextBlock.Text = whatExtension;
                            LoadJSON(filePath);
                        }
                        else if (whatExtension == ".xml")
                        {
                            TypeOfFileTextBlock.Text = whatExtension;
                            LoadXML(filePath);
                        }
                        ShowRaportButtons();
                    }
                    else
                    {
                        MessageBox.Show("Wybrano zły format pliku!");
                    }
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private Boolean CheckExtension(String FilePath, out string WhatExtension)
        {
            Boolean Flag = false;
            try
            {
                WhatExtension = System.IO.Path.GetExtension(FilePath);

                if (WhatExtension != String.Empty)
                {
                    if (WhatExtension == ".csv")
                    {
                        Flag = true;
                    }
                    if (WhatExtension == ".json")
                    {
                        Flag = true;
                    }
                    if (WhatExtension == ".xml")
                    {
                        Flag = true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return Flag;
        }
        public void LoadJSON(string FilePath)
        {
            using (StreamReader streamReader = new StreamReader(FilePath))
            {
                string json = streamReader.ReadToEnd();
                List<Request> myDeserializedObjList = (List<Request>)JsonConvert.DeserializeObject(json, typeof(List<Request>));
                foreach (var item in myDeserializedObjList)
                {
                    db.Orders.Add(new Request { ClientId = item.ClientId, Name = item.Name, Price = item.Price, Quantity = item.Quantity, RequestId = item.RequestId });

                    XmlTextBlock.Text = XmlTextBlock.Text + (
                        item.ClientId.ToString() + " " +
                        item.Name.ToString() + " " +
                        item.Price.ToString() + " " +
                        item.Quantity.ToString() + " " +
                        item.RequestId.ToString()) +
                        " ";
                }
                db.SaveChanges();
            }
        }
        public void LoadXML(string FilePath)
        {
            var xml = System.IO.File.ReadAllText(FilePath);
            using (StreamReader streamReader = new StreamReader(FilePath))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Requests));
                Requests data = (Requests)serializer.Deserialize(streamReader);
                foreach (var item in data.requests)
                {
                    db.Orders.Add(new Request { ClientId = item.ClientId, Name = item.Name, Price = item.Price, Quantity = item.Quantity, RequestId = item.RequestId });

                    XmlTextBlock.Text = XmlTextBlock.Text + (
                        item.ClientId.ToString() + " " +
                        item.Name.ToString() + " " +
                        item.Price.ToString() + " " +
                        item.Quantity.ToString() + " " +
                        item.RequestId.ToString()) +
                        " ";
                }
                db.SaveChanges();
            }
        }
        public void LoadCSV(string FilePath)
        {
            StreamReader streamReader = new StreamReader(FilePath);
            string line = streamReader.ReadLine();
            string[] value = line.Split(',');
            DataTable dataTable = new DataTable();
            DataRow dataRow;
            foreach (string dataColumn in value)
            {
                dataTable.Columns.Add(new DataColumn(dataColumn));
            }

            while (!streamReader.EndOfStream)
            {
                value = streamReader.ReadLine().Split(',');
                if (value.Length == dataTable.Columns.Count)
                {
                    dataRow = dataTable.NewRow();
                    dataRow.ItemArray = value;
                    dataTable.Rows.Add(dataRow);
                }
            }
            foreach (DataRow col in dataTable.Rows)
            {
                var clientId = col.ItemArray[0].ToString();
                var requestId = System.Convert.ToInt64(col.ItemArray[1]);
                var name = col.ItemArray[2].ToString();
                var quantity = System.Convert.ToInt32(col.ItemArray[3]);
                var price = Double.Parse(col.ItemArray[4].ToString(), CultureInfo.InvariantCulture);

                db.Orders.Add(new Request
                {
                    ClientId = clientId,
                    RequestId = requestId,
                    Name = name,
                    Quantity = quantity,
                    Price = price,
                });

                XmlTextBlock.Text = XmlTextBlock.Text + (clientId
                   + " " + requestId
                   + " " + name
                   + " " + quantity
                   + " " + price
                   + " ");

            }
            db.SaveChanges();
        }
        //a.Ilość zamówień
        private void AmountOfOrders_Button_Click(object sender, RoutedEventArgs e)
        {
            DataGrid1.Visibility = Visibility.Hidden;
            RaportTextBlock.Visibility = Visibility.Visible;
            HidePriceRangeItems();
            var AmountOfOrders =
            db.Orders
                .Count();
            RaportTextBlock.Text = "Ilość zamówień: " + AmountOfOrders.ToString() + Environment.NewLine;
        }
        //b.Ilość zamówień dla klienta o wskazanym identyfikatorze 
        private void AmountOfOrdersByClientId_Button_Click(object sender, RoutedEventArgs e)
        {
            DataGrid1.Visibility = Visibility.Hidden;
            HidePriceRangeItems();
            ChooseClientIdWindow chooseClientIdWindow = new ChooseClientIdWindow();
            chooseClientIdWindow.ClientIdComboBox.ItemsSource = (db.Orders).GroupBy(f => f.ClientId).ToList();
            chooseClientIdWindow.ClientIdComboBox.DisplayMemberPath = "ClientId";
            chooseClientIdWindow.ShowDialog();
            var AmountOfOrdersByClientId =
            db.Orders
                .Where(s => s.ClientId.Equals(ChosenClientTextBlock.Text.ToString()))
                .Count();
            RaportTextBlock.Text = "Ilość zamówień dla klienta o wskazanym identyfikatorze: " + AmountOfOrdersByClientId.ToString() + Environment.NewLine;
        }
        //c.Łączna kwota zamówień 
        private void TotalPriceOfOrders_Button_Click(object sender, RoutedEventArgs e)
        {
            DataGrid1.Visibility = Visibility.Hidden;
            ChosenClientTextBlock.Visibility = Visibility.Hidden;
            HeaderChosenClientTextBlock.Visibility = Visibility.Hidden;
            HidePriceRangeItems();
            var totalPriceOfOrders =
            db.Orders
                .Select(s => s.Price)
                .Sum();
            RaportTextBlock.Text = "Łączna kwota zamówień: " + totalPriceOfOrders.ToString() + Environment.NewLine;
        }
        //d.Łączna kwota zamówień dla klienta o wskazanym identyfikatorze 
        private void TotalPriceOfOrdersByClientId_Button_Click(object sender, RoutedEventArgs e)
        {
            DataGrid1.Visibility = Visibility.Hidden;
            HidePriceRangeItems();
            ChooseClientIdWindow chooseClientIdWindow = new ChooseClientIdWindow();
            chooseClientIdWindow.ClientIdComboBox.ItemsSource = (db.Orders).GroupBy(f => f.ClientId).ToList();
            chooseClientIdWindow.ClientIdComboBox.DisplayMemberPath = "ClientId";
            chooseClientIdWindow.ShowDialog();
            var totalPriceOfOrdersByClientId =
            db.Orders
                .Where(d => d.ClientId.Equals(ChosenClientTextBlock.Text.ToString()))
                .Select(s => s.Price)
                .Sum();
            RaportTextBlock.Text = "Łączna kwota zamówień dla klienta o wskazanym identyfikatorze: " + totalPriceOfOrdersByClientId.ToString() + Environment.NewLine;
        }
        //e.Lista wszystkich zamówień 
        private void ListOfAllOrders_Button_Click(object sender, RoutedEventArgs e)
        {
            ChosenClientTextBlock.Visibility = Visibility.Hidden;
            HeaderChosenClientTextBlock.Visibility = Visibility.Hidden;
            RaportTextBlock.Visibility = Visibility.Hidden;
            HidePriceRangeItems();
            var listOfAllOrders = db.Orders.ToList();
            string sortBy = SortByCommonBox.SelectedItem.ToString();
            if (sortBy == "System.Windows.Controls.ComboBoxItem: ClientId")
            {
                var query =
                 from item in db.Orders
                 orderby item.ClientId
                 select new { item.Name, item.ClientId, item.Quantity, item.RequestId, item.Price };
                DataGrid1.Visibility = Visibility.Visible;
                DataGrid1.ItemsSource = query.ToList();

            }
            else if (sortBy == "System.Windows.Controls.ComboBoxItem: RequestId")
            {

                var query =
                 from item in db.Orders
                 orderby item.RequestId
                 select new { item.Name, item.ClientId, item.Quantity, item.RequestId, item.Price };
                DataGrid1.Visibility = Visibility.Visible;
                DataGrid1.ItemsSource = query.ToList();
            }
            else if (sortBy == "System.Windows.Controls.ComboBoxItem: Name")
            {
                var query =
                 from item in db.Orders
                 orderby item.Name
                 select new { item.Name, item.ClientId, item.Quantity, item.RequestId, item.Price };
                DataGrid1.Visibility = Visibility.Visible;
                DataGrid1.ItemsSource = query.ToList();

            }
            else if (sortBy == "System.Windows.Controls.ComboBoxItem: Quantity")
            {

                var query =
                 from item in db.Orders
                 orderby item.Quantity
                 select new { item.Name, item.ClientId, item.Quantity, item.RequestId, item.Price };
                DataGrid1.Visibility = Visibility.Visible;
                DataGrid1.ItemsSource = query.ToList();
            }
            else if (sortBy == "System.Windows.Controls.ComboBoxItem: Price")
            {

                var query =
                 from item in db.Orders
                 orderby item.Price
                 select new { item.Name, item.ClientId, item.Quantity, item.RequestId, item.Price };
                DataGrid1.Visibility = Visibility.Visible;
                DataGrid1.ItemsSource = query.ToList();
            }

        }
        //f.Lista zamówień dla klienta o wskazanym identyfikatorze 
        private void ListOfOrdersByClientId_Button_Click(object sender, RoutedEventArgs e)
        {
            HidePriceRangeItems();
            RaportTextBlock.Visibility = Visibility.Hidden;
            DataGrid1.Visibility = Visibility.Visible;
            ChooseClientIdWindow chooseClientIdWindow = new ChooseClientIdWindow();
            chooseClientIdWindow.ClientIdComboBox.ItemsSource = (db.Orders).GroupBy(f => f.ClientId).ToList();
            chooseClientIdWindow.ClientIdComboBox.DisplayMemberPath = "ClientId";
            chooseClientIdWindow.ShowDialog();
            var listOfAllOrders = db.Orders.ToList();
            string sortBy = SortByCommonBox.SelectedItem.ToString();
            if (sortBy == "System.Windows.Controls.ComboBoxItem: ClientId")
            {
                var query =
                 from item in db.Orders
                 where item.ClientId == ChosenClientTextBlock.Text.ToString()
                 orderby item.ClientId
                 select new { item.Name, item.ClientId, item.Quantity, item.RequestId, item.Price };
                DataGrid1.ItemsSource = query.ToList();

            }
            else if (sortBy == "System.Windows.Controls.ComboBoxItem: RequestId")
            {

                var query =
                 from item in db.Orders
                 where item.ClientId == ChosenClientTextBlock.Text.ToString()
                 orderby item.RequestId
                 select new { item.Name, item.ClientId, item.Quantity, item.RequestId, item.Price };
                DataGrid1.ItemsSource = query.ToList();
            }
            else if (sortBy == "System.Windows.Controls.ComboBoxItem: Name")
            {
                var query =
                 from item in db.Orders
                 where item.ClientId == ChosenClientTextBlock.Text.ToString()
                 orderby item.Name
                 select new { item.Name, item.ClientId, item.Quantity, item.RequestId, item.Price };
                DataGrid1.ItemsSource = query.ToList();

            }
            else if (sortBy == "System.Windows.Controls.ComboBoxItem: Quantity")
            {

                var query =
                 from item in db.Orders
                 where item.ClientId == ChosenClientTextBlock.Text.ToString()
                 orderby item.Quantity
                 select new { item.Name, item.ClientId, item.Quantity, item.RequestId, item.Price };
                DataGrid1.ItemsSource = query.ToList();
            }
            else if (sortBy == "System.Windows.Controls.ComboBoxItem: Price")
            {

                var query =
                 from item in db.Orders
                 where item.ClientId == ChosenClientTextBlock.Text.ToString()
                 orderby item.Price
                 select new { item.Name, item.ClientId, item.Quantity, item.RequestId, item.Price };
                DataGrid1.ItemsSource = query.ToList();
            }
        }
        //g.Średnia wartość zamówienia
        private void AveragePricesOfOrders_Button_Click(object sender, RoutedEventArgs e)
        {
            HidePriceRangeItems();
            DataGrid1.Visibility = Visibility.Hidden;
            RaportTextBlock.Visibility = Visibility.Visible;
            var AveragePricesOfOrders =
            db.Orders
               .Average(f => f.Price);
            RaportTextBlock.Text = "Średnia wartość zamówienia: " + AveragePricesOfOrders.ToString() + Environment.NewLine;
        }
        //h.Średnia wartość zamówienia dla klienta o wskazanym identyfikatorze 
        private void AveragePriceOfOrdersByClientIdButton_Click(object sender, RoutedEventArgs e)
        {
            HidePriceRangeItems();
            DataGrid1.Visibility = Visibility.Hidden;
            RaportTextBlock.Visibility = Visibility.Visible;
            ChooseClientIdWindow chooseClientIdWindow = new ChooseClientIdWindow();
            chooseClientIdWindow.ClientIdComboBox.ItemsSource = (db.Orders).GroupBy(f => f.ClientId).ToList();
            chooseClientIdWindow.ClientIdComboBox.DisplayMemberPath = "ClientId";
            chooseClientIdWindow.ShowDialog();
            var averagePriceOfOrdersByClientId =
            db.Orders
               .Where(r => r.ClientId == ChosenClientTextBlock.Text.ToString())
               .Average(f => f.Price);
            RaportTextBlock.Text = "Średnia wartość zamówienia dla klienta o wskazanym identyfikatorze : " + averagePriceOfOrdersByClientId.ToString() + Environment.NewLine;
        }
        //i.Ilość zamówień pogrupowanych po nazwie 
        private void OrdersGroupByNames_Button_Click(object sender, RoutedEventArgs e)
        {
            RaportTextBlock.Text = " ";
            HidePriceRangeItems();
            DataGrid1.Visibility = Visibility.Hidden;
            RaportTextBlock.Visibility = Visibility.Visible;

            var ordersGroupByNames =
            db.Orders
               .GroupBy(g => new { g.Name })
         .Select(k => new { items = k.Key.Name, howManyItems = k.Count()});
            foreach (var item in ordersGroupByNames)
            {
                RaportTextBlock.Text = RaportTextBlock.Text  + "" + item.items + " " + item.howManyItems + " " + Environment.NewLine;
            }
            
        }
        //j.Ilość zamówień pogrupowanych po nazwie dla klienta o wskazanym identyfikatorze 
        private void OrdersByClientIdGroupByNames_Button_Click(object sender, RoutedEventArgs e)
        {
            RaportTextBlock.Text = " ";
            HidePriceRangeItems();
            DataGrid1.Visibility = Visibility.Hidden;
            RaportTextBlock.Visibility = Visibility.Visible;

            ChooseClientIdWindow chooseClientIdWindow = new ChooseClientIdWindow();
            chooseClientIdWindow.ClientIdComboBox.ItemsSource = (db.Orders).GroupBy(f => f.ClientId).ToList();
            chooseClientIdWindow.ClientIdComboBox.DisplayMemberPath = "ClientId";
            chooseClientIdWindow.ShowDialog();
            var ordersByClientIdGroupByNames =
            db.Orders
               .Where(r => r.ClientId == ChosenClientTextBlock.Text.ToString())
               .GroupBy(g => new { g.Name})
         .Select(k => new { items = k.Key.Name, howManyItems = k.Count() });
            foreach (var item in ordersByClientIdGroupByNames)
            {
                RaportTextBlock.Text = RaportTextBlock.Text + "" + item.items + " " + item.howManyItems + " " + Environment.NewLine;
            }

        }
        //k.Zamówienia w podanym przedziale cenowym
        private void OrdersBetweenTwoPrices_Button_Click(object sender, RoutedEventArgs e)
        {
            ShowPriceRangeItems();
            ChoosePricesWindow choosePricesWindow = new ChoosePricesWindow();

            choosePricesWindow.ShowDialog();
            ShowPriceRangeItems();
            if (ChosenLowerLimitTextBlock.Text=="" && ChosenLowerLimitTextBlock.Text==null)
            {

                MessageBox.Show("Nie podano ograniczeń!");
            }
            double lowerLimit = Double.Parse(ChosenLowerLimitTextBlock.Text.ToString(), CultureInfo.InvariantCulture);
            double upeerLimit = Double.Parse(ChosenUpperLimitTextBlock.Text.ToString(), CultureInfo.InvariantCulture);
            //var ordersBetweenTwoPrices =
            //db.Orders
            //    .Where(s => s.Price >= lowerLimit)
            //    .Where(s => s.Price <= upeerLimit)
            //    .ToList();
            string sortBy = SortByCommonBox.SelectedItem.ToString();
            if (sortBy == "System.Windows.Controls.ComboBoxItem: ClientId")
            {
                var query =
                 from item in db.Orders
                .Where(s => s.Price >= lowerLimit)
                .Where(s => s.Price <= upeerLimit)
                 orderby item.ClientId
                 select new { item.Name, item.ClientId, item.Quantity, item.RequestId, item.Price };
                DataGrid1.Visibility = Visibility.Visible;
                DataGrid1.ItemsSource = query.ToList();

            }
            else if (sortBy == "System.Windows.Controls.ComboBoxItem: RequestId")
            {

                var query =
                 from item in db.Orders
                .Where(s => s.Price >= lowerLimit)
                .Where(s => s.Price <= upeerLimit)
                 orderby item.RequestId
                 select new { item.Name, item.ClientId, item.Quantity, item.RequestId, item.Price };
                DataGrid1.Visibility = Visibility.Visible;
                DataGrid1.ItemsSource = query.ToList();
            }
            else if (sortBy == "System.Windows.Controls.ComboBoxItem: Name")
            {
                var query =
                 from item in db.Orders
                .Where(s => s.Price >= lowerLimit)
                .Where(s => s.Price <= upeerLimit)
                 orderby item.Name
                 select new { item.Name, item.ClientId, item.Quantity, item.RequestId, item.Price };
                DataGrid1.Visibility = Visibility.Visible;
                DataGrid1.ItemsSource = query.ToList();

            }
            else if (sortBy == "System.Windows.Controls.ComboBoxItem: Quantity")
            {

                var query =
                 from item in db.Orders
                .Where(s => s.Price >= lowerLimit)
                .Where(s => s.Price <= upeerLimit)
                 orderby item.Quantity
                 select new { item.Name, item.ClientId, item.Quantity, item.RequestId, item.Price };
                DataGrid1.Visibility = Visibility.Visible;
                DataGrid1.ItemsSource = query.ToList();
            }
            else if (sortBy == "System.Windows.Controls.ComboBoxItem: Price")
            {

                var query =
                 from item in db.Orders
                .Where(s => s.Price >= lowerLimit)
                .Where(s => s.Price <= upeerLimit)
                 orderby item.Price
                 select new { item.Name, item.ClientId, item.Quantity, item.RequestId, item.Price };
                DataGrid1.Visibility = Visibility.Visible;
                DataGrid1.ItemsSource = query.ToList();
            }
        }
        public void HidePriceRangeItems()
        {
            HeaderPriceRangeTextBlock.Visibility = Visibility.Hidden;
            HeaderUpperLimitTextBlock.Visibility = Visibility.Hidden;
            HeaderLowerLimitTextBlock.Visibility = Visibility.Hidden;
            ChosenUpperLimitTextBlock.Visibility = Visibility.Hidden;
            ChosenLowerLimitTextBlock.Visibility = Visibility.Hidden;
        }
        public void ShowPriceRangeItems()
        {
            HeaderPriceRangeTextBlock.Visibility = Visibility.Visible;
            HeaderUpperLimitTextBlock.Visibility = Visibility.Visible;
            HeaderLowerLimitTextBlock.Visibility = Visibility.Visible;
            ChosenUpperLimitTextBlock.Visibility = Visibility.Visible;
            ChosenLowerLimitTextBlock.Visibility = Visibility.Visible;
        }
        public void ShowRaportButtons()
        {
            AmountOfOrders_Button.Visibility = Visibility.Visible;
            AmountOfOrdersByClientId_Button.Visibility = Visibility.Visible;
            TotalPriceOfOrders_Button.Visibility = Visibility.Visible;
            TotalPriceOfOrdersByClientId_Button.Visibility = Visibility.Visible;
            ListOfAllOrders_Button.Visibility = Visibility.Visible;
            ListOfOrdersByClientId_Button.Visibility = Visibility.Visible;
            AveragePricesOfOrders_Button.Visibility = Visibility.Visible;
            AveragePriceOfOrdersByClientIdButton.Visibility = Visibility.Visible;
            OrdersGroupByNames_Button.Visibility = Visibility.Visible;
            OrdersByClientIdGroupByNames_Button.Visibility = Visibility.Visible;
            OrdersBetweenTwoPrices_Button.Visibility = Visibility.Visible;
            SortByCommonBox.Visibility = Visibility.Visible;
            SortByHeaderTextBlock.Visibility = Visibility.Visible;
        }
    }

}

