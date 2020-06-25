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
        string name, id, key;
        string dds, mms, yys;

        bool waitS = true, waitI = true;
        DateTime date = DateTime.Now;
        DatePicker picker = new DatePicker();
        List<Data> listS = new List<Data>();
        List<Data> listR = new List<Data>();
        List<DataC> listС = new List<DataC>();
        List<Item> listI = new List<Item>();

        IFirebaseClient client;
        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "",
            BasePath = "https://project-b58e4.firebaseio.com/"
        };

        public SetItems(string name, string id, string key, DatePicker picker)
        {
            InitializeComponent();
            this.name = name;
            this.id = id;
            this.key = key;
            this.picker = picker;
            this.dds = picker.SelectedDate.Value.Day.ToString();
            this.mms = picker.SelectedDate.Value.Month.ToString();
            this.yys = picker.SelectedDate.Value.Year.ToString();

            client = new FireSharp.FirebaseClient(config);
            labelR.Content = "Комплектация \"" + name + "\"";
            if (picker.SelectedDate.Value.Day != date.Day || picker.SelectedDate.Value.Month != date.Month || picker.SelectedDate.Value.Year != date.Year)
            {
                dataGridS.IsEnabled = false;
                dataGridR.IsEnabled = false;
            }
            else
            {
                dataGridS.IsEnabled = true;
                dataGridR.IsEnabled = true;
            }
            LoadS();
        }

        private async void LoadS()
        {
            try
            {
                progress.Value = 0;
                progress.Visibility = Visibility.Visible;
                listS.Clear();
                listR.Clear();
                listС.Clear();
                listI.Clear();

                FirebaseResponse resp = await client.GetTaskAsync("Counter/nodeC/" + dds + mms + yys);
                CounterC get = resp.ResultAs<CounterC>();
                progress.Value = 10;

                int count = Convert.ToInt32(get.cnt);
                if (count != 0)
                {
                    for (int i = 1; i <= count; i++)
                    {
                        try
                        {
                            Load_S(dds, mms, yys, i, count);
                        }
                        catch
                        {
                            dataGridS.ItemsSource = null;
                            dataGridS.ItemsSource = listS;

                            dataGridR.ItemsSource = null;
                            dataGridR.ItemsSource = listR;

                            progress.Visibility = Visibility.Hidden;

                            labelS.Content = "Вся комплектация на складе (" + listS.Count + " ед.)";
                            labelR.Content = "Комплектация \"" + name + "\" (" + listR.Count + " ед.)";
                        }
                    }
                }
                else
                {

                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private async void LoadI()
        {
            try
            {              
                FirebaseResponse resp1 = await client.GetTaskAsync("Out/" + dds + mms + yys + id);
                ItemsCount getC = resp1.ResultAs<ItemsCount>();
                int count = Convert.ToInt32(getC.cnt);
                if (count != 0)
                {
                    for (int i = 1; i <= count; i++)
                    {
                        Load_I(i, count);
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private async void Load_S(string dds, string mms, string yys, int i, int count)
        {
            FirebaseResponse response = await client.GetTaskAsync("Category/" + dds + mms + yys + i);
            Data data = response.ResultAs<Data>();
            listS.Add(data);
            progress.Value += 35 / count;
            if (listS.Count == count)
                LoadI();
        }

        private async void Load_I(int i, int count)
        {
            FirebaseResponse response = await client.GetTaskAsync("Out/" + key + "/" + i);
            Item itemsR = response.ResultAs<Item>();
            listI.Add(itemsR);
            progress.Value += 20 / count;
            if (listI.Count == count)
                Sort();
        }

        void Sort()
        {
            progress.Value = 60;
            int l = listS.Count;
            foreach (Item item in listI)
                for (int i = 0; i < l; i++)
                {
                    if (listS.ElementAt(i).Name == item.Name)
                    {
                        Data d = listS.ElementAt(i);
                        DataC dataC = new DataC();
                        dataC.Id = d.Id;
                        dataC.Name = d.Name;
                        dataC.Now = d.Now;
                        listС.Add(dataC);
                        listS.Remove(d);
                        progress.Value += 30 / l;
                        l--;
                        d.Rem = d.Now;
                        d.Now = item.Now;
                        listR.Add(d);
                    }
                }

            dataGridS.ItemsSource = null;
            dataGridS.ItemsSource = listS;

            dataGridR.ItemsSource = null;
            dataGridR.ItemsSource = listR;

            progress.Visibility = Visibility.Hidden;

            labelS.Content = "Вся комплектация на складе (" + listS.Count + " ед.)";
            labelR.Content = "Комплектация \"" + name + "\" (" + listR.Count + " ед.)";
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
                Data data = dataGridS.SelectedItem as Data;
                SetQuantity setQuantity = new SetQuantity();
                if (setQuantity.ShowDialog() == true)
                {
                    DataC dataC = new DataC();
                    dataC.Id = data.Id;
                    dataC.Name = data.Name;
                    dataC.Now = data.Now;
                    listС.Add(dataC);
                    listS.Remove(data);
                    data.Rem = data.Now;
                    data.Now = setQuantity.Value;
                    listR.Add(data);

                    dataGridR.ItemsSource = null;
                    dataGridR.ItemsSource = listR;
                    dataGridS.Items.Refresh();

                    labelS.Content = "Вся комплектация на складе (" + listS.Count + " ед.)";
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

                labelS.Content = "Вся комплектация на складе (" + listS.Count + " ед.)";
                if (listR.Count == 0)
                    labelR.Content = "Комплектация \"" + name + "\"";
                else
                    labelR.Content = "Комплектация \"" + name + "\" (" + listR.Count + " ед.)";
            }
            catch { }
        }
    }
}
