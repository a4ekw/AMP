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
        Data dataS;
        DateTime date;
        DateTime? selectedDate;
        CounterC getC = new CounterC();
        CounterP getP = new CounterP();
        CounterO getO = new CounterO();
        static List<int> free = new List<int>();
        static List<Data> list = new List<Data>();
        static List<Data> list1 = new List<Data>();
        static List<Data> list2 = new List<Data>();
        static List<Data> listR = new List<Data>();
        static List<Item> listI = new List<Item>();
        int pos = 0, v, lR;
        int? count = null, value;
        double all, now, newV;
        bool isToday = true, isLoading = false;

        IFirebaseClient client;
        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "q1oz56hqzen6Qlx8zp4gbMH5EgGCsF6AkY50ZKHc",
            BasePath = "https://project-b58e4.firebaseio.com/"
        };

        public MainWindow()
        {
            InitializeComponent();
            client = new FireSharp.FirebaseClient(config);
            date = DateTime.Now;
            picker.SelectedDate = date;
        }

        private async void Load()
        {
            date = DateTime.Now;
            isLoading = true;
            refresh.IsEnabled = false;
            reset.IsEnabled = false;
            picker.IsEnabled = false;
            try
            {
                int dd = date.Day, mm = date.Month, yy = date.Year;

                switch (tab.SelectedIndex)
                {
                    case 0:
                        {
                            list.Clear();
                            dataGrid.Visibility = Visibility.Hidden;
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
                                string month = "";
                                progress.Value = 10;
                                count = null;
                                getC = new CounterC();
                                getC.cnt = null;
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

                                string dds = dd.ToString(), mms = mm.ToString(), yys = yy.ToString();
                                progress.Value = 20;
                                v = Convert.ToInt32(count);

                                value = count;
                                if (count != 0)
                                    for (int i = 1; i <= count; i++)
                                    {
                                        try
                                        {
                                            Load_List("Category/", dds, mms, yys, i);
                                        }
                                        catch { }
                                    }
                            }
                            else
                            {
                                dataGrid.CanUserAddRows = false;
                                dataGrid.IsReadOnly = true;
                                delete.Visibility = Visibility.Hidden;
                                refresh.Visibility = Visibility.Hidden;
                                label.Content = "Поиск данных за " + picker.SelectedDate.Value.Date.ToShortDateString() + "...";
                                try
                                {
                                    string dds = picker.SelectedDate.Value.Day.ToString(),
                                        mms = picker.SelectedDate.Value.Month.ToString(),
                                        yys = picker.SelectedDate.Value.Year.ToString();
                                    FirebaseResponse resp = await client.GetTaskAsync("Counter/nodeC/" + dds + mms + yys);
                                    getC = resp.ResultAs<CounterC>();
                                    progress.Value = 10;
                                    if (getC.cnt != null && getC.cnt != "0")
                                    {
                                        v = Convert.ToInt32(getC.cnt);
                                        count = v;
                                        value = count;
                                        for (int i = 1; i <= count; i++)
                                        {
                                            try
                                            {
                                                Load_List("Category/", dds, mms, yys, i);
                                            }
                                            catch { }
                                        }
                                    }
                                    else
                                    {
                                        tab.IsEnabled = true;
                                        reset.IsEnabled = true;
                                        picker.IsEnabled = true;
                                        progress.Visibility = Visibility.Hidden;
                                        label.Content = "Нет данных за " + picker.SelectedDate.Value.Date.ToShortDateString();
                                        isLoading = false;
                                    }
                                }
                                catch
                                {
                                    tab.IsEnabled = true;
                                    reset.IsEnabled = true;
                                    picker.IsEnabled = true;
                                    progress.Visibility = Visibility.Hidden;
                                    label.Content = "Нет данных за " + picker.SelectedDate.Value.Date.ToShortDateString();
                                    isLoading = false;
                                }
                                picker.IsDropDownOpen = true;
                            }
                            break;
                        }
                    case 1:
                        {
                            list1.Clear();
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
                                count = null;
                                getP.cnt = null;
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

                                string dds = dd.ToString(), mms = mm.ToString(), yys = yy.ToString();
                                progress.Value = 20;
                                v = Convert.ToInt32(count);

                                value = count;
                                if (count != 0)
                                    for (int i = 1; i <= count; i++)
                                    {
                                        try
                                        {
                                            Load_List("Products/", dds, mms, yys, i);
                                        }
                                        catch { }
                                    }
                            }
                            else
                            {
                                dataGrid1.CanUserAddRows = false;
                                dataGrid1.IsReadOnly = true;
                                delete.Visibility = Visibility.Hidden;
                                refresh.Visibility = Visibility.Hidden;
                                label.Content = "Поиск данных за " + picker.SelectedDate.Value.Date.ToShortDateString() + "...";
                                try
                                {
                                    string dds = picker.SelectedDate.Value.Day.ToString(),
                                        mms = picker.SelectedDate.Value.Month.ToString(),
                                        yys = picker.SelectedDate.Value.Year.ToString();
                                    FirebaseResponse resp = await client.GetTaskAsync("Counter/nodeP/" + dds + mms + yys);
                                    getP = resp.ResultAs<CounterP>();
                                    progress.Value = 10;
                                    if (getP.cnt != null && getP.cnt != "0")
                                    {
                                        v = Convert.ToInt32(getP.cnt);
                                        count = v;
                                        value = count;
                                        for (int i = 1; i <= count; i++)
                                        {
                                            try
                                            {
                                                Load_List("Products/", dds, mms, yys, i);
                                            }
                                            catch { }
                                        }
                                    }
                                    else
                                    {
                                        tab.IsEnabled = true;
                                        reset.IsEnabled = true;
                                        picker.IsEnabled = true;
                                        progress.Visibility = Visibility.Hidden;
                                        label1.Content = "Нет данных за " + picker.SelectedDate.Value.Date.ToShortDateString();
                                        isLoading = false;
                                    }
                                }
                                catch
                                {
                                    tab.IsEnabled = true;
                                    reset.IsEnabled = true;
                                    picker.IsEnabled = true;
                                    progress.Visibility = Visibility.Hidden;
                                    label1.Content = "Нет данных за " + picker.SelectedDate.Value.Date.ToShortDateString();
                                    isLoading = false;
                                }
                                picker.IsDropDownOpen = true;
                            }
                            break;
                        }
                    case 2:
                        {
                            list2.Clear();
                            dataGrid2.Visibility = Visibility.Hidden;
                            dataGrid2.Items.Refresh();
                            label2.Visibility = Visibility.Visible;
                            tab.IsEnabled = false;
                            progress.Value = 0;
                            progress.Visibility = Visibility.Visible;

                            if (isToday)
                            {
                                dataGrid2.CanUserAddRows = true;
                                dataGrid2.IsReadOnly = false;
                                delete.Visibility = Visibility.Visible;
                                refresh.Visibility = Visibility.Visible;
                                count = null;
                                getO.cnt = null;
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

                                string dds = dd.ToString(), mms = mm.ToString(), yys = yy.ToString();
                                progress.Value = 20;
                                v = Convert.ToInt32(count);

                                value = count;
                                if (count != 0)
                                    for (int i = 1; i <= count; i++)
                                    {
                                        try
                                        {
                                            Load_List("Out/", dds, mms, yys, i);
                                        }
                                        catch { }
                                    }
                            }
                            else
                            {
                                dataGrid2.CanUserAddRows = false;
                                dataGrid2.IsReadOnly = true;
                                delete.Visibility = Visibility.Hidden;
                                refresh.Visibility = Visibility.Hidden;
                                label.Content = "Поиск данных за " + picker.SelectedDate.Value.Date.ToShortDateString() + "...";
                                try
                                {
                                    string dds = picker.SelectedDate.Value.Day.ToString(),
                                        mms = picker.SelectedDate.Value.Month.ToString(),
                                        yys = picker.SelectedDate.Value.Year.ToString();
                                    FirebaseResponse resp = await client.GetTaskAsync("Counter/nodeO/" + dds + mms + yys);
                                    getO = resp.ResultAs<CounterO>();
                                    progress.Value = 10;
                                    if (getO.cnt != null && getO.cnt != "0")
                                    {
                                        v = Convert.ToInt32(getO.cnt);
                                        count = v;
                                        value = count;
                                        for (int i = 1; i <= count; i++)
                                        {
                                            try
                                            {
                                                Load_List("Out/", dds, mms, yys, i);
                                            }
                                            catch { }
                                        }
                                    }
                                    else
                                    {
                                        tab.IsEnabled = true;
                                        reset.IsEnabled = true;
                                        picker.IsEnabled = true;
                                        progress.Visibility = Visibility.Hidden;
                                        label2.Content = "Нет данных за " + picker.SelectedDate.Value.Date.ToShortDateString();
                                        isLoading = false;
                                    }
                                }
                                catch
                                {
                                    tab.IsEnabled = true;
                                    reset.IsEnabled = true;
                                    picker.IsEnabled = true;
                                    progress.Visibility = Visibility.Hidden;
                                    label2.Content = "Нет данных за " + picker.SelectedDate.Value.Date.ToShortDateString();
                                    isLoading = false;
                                }
                                picker.IsDropDownOpen = true;
                            }
                            break;
                        }
                }

            }
            catch (Exception ex) { MessageBox.Show(ex.Message + "(Load)"); }
        }

        private async void Load_List(string path, string dds, string mms, string yys, int i)
        {
            List<Data> listD = new List<Data>();
            switch (tab.SelectedIndex)
            {
                case 0:
                    {
                        listD = list;
                        break;
                    }
                case 1:
                    {
                        listD = list1;
                        break;
                    }
                case 2:
                    {
                        listD = list2;
                        break;
                    }
            }
            try
            {
                FirebaseResponse response = await client.GetTaskAsync(path + dds + mms + yys + i);
                Data datal = response.ResultAs<Data>();
                listD.Add(datal);
            }
            catch
            {
                --value;
            }
            if (v != 0)
                progress.Value += 70 / v;
            if (listD.Count == value)
            {
                switch (tab.SelectedIndex)
                {
                    case 0:
                        {
                            dataGrid.ItemsSource = null;
                            dataGrid.ItemsSource = list;
                            dataGrid.Visibility = Visibility.Visible;
                            label.Visibility = Visibility.Hidden;
                            break;
                        }
                    case 1:
                        {
                            dataGrid1.ItemsSource = null;
                            dataGrid1.ItemsSource = list1;
                            dataGrid1.Visibility = Visibility.Visible;
                            label1.Visibility = Visibility.Hidden;
                            break;
                        }
                    case 2:
                        {
                            dataGrid2.ItemsSource = null;
                            dataGrid2.ItemsSource = list2;
                            dataGrid2.Visibility = Visibility.Visible;
                            label2.Visibility = Visibility.Hidden;
                            break;
                        }
                }
                if (isToday)
                    if (dds != date.Day.ToString() || mms != date.Month.ToString() || yys != date.Year.ToString())
                    {
                        var obj = new CounterC
                        {
                            cnt = listD.Count.ToString(),
                        };
                        switch (tab.SelectedIndex)
                        {
                            case 0:
                                {
                                    await client.SetTaskAsync("Counter/nodeC/" + date.Day.ToString() + date.Month.ToString() + date.Year.ToString(), obj);
                                    break;
                                }
                            case 1:
                                {
                                    await client.SetTaskAsync("Counter/nodeP/" + date.Day.ToString() + date.Month.ToString() + date.Year.ToString(), obj);
                                    break;
                                }
                            case 2:
                                {
                                    await client.SetTaskAsync("Counter/nodeO/" + date.Day.ToString() + date.Month.ToString() + date.Year.ToString(), obj);
                                    break;
                                }

                        }

                        dds = date.Day.ToString(); mms = date.Month.ToString(); yys = date.Year.ToString();
                        for (int j = 0; j < listD.Count; j++)
                        {
                            Upload_List(path, dds, mms, yys, listD.ElementAt(j));
                        }
                    }
                progress.Visibility = Visibility.Hidden;
                tab.IsEnabled = true;
                reset.IsEnabled = true;
                refresh.IsEnabled = true;
                refresh.Content = "Обновить";
                picker.IsEnabled = true;
                isLoading = false;
            }

        }

        private async void Upload_List(string path, string dds, string mms, string yys, Data data)
        {
            try
            {
                List<Data> listU = new List<Data>();
                switch (tab.SelectedIndex)
                {
                    case 0:
                        {
                            listU = list;
                            break;
                        }
                    case 1:
                        {
                            listU = list1;
                            break;
                        }
                    case 2:
                        {
                            listU = list2;
                            break;
                        }
                }
                tab.IsEnabled = false;
                await client.SetTaskAsync(path + dds + mms + yys + data.Id, data);
                tab.IsEnabled = true;
            }
            catch
            { }
        }

        private void Free_Place()
        {
            try
            {
                List<Data> listF = new List<Data>();
                int count = 0;
                switch (tab.SelectedIndex)
                {
                    case 0:
                        {
                            listF = list;
                            count = Convert.ToInt32(getC.cnt);
                            break;
                        }
                    case 1:
                        {
                            listF = list1;
                            count = Convert.ToInt32(getP.cnt);
                            break;
                        }
                    case 2:
                        {
                            listF = list2;
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
                    for (int j = 0; j < listF.Count; j++)
                    {
                        try
                        {
                            if (listF.ElementAt(j).Id == i.ToString())
                            {
                                free.Remove(i);
                                break;
                            }
                        }
                        catch { }
                    }
                }

            }
            catch (Exception ex) { MessageBox.Show(ex.Message + "(Free_Place)"); }
        }

        private void Button_Click_Refresh(object sender, RoutedEventArgs e)
        {
            refresh.Content = "...";
            if (!isLoading)
                Load();
        }

        private async void Button_Click_Delete(object sender, RoutedEventArgs e)
        {
            delete.Content = "Подождите...";
            try
            {
                Data d = new Data();

                string path = "";
                List<Data> listDel = new List<Data>();
                switch (tab.SelectedIndex)
                {
                    case 0:
                        {
                            listDel = list;
                            path = "Category/";
                            d = dataGrid.SelectedItem as Data;
                            break;
                        }
                    case 1:
                        {
                            path = "Products/";
                            listDel = list1;
                            d = dataGrid1.SelectedItem as Data;
                            break;
                        }
                    case 2:
                        {
                            path = "Out/";
                            listDel = list2;
                            d = dataGrid2.SelectedItem as Data;
                            break;
                        }
                }

                DeleteDialog dialog = new DeleteDialog(d.Name);
                if (dialog.ShowDialog() == true)
                {
                    progress.Value = 0;
                    progress.Visibility = Visibility.Visible;

                    await client.DeleteTaskAsync(path + date.Day.ToString() + date.Month.ToString() + date.Year.ToString() + d.Id);
                    listDel.Remove(d);
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
                Data data = e.Row.Item as Data;
                progress.Value = 0;
                progress.Visibility = Visibility.Visible;

                progress.Value = 10;

                try
                {
                    if (data.New == null)
                    {

                        if (data.Name != null)
                            if (data.Id == null)
                            {
                                Free_Place();
                                if (free.Count != 0)
                                {
                                    data.Id = free.ElementAt(0).ToString();
                                    progress.Value = 60;
                                    await client.SetTaskAsync("Category/" + date.Day.ToString() + date.Month.ToString() + date.Year.ToString() + free.ElementAt(0).ToString(), data);
                                    free.RemoveAt(0);
                                    progress.Value = 99;
                                }
                                else
                                {
                                    int value = 0;
                                    FirebaseResponse resp = await client.GetTaskAsync("Counter/nodeC/" + date.Day.ToString() + date.Month.ToString() + date.Year.ToString());
                                    getC = resp.ResultAs<CounterC>();
                                    value = Convert.ToInt32(getC.cnt) + 1;

                                    data.Id = value.ToString();
                                    await client.SetTaskAsync("Category/" + date.Day.ToString() + date.Month.ToString() + date.Year.ToString() + value.ToString(), data);
                                    progress.Value = 200;
                                    var obj = new CounterC
                                    {
                                        cnt = value.ToString(),
                                    };
                                    await client.SetTaskAsync("Counter/nodeC/" + date.Day.ToString() + date.Month.ToString() + date.Year.ToString(), obj);
                                    progress.Value = 99;
                                }
                            }
                            else
                            {
                                await client.SetTaskAsync("Category/" + date.Day.ToString() + date.Month.ToString() + date.Year.ToString() + data.Id, data);
                                progress.Value = 99;
                            }
                    }
                    else
                    {
                        try
                        {
                            dataGrid.IsEnabled = false;

                            if (data.Name != null)
                            {
                                int i = list.IndexOf(data);
                                double all, now, newV;

                                try
                                {
                                    all = Convert.ToDouble(data.Now);
                                }
                                catch
                                {
                                    all = 0;
                                }

                                try
                                {
                                    now = Convert.ToDouble(data.Now);
                                }
                                catch
                                {
                                    now = 0;
                                }
                                try
                                {
                                    newV = Convert.ToDouble(data.New);
                                }
                                catch
                                {
                                    newV = 0;
                                }

                                data.Old = data.Now;
                                data.Now = (now + newV).ToString();
                                data.All = data.Now;
                                data.Last = data.New;
                                data.New = null;
                                await client.SetTaskAsync("Category/" + date.Day.ToString() + date.Month.ToString() + date.Year.ToString() + data.Id, data);
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
                            dataGrid.Items.Refresh();
                            dataGrid.IsEnabled = true;
                        }
                        catch
                        {
                            MessageBox.Show("Некорректность вводимых или хранимых данных.");
                            dataGrid.Items.Refresh();
                            dataGrid.IsEnabled = true;
                        }
                    }
                }
                catch (Exception ex) { MessageBox.Show(ex.Message + "(dataGrid_RowEditEnding1)"); }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message + "(dataGrid_RowEditEnding2)"); }
            progress.Visibility = Visibility.Hidden;
        }

        private async void dataGrid1_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            try
            {
                Data data = e.Row.Item as Data;
                progress.Value = 0;
                progress.Visibility = Visibility.Visible;

                progress.Value = 10;

                try
                {
                    if (data.New == null)
                    {

                        if (data.Name != null)
                            if (data.Id == null)
                            {
                                Free_Place();
                                if (free.Count != 0)
                                {
                                    data.Id = free.ElementAt(0).ToString();
                                    progress.Value = 60;
                                    await client.SetTaskAsync("Products/" + date.Day.ToString() + date.Month.ToString() + date.Year.ToString() + free.ElementAt(0).ToString(), data);
                                    free.RemoveAt(0);
                                    progress.Value = 99;
                                }
                                else
                                {
                                    int value = 0;
                                    FirebaseResponse resp2 = await client.GetTaskAsync("Counter/nodeP/" + date.Day.ToString() + date.Month.ToString() + date.Year.ToString());
                                    getP = resp2.ResultAs<CounterP>();
                                    value = Convert.ToInt32(getP.cnt) + 1;

                                    data.Id = value.ToString();
                                    await client.SetTaskAsync("Products/" + date.Day.ToString() + date.Month.ToString() + date.Year.ToString() + value.ToString(), data);
                                    progress.Value = 200;
                                    var obj = new CounterP
                                    {
                                        cnt = value.ToString(),
                                    };
                                    await client.SetTaskAsync("Counter/nodeP/" + date.Day.ToString() + date.Month.ToString() + date.Year.ToString(), obj);
                                    progress.Value = 99;
                                }
                            }
                            else
                            {
                                await client.SetTaskAsync("Products/" + date.Day.ToString() + date.Month.ToString() + date.Year.ToString() + data.Id, data);
                                progress.Value = 99;
                            }
                    }
                    else
                    {
                        try
                        {
                            dataGrid1.IsEnabled = false;

                            if (data.Name != null)
                            {
                                int i = list1.IndexOf(data);
                                double all, now, newV;

                                try
                                {
                                    all = Convert.ToDouble(data.Now);
                                }
                                catch
                                {
                                    all = 0;
                                }

                                try
                                {
                                    now = Convert.ToDouble(data.Now);
                                }
                                catch
                                {
                                    now = 0;
                                }
                                try
                                {
                                    newV = Convert.ToDouble(data.New);
                                }
                                catch
                                {
                                    newV = 0;
                                }
                                data.Old = data.Now;
                                data.Now = (now + newV).ToString();
                                data.All = data.Now;
                                data.Last = data.New;
                                data.New = null;
                                await client.SetTaskAsync("Products/" + date.Day.ToString() + date.Month.ToString() + date.Year.ToString() + data.Id, data);
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
                            dataGrid1.Items.Refresh();
                            dataGrid1.IsEnabled = true;
                        }
                        catch
                        {
                            MessageBox.Show("Некорректность вводимых или хранимых данных.");
                            dataGrid1.Items.Refresh();
                            dataGrid1.IsEnabled = true;
                        }
                    }
                }
                catch (Exception ex) { MessageBox.Show(ex.Message + "(dataGrid_RowEditEnding1)"); }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message + "(dataGrid_RowEditEnding2)"); }
            progress.Visibility = Visibility.Hidden;
        }

        private async void dataGrid2_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            try
            {
                dataS = e.Row.Item as Data;
                progress.Value = 0;
                progress.Visibility = Visibility.Visible;

                progress.Value = 10;

                try
                {
                    if (dataS.New == null)
                    {

                        if (dataS.Name != null)
                            if (dataS.Id == null)
                            {
                                dataS.cnt = "0";
                                FirebaseResponse respKey = await client.GetTaskAsync("Key/");
                                KeyO getKey = respKey.ResultAs<KeyO>();
                                progress.Value = 20;
                                getKey.key = (Convert.ToDouble(getKey.key) + 1).ToString();
                                dataS.Key = getKey.key;
                                progress.Value = 30;
                                await client.SetTaskAsync("Key/", getKey);
                                progress.Value = 40;

                                Free_Place();
                                if (free.Count != 0)
                                {
                                    dataS.Id = free.ElementAt(0).ToString();
                                    progress.Value = 60;
                                    await client.SetTaskAsync("Out/" + date.Day.ToString() + date.Month.ToString() + date.Year.ToString() + free.ElementAt(0).ToString(), dataS);
                                    free.RemoveAt(0);
                                    progress.Value = 99;
                                }
                                else
                                {
                                    int value = 0;
                                    FirebaseResponse resp3 = await client.GetTaskAsync("Counter/nodeO/" + date.Day.ToString() + date.Month.ToString() + date.Year.ToString());
                                    getO = resp3.ResultAs<CounterO>();
                                    value = Convert.ToInt32(getO.cnt) + 1;

                                    dataS.Id = value.ToString();
                                    await client.SetTaskAsync("Out/" + date.Day.ToString() + date.Month.ToString() + date.Year.ToString() + value.ToString(), dataS);
                                    progress.Value = 200;
                                    var obj = new CounterO
                                    {
                                        cnt = value.ToString(),
                                    };
                                    await client.SetTaskAsync("Counter/nodeO/" + date.Day.ToString() + date.Month.ToString() + date.Year.ToString(), obj);
                                    progress.Value = 99;
                                }
                            }
                            else
                            {
                                await client.SetTaskAsync("Out/" + date.Day.ToString() + date.Month.ToString() + date.Year.ToString() + dataS.Id, dataS);
                                progress.Value = 99;
                            }
                    }
                    else
                    {
                        try
                        {
                            dataGrid2.IsEnabled = false;

                            if (dataS.Name != null)
                            {
                                try
                                {
                                    now = Convert.ToDouble(dataS.Now);
                                }
                                catch
                                {
                                    now = 0;
                                }
                                try
                                {
                                    newV = Convert.ToDouble(dataS.New);
                                }
                                catch
                                {
                                    newV = 0;
                                }
                                listI.Clear();

                                FirebaseResponse resp1 = await client.GetTaskAsync("Out/" + date.Day.ToString() + date.Month.ToString() + date.Year.ToString() + dataS.Id);
                                ItemsCount getC = resp1.ResultAs<ItemsCount>();
                                progress.Value = 5;

                                count = Convert.ToInt32(getC.cnt);
                                if (count != 0)
                                {
                                    int v = Convert.ToInt32(count);
                                    for (int j = 1; j <= count; j++)
                                    {
                                        try
                                        {
                                            FirebaseResponse response = await client.GetTaskAsync("Out/" + dataS.Key + "/" + j);
                                            Item item = response.ResultAs<Item>();
                                            listI.Add(item);
                                            progress.Value += 30 / v;
                                        }
                                        catch { }
                                    }

                                }
                                LoadS();
                            }
                            else
                            {
                                dataS.Old = null;
                                dataS.All = null;
                                dataS.Now = null;
                                dataS.New = null;
                                dataS.Last = null;
                                dataS.Note = null;
                            }
                        }
                        catch
                        {
                            MessageBox.Show("Некорректность вводимых или хранимых данных.");
                            dataGrid2.Items.Refresh();
                            dataGrid2.IsEnabled = true;
                        }
                    }
                }
                catch (Exception ex) { MessageBox.Show(ex.Message + "(dataGrid_RowEditEnding1)"); }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message + "(dataGrid_RowEditEnding2)"); }
            progress.Visibility = Visibility.Hidden;
        }

        private async void LoadS()
        {
            try
            {
                progress.Visibility = Visibility.Visible;

                FirebaseResponse resp = await client.GetTaskAsync("Counter/nodeC/" + date.Day.ToString() + date.Month.ToString() + date.Year.ToString());
                CounterC get = resp.ResultAs<CounterC>();
                listR.Clear();
                progress.Value = 35;
                int cnt = Convert.ToInt32(get.cnt);
                if (cnt != 0)
                {
                    lR = Convert.ToInt32(cnt);
                    for (int i = 1; i <= cnt; i++)
                    {
                        try
                        {
                            Load_S(i, cnt);
                        }
                        catch
                        { }
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private async void Load_S(int i, int count)
        {
            try
            {
                progress.Visibility = Visibility.Visible;
                FirebaseResponse response = await client.GetTaskAsync("Category/" + date.Day.ToString() + date.Month.ToString() + date.Year.ToString() + i);
                Data data = response.ResultAs<Data>();
                listR.Add(data);
            }
            catch
            {
                lR -= 1;
            }
            progress.Value += 35 / count;
            if (listR.Count == lR)
                LoadI();
        }

        private async void LoadI()
        {
            foreach (Data obj in listR)
            {
                foreach (Item item in listI)
                {
                    if (item.Name == obj.Name)
                    {
                        try
                        {
                            Convert.ToDouble(item.Now);
                        }
                        catch
                        {
                            item.Now = "0";
                        }
                        try
                        {
                            Convert.ToDouble(obj.Now);
                        }
                        catch
                        {
                            obj.Now = "0";
                        }
                        obj.Now = (Convert.ToDouble(obj.Now) - Convert.ToDouble(item.Now) * newV).ToString();
                        await client.SetTaskAsync("Category/" + date.Day.ToString() + date.Month.ToString() + date.Year.ToString() + obj.Id, obj);
                        break;
                    }
                }
            }

            try
            {
                Convert.ToDouble(dataS.Now);
            }
            catch
            {
                dataS.Now = "0";
            }

            dataS.Last = newV.ToString();
            dataS.Old = dataS.Now;
            dataS.Now = (Convert.ToDouble(dataS.Now) + newV).ToString();
            dataS.New = null;
            await client.SetTaskAsync("Out/" + date.Day.ToString() + date.Month.ToString() + date.Year.ToString() + dataS.Id, dataS);
            progress.Value = 99;

            dataGrid2.Items.Refresh();
            dataGrid2.IsEnabled = true;
            progress.Visibility = Visibility.Hidden;
        }

        private void tab_Loaded(object sender, RoutedEventArgs e)
        {
            Load();
        }

        private void tab_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (pos != tab.SelectedIndex)
            {
                pos = tab.SelectedIndex;
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
                Load();
            }
        }

        private async void Set_Items(object sender, RoutedEventArgs e)
        {
            try
            {
                Data data = dataGrid2.SelectedItem as Data;
                SetItems setItems = new SetItems(data.Name, data.Id, data.Key, picker);

                if (setItems.ShowDialog() == true)
                {
                    if (picker.SelectedDate.Value.Day == date.Day &&
                        picker.SelectedDate.Value.Month == date.Month &&
                        picker.SelectedDate.Value.Year == date.Year)
                    {
                        progress.Value = 0;
                        progress.Visibility = Visibility.Visible;

                        var items = new ItemsCount
                        {
                            cnt = setItems.ItemsList.Count.ToString(),
                        };

                        if (data.Key != null)
                        {
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
                        else
                        {
                            MessageBox.Show("Не удалось получить ключ.");
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
