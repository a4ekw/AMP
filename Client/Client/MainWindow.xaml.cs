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
        DateTime date = DateTime.Now;
        static List<int> free = new List<int>();
        static List<Data> list = new List<Data>();
        static List<Data> listR = new List<Data>();
        static List<Item> listI = new List<Item>();
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

                int dd = date.Day, mm = date.Month, yy = date.Year;


                list.Clear();

                switch (tab.SelectedIndex)
                {
                    case 0:
                        {
                            label.Visibility = Visibility.Visible;
                            dataGrid.Visibility = Visibility.Hidden;
                            dataGrid.Items.Refresh();
                            tab.IsEnabled = false;
                            int? count = null;
                            string month = "";
                            while (count == null)
                            {
                                switch (mm)
                                {
                                    case 1:
                                        {
                                            month = "январь";
                                            break;
                                        }
                                    case 2:
                                        {
                                            month = "февраль";
                                            break;
                                        }
                                    case 3:
                                        {
                                            month = "март";
                                            break;
                                        }
                                    case 4:
                                        {
                                            month = "апрель";
                                            break;
                                        }
                                    case 5:
                                        {
                                            month = "май";
                                            break;
                                        }
                                    case 6:
                                        {
                                            month = "июнь";
                                            break;
                                        }
                                    case 7:
                                        {
                                            month = "июль";
                                            break;
                                        }
                                    case 8:
                                        {
                                            month = "август";
                                            break;
                                        }
                                    case 9:
                                        {
                                            month = "сентябрь";
                                            break;
                                        }
                                    case 10:
                                        {
                                            month = "октябрь";
                                            break;
                                        }
                                    case 11:
                                        {
                                            month = "ноябрь";
                                            break;
                                        }
                                    case 12:
                                        {
                                            month = "декабрь";
                                            break;
                                        }
                                }
                                label.Content = "Поиск данных за " + month + " " + yy + "...";

                                try
                                {
                                    FirebaseResponse resp = await client.GetTaskAsync("Counter/node/" + dd.ToString() + mm.ToString() + yy.ToString());
                                    get = resp.ResultAs<Counter>();
                                }
                                catch { }

                                if (get.cnt != null)
                                {
                                    count = Convert.ToInt32(get.cnt);
                                    break;
                                }
                                if (dd > 1)
                                    dd--;
                                else
                                {
                                    if (mm > 1)
                                    {
                                        dd = 31;
                                        mm--;
                                    }
                                    else
                                    {
                                        dd = 31;
                                        mm = 12;
                                        yy--;
                                    }
                                }
                                if (dd == date.Day && mm == date.Month && yy == date.Year - 1)
                                {
                                    get.cnt = "0";
                                    break;
                                }

                            }
                            if (count != 0)
                                for (int i = 1; i <= count; i++)
                                {
                                    try
                                    {
                                        FirebaseResponse response = await client.GetTaskAsync("Category/" + dd.ToString() + mm.ToString() + yy.ToString() + i);
                                        Data data = response.ResultAs<Data>();
                                        list.Add(data);
                                    }
                                    catch { }
                                }

                            dataGrid.ItemsSource = null;
                            dataGrid.ItemsSource = list;
                            dataGrid.Visibility = Visibility.Visible;
                            label.Visibility = Visibility.Hidden;
                            tab.IsEnabled = true;
                            break;
                        }
                    case 1:
                        {
                            label1.Visibility = Visibility.Visible;
                            dataGrid1.Visibility = Visibility.Hidden;
                            dataGrid1.Items.Refresh();
                            tab.IsEnabled = false;

                            int? count = null;
                            string month = "";
                            while (count == null)
                            {
                                switch (mm)
                                {
                                    case 1:
                                        {
                                            month = "январь";
                                            break;
                                        }
                                    case 2:
                                        {
                                            month = "февраль";
                                            break;
                                        }
                                    case 3:
                                        {
                                            month = "март";
                                            break;
                                        }
                                    case 4:
                                        {
                                            month = "апрель";
                                            break;
                                        }
                                    case 5:
                                        {
                                            month = "май";
                                            break;
                                        }
                                    case 6:
                                        {
                                            month = "июнь";
                                            break;
                                        }
                                    case 7:
                                        {
                                            month = "июль";
                                            break;
                                        }
                                    case 8:
                                        {
                                            month = "август";
                                            break;
                                        }
                                    case 9:
                                        {
                                            month = "сентябрь";
                                            break;
                                        }
                                    case 10:
                                        {
                                            month = "октябрь";
                                            break;
                                        }
                                    case 11:
                                        {
                                            month = "ноябрь";
                                            break;
                                        }
                                    case 12:
                                        {
                                            month = "декабрь";
                                            break;
                                        }
                                }
                                label1.Content = "Поиск данных за " + month + " " + yy + "...";

                                try
                                {
                                    FirebaseResponse resp = await client.GetTaskAsync("Counter/node/" + dd.ToString() + mm.ToString() + yy.ToString());
                                    get = resp.ResultAs<Counter>();
                                }
                                catch { }

                                if (get.cntP != null)
                                {
                                    count = Convert.ToInt32(get.cntP);
                                    break;
                                }
                                if (dd > 1)
                                    dd--;
                                else
                                {
                                    if (mm > 1)
                                    {
                                        dd = 31;
                                        mm--;
                                    }
                                    else
                                    {
                                        dd = 31;
                                        mm = 12;
                                        yy--;
                                    }
                                }
                                if (dd == date.Day && mm == date.Month && yy == date.Year - 1)
                                {
                                    get.cntP = "0";
                                    break;
                                }
                            }
                            if (count != 0)
                                for (int i = 1; i <= count; i++)
                                {
                                    try
                                    {
                                        FirebaseResponse response = await client.GetTaskAsync("Products/" + dd.ToString() + mm.ToString() + yy.ToString() + i);
                                        Data data = response.ResultAs<Data>();
                                        list.Add(data);
                                    }
                                    catch { }
                                }

                            dataGrid1.ItemsSource = null;
                            dataGrid1.ItemsSource = list;
                            dataGrid1.Visibility = Visibility.Visible;
                            label1.Visibility = Visibility.Hidden;
                            tab.IsEnabled = true;
                            break;
                        }
                    case 2:
                        {
                            label2.Visibility = Visibility.Visible;
                            dataGrid2.Visibility = Visibility.Hidden;
                            dataGrid2.Items.Refresh();
                            tab.IsEnabled = false;

                            int? count = null;
                            string month = "";
                            while (count == null)
                            {
                                switch (mm)
                                {
                                    case 1:
                                        {
                                            month = "январь";
                                            break;
                                        }
                                    case 2:
                                        {
                                            month = "февраль";
                                            break;
                                        }
                                    case 3:
                                        {
                                            month = "март";
                                            break;
                                        }
                                    case 4:
                                        {
                                            month = "апрель";
                                            break;
                                        }
                                    case 5:
                                        {
                                            month = "май";
                                            break;
                                        }
                                    case 6:
                                        {
                                            month = "июнь";
                                            break;
                                        }
                                    case 7:
                                        {
                                            month = "июль";
                                            break;
                                        }
                                    case 8:
                                        {
                                            month = "август";
                                            break;
                                        }
                                    case 9:
                                        {
                                            month = "сентябрь";
                                            break;
                                        }
                                    case 10:
                                        {
                                            month = "октябрь";
                                            break;
                                        }
                                    case 11:
                                        {
                                            month = "ноябрь";
                                            break;
                                        }
                                    case 12:
                                        {
                                            month = "декабрь";
                                            break;
                                        }
                                }
                                label2.Content = "Поиск данных за " + month + " " + yy + "...";

                                try
                                {
                                    FirebaseResponse resp = await client.GetTaskAsync("Counter/node/" + dd.ToString() + mm.ToString() + yy.ToString());
                                    get = resp.ResultAs<Counter>();
                                }
                                catch { }

                                if (get.cntO != null)
                                {
                                    count = Convert.ToInt32(get.cntO);
                                    break;
                                }
                                if (dd > 1)
                                    dd--;
                                else
                                {
                                    if (mm > 1)
                                    {
                                        dd = 31;
                                        mm--;
                                    }
                                    else
                                    {
                                        dd = 31;
                                        mm = 12;
                                        yy--;
                                    }
                                }
                                if (dd == date.Day && mm == date.Month && yy == date.Year - 1)
                                {
                                    get.cntO = "0";
                                    break;
                                }
                            }
                            if (count != 0)
                                for (int i = 1; i <= count; i++)
                                {
                                    try
                                    {
                                        FirebaseResponse response = await client.GetTaskAsync("Out/" + dd.ToString() + mm.ToString() + yy.ToString() + i);
                                        Data data = response.ResultAs<Data>();
                                        list.Add(data);
                                    }
                                    catch { }
                                }

                            dataGrid2.ItemsSource = null;
                            dataGrid2.ItemsSource = list;
                            dataGrid2.Visibility = Visibility.Visible;
                            label2.Visibility = Visibility.Hidden;
                            tab.IsEnabled = true;
                            break;
                        }
                }

                if (dd != date.Day || mm != date.Month || yy != date.Year)
                {
                    Counter obj = new Counter();
                    switch (tab.SelectedIndex)
                    {
                        case 0:
                            {
                                obj = new Counter
                                {
                                    cnt = list.Count.ToString(),
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
                                    cntP = list.Count.ToString(),
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
                                    cntO = list.Count.ToString()
                                };
                                break;
                            }

                    }
                    await client.SetTaskAsync("Counter/node/" + date.Day.ToString() + date.Month.ToString() + date.Year.ToString(), obj);
                    string path = "";

                    if (pos == 0)
                    {
                        path = "Category/";
                    }
                    else if (pos == 1)
                    {
                        path = "Products/";
                    }
                    else if (pos == 2)
                    {
                        path = "Out/";
                    }

                    for (int i = 1; i <= list.Count; i++)
                    {
                        await client.SetTaskAsync(path + date.Day.ToString() + date.Month.ToString() + date.Year.ToString() + i, list.ElementAt(i - 1));
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
                    await client.DeleteTaskAsync(path + date.Day.ToString() + date.Month.ToString() + date.Year.ToString() + d.Id);
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
                FirebaseResponse resp = await client.GetTaskAsync("Counter/node/" + date.Day.ToString() + date.Month.ToString() + date.Year.ToString());
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
                try
                {
                    if (data.New == null)
                    {

                        if (data.Name != null)
                            if (data.Id == null)
                            {
                                if (pos == 2)
                                {
                                    data.cnt = "0";
                                    FirebaseResponse respKey = await client.GetTaskAsync("Key/");
                                    KeyO getKey = resp.ResultAs<KeyO>();
                                    getKey.key = (Convert.ToDouble(getKey.key) + 1).ToString();
                                    data.Key = getKey.key;
                                    await client.SetTaskAsync("Key/", getKey);
                                }
                                Free_Place();
                                if (free.Count != 0)
                                {
                                    data.Id = free.ElementAt(0).ToString();
                                    await client.SetTaskAsync(path + date.Day.ToString() + date.Month.ToString() + date.Year.ToString() + free.ElementAt(0).ToString(), data);
                                    free.RemoveAt(0);
                                }
                                else
                                {
                                    data.Id = value.ToString();
                                    await client.SetTaskAsync(path + date.Day.ToString() + date.Month.ToString() + date.Year.ToString() + value.ToString(), data);
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
                                    await client.SetTaskAsync("Counter/node/" + date.Day.ToString() + date.Month.ToString() + date.Year.ToString(), obj);
                                }
                            }
                            else
                            {
                                await client.SetTaskAsync(path + date.Day.ToString() + date.Month.ToString() + date.Year.ToString() + data.Id, data);
                            }
                    }
                    else
                    {
                        try
                        {
                            switch (pos)
                            {
                                case 0:
                                    {
                                        dataGrid.IsEnabled = false;
                                        break;
                                    }
                                case 1:
                                    {
                                        dataGrid1.IsEnabled = false;
                                        break;
                                    }
                                case 2:
                                    {
                                        dataGrid2.IsEnabled = false;
                                        break;
                                    }
                            }

                            if (data.Name != null)
                            {
                                int i = list.IndexOf(data);
                                double all, now, newV;
                                if (data.All != null && data.All != " " && data.All != "")
                                    all = Convert.ToDouble(data.All);
                                else
                                    all = 0;

                                if (data.Now != null && data.Now != " " && data.Now != "")
                                    now = Convert.ToDouble(data.Now);
                                else
                                    now = 0;
                                newV = Convert.ToDouble(data.New);

                                if (pos == 2)
                                {
                                    listI.Clear();
                                    listR.Clear();

                                    FirebaseResponse res = await client.GetTaskAsync("Counter/node/" + date.Day.ToString() + date.Month.ToString() + date.Year.ToString());
                                    get = res.ResultAs<Counter>();

                                    int count = Convert.ToInt32(get.cnt);
                                    if (count != 0)
                                        for (int j = 1; j <= count; j++)
                                        {
                                            try
                                            {
                                                FirebaseResponse response = await client.GetTaskAsync("Category/" + date.Day.ToString() + date.Month.ToString() + date.Year.ToString() + j);
                                                Data data1 = response.ResultAs<Data>();
                                                listR.Add(data1);
                                            }
                                            catch { }
                                        }
                                    FirebaseResponse resp1 = await client.GetTaskAsync("Out/" + date.Day.ToString() + date.Month.ToString() + date.Year.ToString() + data.Id);
                                    ItemsCount getC = resp1.ResultAs<ItemsCount>();

                                    count = Convert.ToInt32(getC.cnt);
                                    if (count != 0)
                                    {
                                        for (int j = 1; j <= count; j++)
                                        {
                                            try
                                            {
                                                FirebaseResponse response = await client.GetTaskAsync("Out/" + data.Key + "/" + j);
                                                Item item = response.ResultAs<Item>();
                                                listI.Add(item);
                                            }
                                            catch { }
                                        }
                                    }
                                    foreach (Data obj in listR)
                                    {
                                        foreach (Item item in listI)
                                        {
                                            if (item.Name == obj.Name)
                                            {
                                                obj.Now = (
                                                    Convert.ToDouble(obj.Now) - Convert.ToDouble(item.Now) * newV
                                                    ).ToString();
                                                await client.SetTaskAsync("Category/" + date.Day.ToString() + date.Month.ToString() + date.Year.ToString() + obj.Id, obj);
                                                break;
                                            }
                                        }
                                    }
                                }
                                data.All = (all + newV).ToString();
                                data.Now = (now + newV).ToString();
                                data.New = null;
                                await client.SetTaskAsync(path + date.Day.ToString() + date.Month.ToString() + date.Year.ToString() + data.Id, data);
                            }
                            else
                            {
                                data.Old = null;
                                data.All = null;
                                data.Now = null;
                                data.New = null;
                                data.Note = null;
                            }
                            switch (pos)
                            {
                                case 0:
                                    {
                                        dataGrid.Items.Refresh();
                                        dataGrid.IsEnabled = true;
                                        break;
                                    }
                                case 1:
                                    {
                                        dataGrid1.Items.Refresh();
                                        dataGrid1.IsEnabled = true;
                                        break;
                                    }
                                case 2:
                                    {
                                        dataGrid2.Items.Refresh();
                                        dataGrid2.IsEnabled = true;
                                        break;
                                    }
                            }
                        }
                        catch
                        {
                            MessageBox.Show("Некорректность вводимых или хранимых данных.");
                        }
                    }
                }
                catch { }
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
                    switch (pos)
                    {
                        case 0:
                            {
                                dataGrid.CancelEdit(DataGridEditingUnit.Row);
                                break;
                            }
                        case 1:
                            {
                                dataGrid1.CancelEdit(DataGridEditingUnit.Row);
                                break;
                            }
                        case 2:
                            {
                                dataGrid2.CancelEdit(DataGridEditingUnit.Row);
                                break;
                            }
                    }
                    pos = tab.SelectedIndex;
                    Load();
                }
            }
            catch { }
        }

        private async void Set_Items(object sender, RoutedEventArgs e)
        {
            try
            {
                Data data = dataGrid2.SelectedItem as Data;
                SetItems setItems = new SetItems(data.Name, data.Id, data.Key);

                if (setItems.ShowDialog() == true)
                {
                    var items = new ItemsCount
                    {
                        cnt = setItems.ItemsList.Count.ToString(),
                    };

                    await client.SetTaskAsync("Out/" + date.Day.ToString() + date.Month.ToString() + date.Year.ToString() + data.Id, items);
                    await client.DeleteTaskAsync("Out/" + data.Key);

                    for (int i = 0; i < setItems.ItemsList.Count; i++)
                    {
                        Data obj = setItems.ItemsList.ElementAt(i);
                        obj.Id = null;
                        obj.New = null;
                        obj.All = null;
                        obj.Old = null;
                        obj.Key = null;
                        await client.SetTaskAsync("Out/" + data.Key + "/" + (i + 1), obj);
                    }
                }
            }
            catch { }
        }
    }
}
