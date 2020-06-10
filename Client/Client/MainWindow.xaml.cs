using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Client
{
    public partial class MainWindow : Window
    {
        Counter get = new Counter();
        static List<int> free = new List<int>();
        static List<Data> list = new List<Data>();
        int pos = 0;

        IFirebaseClient client;
        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "",
            BasePath = "https://project-b58e4.firebaseio.com/"
        };

        public MainWindow()
        {
            InitializeComponent();
            client = new FireSharp.FirebaseClient(config);
            Load();
        }

        private async void Load()
        {
            try
            {
                refresh.IsEnabled = false;

                switch (tab.SelectedIndex)
                {
                    case 0:
                        {
                            list.Clear();
                            dataGrid.Items.Refresh();
                            tab.IsEnabled = false;

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
                                        list.Add(data);
                                    }
                                    catch { }
                                }
                            dataGrid.ItemsSource = null;
                            dataGrid.ItemsSource = list;
                            tab.IsEnabled = true;
                            break;
                        }
                    case 1:
                        {
                            list.Clear();
                            dataGrid1.Items.Refresh();
                            tab.IsEnabled = false;

                            FirebaseResponse resp = await client.GetTaskAsync("Counter/node");
                            get = resp.ResultAs<Counter>();

                            int count = Convert.ToInt32(get.cntP);
                            if (count != 0)
                                for (int i = 1; i <= count; i++)
                                {
                                    try
                                    {
                                        FirebaseResponse response = await client.GetTaskAsync("Products/" + i);
                                        Data data = response.ResultAs<Data>();
                                        list.Add(data);
                                    }
                                    catch { }
                                }
                            dataGrid1.ItemsSource = null;
                            dataGrid1.ItemsSource = list;
                            tab.IsEnabled = true;
                            break;
                        }
                    case 2:
                        {
                            list.Clear();
                            dataGrid2.Items.Refresh();
                            tab.IsEnabled = false;

                            FirebaseResponse resp = await client.GetTaskAsync("Counter/node");
                            get = resp.ResultAs<Counter>();

                            int count = Convert.ToInt32(get.cntO);
                            if (count != 0)
                                for (int i = 1; i <= count; i++)
                                {
                                    try
                                    {
                                        FirebaseResponse response = await client.GetTaskAsync("Out/" + i);
                                        Data data = response.ResultAs<Data>();
                                        list.Add(data);
                                    }
                                    catch { }
                                }
                            dataGrid2.ItemsSource = null;
                            dataGrid2.ItemsSource = list;
                            tab.IsEnabled = true;
                            break;
                        }
                }
                refresh.IsEnabled = true;
                refresh.Content = "Обновить";
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void Free_Place()
        {
            try
            {
                int count = 0;
                switch (tab.SelectedIndex)
                {
                    case 0:
                        {
                            count = Convert.ToInt32(get.cnt);
                            break;
                        }
                    case 1:
                        {
                            count = Convert.ToInt32(get.cntP);
                            break;
                        }
                    case 2:
                        {
                            count = Convert.ToInt32(get.cntO);
                            break;
                        }
                }
                free.Clear();

                for (int i = 1; i <= count; i++)
                {
                    free.Add(i);
                }

                for (int i = 1; i <= count; i++)
                {
                    for (int j = 0; j < list.Count; j++)
                    {
                        try
                        {
                            if (list.ElementAt(j).Id == i.ToString())
                            {
                                free.Remove(i);
                                break;
                            }
                        }
                        catch { }
                    }
                }

            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void Button_Click_Refresh(object sender, RoutedEventArgs e)
        {
            refresh.Content = "...";
            Load();
        }

        private async void Button_Click_Delete(object sender, RoutedEventArgs e)
        {
            delete.Content = "Подождите...";
            try
            {
                Data d = new Data();

                string path = "";
                if (pos == 0)
                {
                    path = "Category/";
                    d = dataGrid.SelectedItem as Data;
                }
                if (pos == 1)
                {
                    path = "Products/";
                    d = dataGrid1.SelectedItem as Data;
                }
                if (pos == 2)
                {
                    path = "Out/";
                    d = dataGrid2.SelectedItem as Data;
                }
                DeleteDialog dialog = new DeleteDialog(d.Name);
                if (dialog.ShowDialog() == true)
                {
                    await client.DeleteTaskAsync(path + d.Id);
                    list.Remove(d);
                    switch (tab.SelectedIndex)
                    {
                        case 0:
                            {
                                dataGrid.Items.Refresh();
                                break;
                            }
                        case 1:
                            {
                                dataGrid1.Items.Refresh();
                                break;
                            }
                        case 2:
                            {
                                dataGrid2.Items.Refresh();
                                break;
                            }
                    }
                }
            }
            catch { }
            delete.Content = "Удалить";
        }

        private async void dataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            try
            {
                FirebaseResponse resp = await client.GetTaskAsync("Counter/node");
                Counter get = resp.ResultAs<Counter>();

                int value = 0;
                string path = "";

                if (pos == 0)
                {
                    path = "Category/";
                    value = Convert.ToInt32(get.cnt) + 1;
                }
                if (pos == 1)
                {
                    path = "Products/";
                    value = Convert.ToInt32(get.cntP) + 1;
                }
                if (pos == 2)
                {
                    path = "Out/";
                    value = Convert.ToInt32(get.cntO) + 1;
                }

                var data = e.Row.Item as Data;
                if (data.New == null)
                {

                    if (data.Name != null)
                        if (data.Id == null)
                        {
                            Free_Place();
                            if (free.Count != 0)
                            {
                                data.Id = free.ElementAt(0).ToString();
                                await client.SetTaskAsync(path + free.ElementAt(0).ToString(), data);
                                free.RemoveAt(0);
                            }
                            else
                            {
                                data.Id = value.ToString();
                                await client.SetTaskAsync(path + value.ToString(), data);
                                Counter obj = new Counter();
                                switch (tab.SelectedIndex)
                                {
                                    case 0:
                                        {
                                            obj = new Counter
                                            {
                                                cnt = value.ToString(),
                                                cntP = get.cntP,
                                                cntO = get.cntO
                                            };
                                            break;
                                        }
                                    case 1:
                                        {
                                            obj = new Counter
                                            {
                                                cnt = get.cnt,
                                                cntP = value.ToString(),
                                                cntO = get.cntO
                                            };
                                            break;
                                        }
                                    case 2:
                                        {
                                            obj = new Counter
                                            {
                                                cnt = get.cnt,
                                                cntP = get.cntP,
                                                cntO = value.ToString()
                                            };
                                            break;
                                        }

                                }
                                await client.SetTaskAsync("Counter/node", obj);
                            }
                        }
                        else
                        {
                            await client.SetTaskAsync(path + data.Id, data);
                        }
                }
                else
                {
                    try
                    {
                        if (data.Name != null)
                        {
                            int i = list.IndexOf(data);
                            double all, now;
                            if (data.All != null && data.All != " " && data.All != "")
                                all = Convert.ToDouble(data.All);
                            else
                                all = 0;

                            if (data.Now != null && data.Now != " " && data.Now != "")
                                now = Convert.ToDouble(data.Now);
                            else
                                now = 0;

                            data.All = (all + Convert.ToDouble(data.New)).ToString();
                            data.Now = (now + Convert.ToDouble(data.New)).ToString();
                            data.New = null;
                            await client.SetTaskAsync(path + data.Id, data);
                        }
                        else
                            data.New = null;
                        switch (pos)
                        {
                            case 0:
                                {
                                    dataGrid.Items.Refresh();
                                    break;
                                }
                            case 1:
                                {
                                    dataGrid1.Items.Refresh();
                                    break;
                                }
                            case 2:
                                {
                                    dataGrid2.Items.Refresh();
                                    break;
                                }
                        }
                    }
                    catch
                    {
                        data.New = null;
                        MessageBox.Show("Некорректность вводимых или хранимых данных.");
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void tab_Loaded(object sender, RoutedEventArgs e)
        {
            Load();
        }

        private void tab_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (tab.SelectedIndex != pos)
                {
                    pos = tab.SelectedIndex;
                    Load();
                }
            }
            catch { }

        }

        void ShowHideDetails(object sender, RoutedEventArgs e)
        {
            try
            {
                Data d = dataGrid2.SelectedItem as Data;
                MessageBox.Show(d.Name);
            }
            catch { }
        }
    }
}
