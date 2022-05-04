using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MeetingApp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            if (File.Exists("Data.dat") != true)
                File.Create("Data.dat");

            type1.Items.Add("Любой тип");
            type1.Items.Add("Свободная");
            type1.Items.Add("Бизнес");
            type1.Items.Add("Деловая");
            type1.Text = "Любой тип";

            Show_List();
        }
        string path = "Data.dat";
        CreateWindow CW = new CreateWindow();
        //CreateWindow EW = new CreateWindow(name);
        class MeetingData
        {
            public MeetingData(string Name, string Description, string Type, string Date)
            {
                this.Name = Name;
                this.Description = Description;
                this.Type = Type;
                this.Date = Date;
            }
            public string Name { get; set; }
            public string Description { get; set; }
            public string Type { get; set; }
            public string Date { get; set; }
        }
        

        List<string[]> data = new List<string[]>();
        string[] info = new string[3];

        public void Show_List()
        {
            DataGrid1.ItemsSource = null;
            List<MeetingData> result = new List<MeetingData>(3);

            if (File.Exists(path))
            {
                //try
                {
                    using (StreamReader sr = new StreamReader(path))
                    {
                        String line;
                        while ((line = sr.ReadLine()) != null) //Читает все строки и следом выводит их
                        {
                            info = line.Split('|');
                            if (info[0].Trim() != "" && info[2].Trim() != "" && info[3].Trim() != "")
                                if(type1.Text == "Любой тип" && (info[0].Contains(Search.Text) || info[1].Contains(Search.Text) || info[3].Contains(Search.Text)))
                                    result.Add(new MeetingData(info[0], info[1], info[2], info[3]));
                                else if (type1.Text != "Любой тип" && (info[0].Contains(Search.Text) || info[1].Contains(Search.Text) || info[3].Contains(Search.Text)))
                                    result.Add(new MeetingData(info[0], info[1], info[2], info[3]));

                        }
                        sr.Close();
                    }
                }
                //catch 
                {
                    
                }
            }
            DataGrid1.ItemsSource = result;//*/
        }


        private void Create_Click(object sender, RoutedEventArgs e)
        {
            CW.Owner = this;
            CW.ShowDialog();
        }
        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Search.Text != "")
                Search.Opacity = 100;
            else Search.Opacity = 50;

            Show_List();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Closing1(object sender, CancelEventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите выйти?", "Выход", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                e.Cancel = true;
            else
                CW.Close();
        }


        private void Header_Name(object sender, EventArgs e)
        {
            DataGrid1.Columns[0].Header = "Имя";
            DataGrid1.Columns[1].Header = "Описание";
            DataGrid1.Columns[2].Header = "Тип";
            DataGrid1.Columns[3].Header = "Дата";

            DataGrid1.Columns[0].Width = 100;
            DataGrid1.Columns[2].Width = 100;
            DataGrid1.Columns[3].Width = 100;
            DataGrid1.Columns[1].Width = 308;

            /*int w1 = Convert.ToInt32(DataGrid1.Columns[0].ActualWidth);
            int w2 = Convert.ToInt32(DataGrid1.Columns[2].ActualWidth);
            int w3 = Convert.ToInt32(DataGrid1.Columns[3].ActualWidth);
            int w0 = Convert.ToInt32(DataGrid1.ActualWidth);
            //DataGrid1.Columns[1].MaxWidth =     ;
            debug.Content = $"{w1} {w2} {w3} {w0} {DataGrid1.Columns[0].CellStyle}";//*/
            DataGrid1.Columns[0].IsReadOnly = false;

        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            int ind = DataGrid1.SelectedIndex;
            //DataGridCell test = new DataGridCell();
            if (MessageBox.Show("Вы уверены, что хотите удалить встречу?", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                string t = (DataGrid1.Columns[3].GetCellContent(DataGrid1.Items[ind]) as TextBlock).Text.ToString();
                try
                {
                    string txt_new = null;
                    string text = File.ReadAllText(path);
                    string[] meetings = text.Split('\n');
                    //*
                    for (int i = 0; i < meetings.Length; i++)
                    {
                        if (meetings[i].Contains(t))
                        {
                            meetings[i] = String.Empty;
                        }
                        else
                        {
                            txt_new += meetings[i] + "\n";
                            //debug.Content = txt_new;
                        }
                    }//*/
                    File.WriteAllText(path, string.Empty);
                    while (txt_new.Contains($"\n\n"))
                    {
                        txt_new = txt_new.Replace($"\n\n", $"\n");
                    }
                    using (StreamWriter file = new StreamWriter(path))
                    {
                        file.Write(txt_new);
                    }
                }


                catch
                {
                    debug.Content = "Возникла неизвестная ошибка";
                }
            }
            Show_List();
            //debug.Content = $"{ind} {t}";
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int ind = DataGrid1.SelectedIndex;
                string name = (DataGrid1.Columns[0].GetCellContent(DataGrid1.Items[ind]) as TextBlock).Text.ToString();
                string description = (DataGrid1.Columns[1].GetCellContent(DataGrid1.Items[ind]) as TextBlock).Text.ToString();
                string type = (DataGrid1.Columns[2].GetCellContent(DataGrid1.Items[ind]) as TextBlock).Text.ToString();
                string t = (DataGrid1.Columns[3].GetCellContent(DataGrid1.Items[ind]) as TextBlock).Text.ToString();

                CreateWindow EW = new CreateWindow(name, description, type, t);
                EW.Owner = this;
                EW.ShowDialog();
            }
            catch
            {
                debug.Content = "Возникла неизвестная ошибка";
            }
        }


        private void event2(object sender, RoutedEventArgs e)
        {
            Show_List();
        }
    }
}
