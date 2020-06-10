using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
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

namespace Client
{
    public partial class SetItems : Window
    {
        string name;
        Counter get = new Counter();
        List<Data> listS = new List<Data>();
        List<Data> listR = new List<Data>();
        List<DataC> listС = new List<DataC>();

        IFirebaseClient client;
        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "q1oz56hqzen6Qlx8zp4gbMH5EgGCsF6AkY50ZKHc",
            BasePath = "https://project-b58e4.firebaseio.com/"
        };

        public SetItems(string name)
        {
            InitializeComponent();
            this.name = name;
            client = new FireSharp.FirebaseClient(config);
            dataGridR.ItemsSource = listR;
            labelR.Content = "Комплектация \""+ name + "\"";
            Load();
        }


        private async void Load()
        {
            try
            {
                listS.Clear();
                listR.Clear();
                dataGridS.Items.Refresh();

                FirebaseResponse resp = await client.GetTaskAsync("Counter/node");
                get = resp.ResultAs<Counter>();

                int count = Convert.ToInt32(get.cnt);
                if (count != 0)
                    for (int i = 1; i <= count; i++)
                    {
                        try
                        {
                            FirebaseResponse response = await client.GetTaskAsync("Category/" + i);
                            Data data = response.ResultAs<Data>();
                            listS.Add(data);
                        }
                        catch { }
                    }
                dataGridS.ItemsSource = null;
                dataGridS.ItemsSource = listS;
                labelS.Content = "Доступная комплектация (" + listS.Count + " ед.)";
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        public List<Data> ItemsList
        {            
            get { return listR; }
        }

        private void dataGridS_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                SetQuantity setQuantity = new SetQuantity();
                if (setQuantity.ShowDialog() == true)
                {
                    Data data = dataGridS.SelectedItem as Data;
                    DataC dataC = new DataC();
                    dataC.Id = data.Id;
                    dataC.Name = data.Name;
                    dataC.Now = data.Now;
                    listС.Add(dataC);
                    listS.Remove(data);
                    data.Now = setQuantity.Value;
                    listR.Add(data);

                    dataGridR.ItemsSource = null;
                    dataGridR.ItemsSource = listR;
                    dataGridS.Items.Refresh();

                    labelS.Content = "Доступная комплектация (" + listS.Count + " ед.)";
                    if (listR.Count == 0)
                        labelR.Content = "Комплектация \"" + name + "\"";
                    else
                        labelR.Content = "Комплектация \"" + name + "\" (" + listR.Count + " ед.)";
                }
            }
            catch { }
        }

        private void dataGridR_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Data data = dataGridR.SelectedItem as Data;

                foreach (DataC dataC in listС)
                    if (dataC.Id == data.Id && dataC.Name == data.Name)
                    {
                        data.Now = dataC.Now;
                        listS.Add(data);
                        listС.Remove(dataC);
                        break;
                    }
                listR.Remove(data);

                dataGridS.ItemsSource = null;
                dataGridS.ItemsSource = listS;
                dataGridR.Items.Refresh();

                labelS.Content = "Доступная комплектация (" + listS.Count + " ед.)";
                if (listR.Count == 0)
                    labelR.Content = "Комплектация \"" + name + "\"";
                else
                    labelR.Content = "Комплектация \"" + name + "\" (" + listR.Count + " ед.)";
            }
            catch { }
        }
    }

    internal class DataC
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Now { get; set; }
    }
}
