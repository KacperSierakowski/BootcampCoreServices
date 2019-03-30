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
                    }
                    else
                    {
                        MessageBox.Show("You are choose wrong file!");
                    }
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void GenerateRaport(object sender, System.EventArgs e)
        {
            int Upper = 100;
            int Lower = 10;
            string clientId = "1";

            //a.Ilość zamówień
            var AmountOfOrders =
            db.Orders
                .Count();
            RaportTextBlock.Text = RaportTextBlock.Text + "Ilość zamówień: " + AmountOfOrders.ToString() + Environment.NewLine;
            //b.Ilość zamówień dla klienta o wskazanym identyfikatorze 
            var AmountOfOrdersByClientId =
            db.Orders
                .Where(s => s.ClientId.Equals(clientId))
                .Count();
            RaportTextBlock.Text = RaportTextBlock.Text + "Ilość zamówień dla klienta o wskazanym identyfikatorze: " + AmountOfOrdersByClientId.ToString() + Environment.NewLine;
            //c.Łączna kwota zamówień 
            var totalPriceOfOrders =
            db.Orders
                .Select(s => s.Price)
                .Sum();
            RaportTextBlock.Text = RaportTextBlock.Text + "Łączna kwota zamówień: " + totalPriceOfOrders.ToString() + Environment.NewLine;
            //d.Łączna kwota zamówień dla klienta o wskazanym identyfikatorze 
            var totalPriceOfOrdersByClientId =
            db.Orders
                .Where(d => d.ClientId.Equals(clientId))
                .Select(s => s.Price)
                .Sum();
            RaportTextBlock.Text = RaportTextBlock.Text + "Łączna kwota zamówień dla klienta o wskazanym identyfikatorze: " + totalPriceOfOrdersByClientId.ToString() + Environment.NewLine;


            //e.Lista wszystkich zamówień 
            var listOfAllOrders = db.Orders.ToList();
            // RaportTextBlock.Text = listOfAllOrders.ToString();
            //f.Lista zamówień dla klienta o wskazanym identyfikatorze 
            var listOfOrdersByClientId =
            db.Orders
                .Where(s => s.ClientId.Equals(clientId))
                .ToList();



            //g.Średnia wartość zamówienia
            var AveragePricesOfOrders =
            db.Orders
               .Average(f => f.Price);
            RaportTextBlock.Text = RaportTextBlock.Text + "Średnia wartość zamówienia: " + AveragePricesOfOrders.ToString() + Environment.NewLine;
            //h.Średnia wartość zamówienia dla klienta o wskazanym identyfikatorze 
            var averagePriceOfOrdersByClientId =
            db.Orders
               .Where(r => r.ClientId == clientId)
               .Average(f => f.Price);
            RaportTextBlock.Text = RaportTextBlock.Text + "Średnia wartość zamówienia dla klienta o wskazanym identyfikatorze : " + averagePriceOfOrdersByClientId.ToString() + Environment.NewLine;


            //i.Ilość zamówień pogrupowanych po nazwie 
            var ordersGroupByNames =
            db.Orders
               .GroupBy(g => g.Name).Count();
            //j.Ilość zamówień pogrupowanych po nazwie dla klienta o wskazanym identyfikatorze 
            var ordersByClientIdGroupByNames =
            db.Orders
               .Where(r => r.ClientId == clientId)
               .GroupBy(g => g.Name)
               .Count();
            //k.Zamówienia w podanym przedziale cenowym
            var ordersBetweenTwoPrices =
            db.Orders
                .Where(s => s.Price >= Lower)
                .Where(s => s.Price <= Upper)
                .ToList();


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
                var x =
                col.ItemArray[0].ToString();
                var y =
                System.Convert.ToInt64(col.ItemArray[1]);
                var a =
                col.ItemArray[2].ToString();
                var b =
            System.Convert.ToInt32(col.ItemArray[3]);
                var c = Double.Parse(col.ItemArray[4].ToString(), CultureInfo.InvariantCulture);

                long gf = (long)y;

                db.Orders.Add(new Request
                {
                    ClientId = x,
                    RequestId = y,
                    Name = a,
                    Quantity = b,
                    Price = c,
                });

                XmlTextBlock.Text = XmlTextBlock.Text + (x
                   + " " + y
                   + " " + a
                   + " " + b
                   + " " + c
                   + " ");

            }
            db.SaveChanges();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_8(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_9(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_10(object sender, RoutedEventArgs e)
        {

        }
    }

}

