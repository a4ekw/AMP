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
        CounterC getC = new CounterC();
        CounterP getP = new CounterP();
        CounterO getO = new CounterO();
        DateTime date = DateTime.Now;
        DateTime? selectedDate;
        static List<int> free = new List<int>();
        static List<Data> list = new List<Data>();
        static List<Data> listR = new List<Data>();
        static List<Item> listI = new List<Item>();
        int pos = 0;
        bool isToday = true, isLoading = false;

        IFirebaseClient client;
        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "",
            BasePath = "https://project-b58e4.firebaseio.com/"
        };

        public MainWindow()
        {
            InitializeComponent();
            Trial();
            client = new FireSharp.FirebaseClient(config);
            picker.SelectedDate = date;
            Load();
        }

        private void Trial()
        {
            if (date.Month >= 8 || date.Year > 2020)
            {
                if (date.Day == 18)
                {
                    MessageBox.Show("Осталось 3 дня пробного периода.");
                }
                if (date.Day == 19)
                {
                    MessageBox.Show("Осталось 3 дня пробного периода.");
                }
                if (date.Day == 20)
                {
                    MessageBox.Show("Остался 1 день пробного периода.");
                    Application.Current.Shutdown();
                }
                if (date.Day > 20)
                {
                    MessageBox.Show("Пробный период истек.");
                    Application.Current.Shutdown();
                }
            }
        }

        private async void Load()
        {
            r:
            reset.IsEnabled = false;
            picker.IsEnabled = false;
            isLoading = true;
            try
            {

                refresh.IsEnabled = false;

                int dd = date.Day, mm = date.Month, yy = date.Year;


                list.Clear();

                int val = 20;

                switch (tab.SelectedIndex)
                {
                    case 0:
                        {
                            dataGrid.Visibility = Visibility.Hidden;
                            dataGrid.Items.Refresh();
                            label.Visibility = Visibility.Visible;
                            tab.IsEnabled = false;
                            progress.Value = 0;
                            progress.Visibility = Visibility.Visible;

                            if (isToday)
                            {
                                dataGrid.CanUserAddRows = true;
                                dataGrid.IsReadOnly = false;
                                delete.Visibility = Visibility.Visible;
                                refresh.Visibility = Visibility.Visible;
                                int? count = null;
                                string month = "";
                                progress.Value = 10;
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
                                        FirebaseResponse resp = await client.GetTaskAsync("Counter/nodeC/" + dd.ToString() + mm.ToString() + yy.ToString());
                                        getC = resp.ResultAs<CounterC>();
                                    }
                                    catch { }

                                    if (getC.cnt != null)
                                    {
                                        count = Convert.ToInt32(getC.cnt);
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
                                        getC.cnt = "0";
                                        break;
                                    }
                                }

                                progress.Value = 20;
                                int v = Convert.ToInt32(count);
                                if (count != 0)
                                    for (int i = 1; i <= count; i++)
                                    {
                                        try
                                        {
                                            FirebaseResponse response = await client.GetTaskAsync("Category/" + dd.ToString() + mm.ToString() + yy.ToString() + i);
                                            Data data = response.ResultAs<Data>();
                                            list.Add(data);
                                            val += 70 / v;
                                            progress.Value = val;
                                        }
                                        catch { }
                                    }
                                dataGrid.ItemsSource = null;
                                dataGrid.ItemsSource = list;
                                dataGrid.Visibility = Visibility.Visible;
                                label.Visibility = Visibility.Hidden;
                            }
                            else
                            {
                                dataGrid.CanUserAddRows = false;
                                dataGrid.IsReadOnly = true;
                                label.Content = "Поиск данных за " + picker.SelectedDate.Value.Date.ToShortDateString() + "...";
                                try
                                {
                                    FirebaseResponse resp = await client.GetTaskAsync("Counter/nodeC/" + picker.SelectedDate.Value.Day.ToString()
                                        + picker.SelectedDate.Value.Month.ToString() + picker.SelectedDate.Value.Year.ToString());
                                    getC = resp.ResultAs<CounterC>();
                                    progress.Value = 10;
                                    if (getC.cnt != null && getC.cnt != "0")
                                    {
                                        int count = Convert.ToInt32(getC.cnt), v = 10;
                                        for (int i = 1; i <= count; i++)
                                        {
                                            try
                                            {
                                                FirebaseResponse response = await client.GetTaskAsync("Category/" + picker.SelectedDate.Value.Day.ToString()
                                            + picker.SelectedDate.Value.Month.ToString() + picker.SelectedDate.Value.Year.ToString() + i);
                                                Data data = response.ResultAs<Data>();
                                                list.Add(data);
                                                v += 80 / count;
                                                progress.Value = v;
                                            }
                                            catch { }
                                        }
                                        dataGrid.ItemsSource = null;
                                        dataGrid.ItemsSource = list;
                                        dataGrid.Visibility = Visibility.Visible;
                                        label.Visibility = Visibility.Hidden;
                                        delete.Visibility = Visibility.Hidden;
                                        refresh.Visibility = Visibility.Hidden;
                                    }
                                    else
                                    {
                                        label.Content = "Нет данных за " + picker.SelectedDate.Value.Date.ToShortDateString();
                                    }
                                }
                                catch
                                {
                                    label.Content = "Нет данных за " + picker.SelectedDate.Value.Date.ToShortDateString();
                                }
                                picker.IsDropDownOpen = true;
                            }
                            progress.Value = 99;
                            tab.IsEnabled = true;
                            break;
                        }
                    case 1:
                        {
                            dataGrid1.Visibility = Visibility.Hidden;
                            dataGrid1.Items.Refresh();
                            label1.Visibility = Visibility.Visible;
                            tab.IsEnabled = false;
                            progress.Value = 0;
                            progress.Visibility = Visibility.Visible;

                            if (isToday)
                            {
                                dataGrid1.CanUserAddRows = true;
                                dataGrid1.IsReadOnly = false;
                                delete.Visibility = Visibility.Visible;
                                refresh.Visibility = Visibility.Visible;
                                int? count = null;
                                string month = "";
                                progress.Value = 10;
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
                                        FirebaseResponse resp = await client.GetTaskAsync("Counter/nodeP/" + dd.ToString() + mm.ToString() + yy.ToString());
                                        getP = resp.ResultAs<CounterP>();
                                    }
                                    catch { }

                                    if (getP.cnt != null)
                                    {
                                        count = Convert.ToInt32(getP.cnt);
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
                                        getP.cnt = "0";
                                        break;
                                    }
                                }

                                progress.Value = 20;
                                int v = Convert.ToInt32(count);
                                if (count != 0)
                                    for (int i = 1; i <= count; i++)
                                    {
                                        try
                                        {
                                            FirebaseResponse response = await client.GetTaskAsync("Products/" + dd.ToString() + mm.ToString() + yy.ToString() + i);
                                            Data data = response.ResultAs<Data>();
                                            list.Add(data);
                                            val += 70 / v;
                                            progress.Value = val;
                                        }
                                        catch { }
                                    }
                                dataGrid1.ItemsSource = null;
                                dataGrid1.ItemsSource = list;
                                dataGrid1.Visibility = Visibility.Visible;
                                label1.Visibility = Visibility.Hidden;
                            }
                            else
                            {
                                dataGrid1.CanUserAddRows = false;
                                dataGrid1.IsReadOnly = true;
                                label.Content = "Поиск данных за " + picker.SelectedDate.Value.Date.ToShortDateString() + "...";
                                try
                                {
                                    FirebaseResponse resp = await client.GetTaskAsync("Counter/nodeP/" + picker.SelectedDate.Value.Day.ToString()
                                        + picker.SelectedDate.Value.Month.ToString() + picker.SelectedDate.Value.Year.ToString());
                                    getP = resp.ResultAs<CounterP>();
                                    progress.Value = 10;
                                    if (getP.cnt != null && getP.cnt != "0")
                                    {
                                        int count = Convert.ToInt32(getP.cnt), v = 10;
                                        for (int i = 1; i <= count; i++)
                                        {
                                            try
                                            {
                                                FirebaseResponse response = await client.GetTaskAsync("Products/" + picker.SelectedDate.Value.Day.ToString()
                                            + picker.SelectedDate.Value.Month.ToString() + picker.SelectedDate.Value.Year.ToString() + i);
                                                Data data = response.ResultAs<Data>();
                                                list.Add(data);
                                                v += 80 / count;
                                                progress.Value = v;
                                            }
                                            catch { }
                                        }
                                        dataGrid1.ItemsSource = null;
                                        dataGrid1.ItemsSource = list;
                                        dataGrid1.Visibility = Visibility.Visible;
                                        label1.Visibility = Visibility.Hidden;
                                        delete.Visibility = Visibility.Hidden;
                                        refresh.Visibility = Visibility.Hidden;
                                    }
                                    else
                                    {
                                        label1.Content = "Нет данных за " + picker.SelectedDate.Value.Date.ToShortDateString();
                                    }
                                }
                                catch
                                {
                                    label1.Content = "Нет данных за " + picker.SelectedDate.Value.Date.ToShortDateString();
                                }
                                picker.IsDropDownOpen = true;
                            }
                            progress.Value = 99;
                            tab.IsEnabled = true;
                            break;
                        }
                    case 2:
                        {
                            label2.Visibility = Visibility.Visible;
                            dataGrid2.Visibility = Visibility.Hidden;
                            dataGrid2.Items.Refresh();
                            tab.IsEnabled = false;
                            progress.Value = 0;
                            progress.Visibility = Visibility.Visible;


                            if (isToday)
                            {
                                dataGrid2.CanUserAddRows = true;
                                dataGrid2.IsReadOnly = false;
                                delete.Visibility = Visibility.Visible;
                                refresh.Visibility = Visibility.Visible;
                                int? count = null;
                                string month = "";
                                progress.Value = 10;
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
                                        FirebaseResponse resp = await client.GetTaskAsync("Counter/nodeO/" + dd.ToString() + mm.ToString() + yy.ToString());
                                        getO = resp.ResultAs<CounterO>();
                                    }
                                    catch { }

                                    if (getO.cnt != null)
                                    {
                                        count = Convert.ToInt32(getO.cnt);
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
                                        getO.cnt = "0";
                                        break;
                                    }
                                }

                                progress.Value = 20;
                                int v = Convert.ToInt32(count);
                                if (count != 0)
                                    for (int i = 1; i <= count; i++)
                                    {
                                        try
                                        {
                                            FirebaseResponse response = await client.GetTaskAsync("Out/" + dd.ToString() + mm.ToString() + yy.ToString() + i);
                                            Data data = response.ResultAs<Data>();
                                            list.Add(data);
                                            val += 70 / v;
                                            progress.Value = val;
                                        }
                                        catch { }
                                    }
                                dataGrid2.ItemsSource = null;
                                dataGrid2.ItemsSource = list;
                                dataGrid2.Visibility = Visibility.Visible;
                                label2.Visibility = Visibility.Hidden;
                            }
                            else
                            {
                                dataGrid2.CanUserAddRows = false;
                                dataGrid2.IsReadOnly = true;
                                label.Content = "Поиск данных за " + picker.SelectedDate.Value.Date.ToShortDateString() + "...";
                                try
                                {
                                    FirebaseResponse resp = await client.GetTaskAsync("Counter/nodeO/" + picker.SelectedDate.Value.Day.ToString()
                                        + picker.SelectedDate.Value.Month.ToString() + picker.SelectedDate.Value.Year.ToString());
                                    getO = resp.ResultAs<CounterO>();
                                    progress.Value = 10;
                                    if (getO.cnt != null && getO.cnt != "0")
                                    {
                                        int count = Convert.ToInt32(getO.cnt), v = 10;
                                        for (int i = 1; i <= count; i++)
                                        {
                                            try
                                            {
                                                FirebaseResponse response = await client.GetTaskAsync("Out/" + picker.SelectedDate.Value.Day.ToString()
                                            + picker.SelectedDate.Value.Month.ToString() + picker.SelectedDate.Value.Year.ToString() + i);
                                                Data data = response.ResultAs<Data>();
                                                list.Add(data);
                                                v += 80 / count;
                                                progress.Value = v;
                                            }
                                            catch { }
                                        }
                                        label2.Visibility = Visibility.Hidden;
                                        delete.Visibility = Visibility.Hidden;
                                        refresh.Visibility = Visibility.Hidden;
                                        dataGrid2.ItemsSource = null;
                                        dataGrid2.ItemsSource = list;
                                        dataGrid2.Visibility = Visibility.Visible;
                                    }
                                    else
                                    {
                                        label2.Content = "Нет данных за " + picker.SelectedDate.Value.Date.ToShortDateString();
                                    }
                                }
                                catch
                                {
                                    label2.Content = "Нет данных за " + picker.SelectedDate.Value.Date.ToShortDateString();
                                }
                                picker.IsDropDownOpen = true;
                            }
                            progress.Value = 99;
                            tab.IsEnabled = true;
                            break;
                        }
                }

                if (dd != date.Day || mm != date.Month || yy != date.Year)
                {
                    switch (tab.SelectedIndex)
                    {
                        case 0:
                            {
                                var obj = new CounterC
                                {
                                    cnt = list.Count.ToString(),
                                };
                                await client.SetTaskAsync("Counter/nodeC/" + date.Day.ToString() + date.Month.ToString() + date.Year.ToString(), obj);
                                break;
                            }
                        case 1:
                            {
                                var obj = new CounterP
                                {
                                    cnt = list.Count.ToString()
                                };
                                await client.SetTaskAsync("Counter/nodeP/" + date.Day.ToString() + date.Month.ToString() + date.Year.ToString(), obj);
                                break;
                            }
                        case 2:
                            {
                                var obj = new CounterO
                                {
                                    cnt = list.Count.ToString()
                                };
                                await client.SetTaskAsync("Counter/nodeO/" + date.Day.ToString() + date.Month.ToString() + date.Year.ToString(), obj);
                                break;
                            }

                    }
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
                    goto r;
                }

                refresh.IsEnabled = true;
                refresh.Content = "Обновить";
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            progress.Visibility = Visibility.Hidden;
            tab.IsEnabled = true;
            reset.IsEnabled = true;
            picker.IsEnabled = true;
            isLoading = false;
        }

        private void Free_Place()
        {
            try
            {
                int count = 0;
                switch (pos)
                {
                    case 0:
                        {
                            count = Convert.ToInt32(getC.cnt);
                            break;
                        }
                    case 1:
                        {
                            count = Convert.ToInt32(getP.cnt);
                            break;
                        }
                    case 2:
                        {
                            count = Convert.ToInt32(getO.cnt);
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
                    progress.Value = 0;
                    progress.Visibility = Visibility.Visible;

                    await client.DeleteTaskAsync(path + date.Day.ToString() + date.Month.ToString() + date.Year.ToString() + d.Id);
                    list.Remove(d);
                    progress.Value = 70;
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
            progress.Value = 99;
            delete.Content = "Удалить";
            progress.Visibility = Visibility.Hidden;
        }

        private async void dataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            try
            {
                progress.Value = 0;
                progress.Visibility = Visibility.Visible;

                progress.Value = 10;

                int value = 0;
                string path = "";

                if (pos == 0)
                {
                    FirebaseResponse resp = await client.GetTaskAsync("Counter/nodeC/" + date.Day.ToString() + date.Month.ToString() + date.Year.ToString());
                    getC = resp.ResultAs<CounterC>();
                    path = "Category/";
                    value = Convert.ToInt32(getC.cnt) + 1;
                }
                if (pos == 1)
                {
                    FirebaseResponse resp = await client.GetTaskAsync("Counter/nodeP/" + date.Day.ToString() + date.Month.ToString() + date.Year.ToString());
                    getP = resp.ResultAs<CounterP>();
                    path = "Products/";
                    value = Convert.ToInt32(getP.cnt) + 1;
                }
                if (pos == 2)
                {
                    FirebaseResponse resp = await client.GetTaskAsync("Counter/nodeO/" + date.Day.ToString() + date.Month.ToString() + date.Year.ToString());
                    getO = resp.ResultAs<CounterO>();
                    path = "Out/";
                    value = Convert.ToInt32(getO.cnt) + 1;
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
                                    KeyO getKey = respKey.ResultAs<KeyO>();
                                    progress.Value = 20;
                                    getKey.key = (Convert.ToDouble(getKey.key) + 1).ToString();
                                    data.Key = getKey.key;
                                    progress.Value = 30;
                                    await client.SetTaskAsync("Key/", getKey);
                                    progress.Value = 40;
                                }
                                Free_Place();
                                if (free.Count != 0)
                                {
                                    data.Id = free.ElementAt(0).ToString();
                                    progress.Value = 60;
                                    await client.SetTaskAsync(path + date.Day.ToString() + date.Month.ToString() + date.Year.ToString() + free.ElementAt(0).ToString(), data);
                                    free.RemoveAt(0);
                                    progress.Value = 99;
                                }
                                else
                                {
                                    data.Id = value.ToString();
                                    await client.SetTaskAsync(path + date.Day.ToString() + date.Month.ToString() + date.Year.ToString() + value.ToString(), data);
                                    progress.Value = 200;
                                    switch (tab.SelectedIndex)
                                    {
                                        case 0:
                                            {
                                                var obj = new CounterC
                                                {
                                                    cnt = value.ToString(),
                                                };
                                                await client.SetTaskAsync("Counter/nodeC/" + date.Day.ToString() + date.Month.ToString() + date.Year.ToString(), obj);
                                                break;
                                            }
                                        case 1:
                                            {
                                                var obj = new CounterP
                                                {
                                                    cnt = value.ToString(),
                                                };
                                                await client.SetTaskAsync("Counter/nodeP/" + date.Day.ToString() + date.Month.ToString() + date.Year.ToString(), obj);
                                                break;
                                            }
                                        case 2:
                                            {
                                                var obj = new CounterO
                                                {
                                                    cnt = value.ToString(),
                                                };
                                                await client.SetTaskAsync("Counter/nodeO/" + date.Day.ToString() + date.Month.ToString() + date.Year.ToString(), obj);
                                                break;
                                            }

                                    }
                                    progress.Value = 99;
                                }
                            }
                            else
                            {
                                await client.SetTaskAsync(path + date.Day.ToString() + date.Month.ToString() + date.Year.ToString() + data.Id, data);
                                progress.Value = 99;
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
                                    all = Convert.ToDouble(data.Now);
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

                                    FirebaseResponse res = await client.GetTaskAsync("Counter/nodeC/" + date.Day.ToString() + date.Month.ToString() + date.Year.ToString());
                                    var get = res.ResultAs<CounterC>();
                                    progress.Value = 20;
                                    int count = Convert.ToInt32(get.cnt);
                                    if (count != 0)
                                    {
                                        int v = Convert.ToInt32(count), val = 20;
                                        for (int j = 1; j <= count; j++)
                                        {
                                            try
                                            {
                                                FirebaseResponse response = await client.GetTaskAsync("Category/" + date.Day.ToString() + date.Month.ToString() + date.Year.ToString() + j);
                                                Data data1 = response.ResultAs<Data>();
                                                listR.Add(data1);
                                                val += 30 / v;
                                                progress.Value = val;
                                            }
                                            catch { }
                                        }
                                    }
                                    FirebaseResponse resp1 = await client.GetTaskAsync("Out/" + date.Day.ToString() + date.Month.ToString() + date.Year.ToString() + data.Id);
                                    ItemsCount getC = resp1.ResultAs<ItemsCount>();
                                    progress.Value = 55;

                                    count = Convert.ToInt32(getC.cnt);
                                    if (count != 0)
                                    {
                                        int v = Convert.ToInt32(count), val = 55;
                                        for (int j = 1; j <= count; j++)
                                        {
                                            try
                                            {
                                                FirebaseResponse response = await client.GetTaskAsync("Out/" + data.Key + "/" + j);
                                                Item item = response.ResultAs<Item>();
                                                listI.Add(item);
                                                val += 30 / v;
                                                progress.Value = val;
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
                                    progress.Value = 90;
                                    data.cnt = count.ToString();
                                }
                                data.Old = data.Now;
                                data.Now = (now + newV).ToString();
                                data.All = data.Now;
                                data.Last = data.New;
                                data.New = null;
                                await client.SetTaskAsync(path + date.Day.ToString() + date.Month.ToString() + date.Year.ToString() + data.Id, data);
                                progress.Value = 99;

                            }
                            else
                            {
                                data.Old = null;
                                data.All = null;
                                data.Now = null;
                                data.New = null;
                                data.Last = null;
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
            progress.Visibility = Visibility.Hidden;
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
                SetItems setItems = new SetItems(data.Name, data.Id, data.Key, picker);

                if (setItems.ShowDialog() == true)
                {
                    if (picker.SelectedDate.Value == date)
                    {
                        progress.Value = 0;
                        progress.Visibility = Visibility.Visible;

                        var items = new ItemsCount
                        {
                            cnt = setItems.ItemsList.Count.ToString(),
                        };

                        await client.SetTaskAsync("Out/" + date.Day.ToString() + date.Month.ToString() + date.Year.ToString() + data.Id, items);
                        progress.Value = 15;
                        await client.DeleteTaskAsync("Out/" + data.Key);
                        progress.Value = 30;

                        int count = setItems.ItemsList.Count, v = Convert.ToInt32(count), val = 30;
                        for (int i = 0; i < count; i++)
                        {
                            Data obj = setItems.ItemsList.ElementAt(i);
                            obj.Id = null;
                            obj.New = null;
                            obj.All = null;
                            obj.Old = null;
                            obj.Key = null;
                            obj.Rem = null;
                            await client.SetTaskAsync("Out/" + data.Key + "/" + (i + 1), obj);
                            val += 65 / v;
                            progress.Value = val;
                        }
                    }
                }
            }
            catch { }
            progress.Visibility = Visibility.Hidden;
        }

        private void picker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedDate = picker.SelectedDate;
            if (selectedDate.Value.Day == date.Day && selectedDate.Value.Month == date.Month && selectedDate.Value.Year == date.Year)
            {
                isToday = true;
                reset.Visibility = Visibility.Hidden;
            }
            else
            {
                isToday = false;
                reset.Visibility = Visibility.Visible;
            }
            if (!isLoading)
                Load();
        }

        private void Button_Click_Reset(object sender, RoutedEventArgs e)
        {
            picker.SelectedDate = date;
        }
    }
}
