using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;

namespace MeetingApp
{
    /// <summary>
    /// Логика взаимодействия для CreateWindow.xaml
    /// </summary>
    public partial class CreateWindow : Window
    {
        public string datetime;

        public CreateWindow()
        {
            InitializeComponent();
            ButtonEdit.Visibility = Visibility.Hidden;
            ButtonCreateSave.Visibility = Visibility.Visible;
            MeetingName.Text = "";
            MeetingDescription.Text = "";
            TypeMeeting.Text = "";
            MeetingDateTime.Text = "";

            TypeMeeting.Items.Add("Свободная");
            TypeMeeting.Items.Add("Бизнес");
            TypeMeeting.Items.Add("Деловая");
            TypeMeeting.Text = "Test";
            clock_start();
        }
        public CreateWindow(string name, string description, string type, string t)
        {
            InitializeComponent();
            MeetingName.Text = name;
            MeetingDescription.Text = description;
            TypeMeeting.Text = type;
            MeetingDateTime.Text = t;
            datetime = t;

            TypeMeeting.IsEnabled = false;
            ButtonEdit.Visibility = Visibility.Visible;
            ButtonCreateSave.Visibility = Visibility.Hidden;
            clock_start();

            calendar1.SelectedDate = DateTime.Parse(t);
            string[] clocks = t.Remove(0, 11).Split(':');
            hours.Text = $"{clocks[0]}";
            minutes.Text = $"{clocks[1]}";
        }
        string path = "Data.dat";
        bool is_free;

        public void clock_start()
        {
            for (int i = 0; i < 24; i++)
            {
                if (i < 10)
                    hours.Items.Add($"0{i}");
                else
                    hours.Items.Add(i);
            }
            for (int i = 0; i < 60; i++)
            {
                if (i < 10)
                    minutes.Items.Add($"0{i}");
                else
                    minutes.Items.Add(i);
            }
        }

        public void data_to_text()
        {
            try 
            {
                DateTime date = (DateTime)calendar1.SelectedDate;
                string s = $"{date} {hours.Text}:{minutes.Text}";
                s = s.Remove(10, 8);
                MeetingDateTime.Text = s;
            }
            catch
            {
                MessageBox.Show("Не удалось ввести дату! Проверьте, указаны ли у вас дата и время встречи.", "Ошибка данных!", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public void free_time(string dates)
        {
            using (StreamReader sr = new StreamReader(path))
            {
                is_free = true;
                String line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line != "" && line.Contains(dates))
                        is_free = false;
                }
                sr.Close();
            }
        }

        private void ButtonCreateSave_Click(object sender, RoutedEventArgs e)
        {
            //*
            {
                data_to_text();
                if (MeetingName.Text.Trim() != "" && TypeMeeting.Text.Trim() != "" && MeetingDateTime.Text.Trim() != "" && MeetingDateTime.Text.Length == 16)
                {
                    free_time(MeetingDateTime.Text.Trim());
                    FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write);
                    if (is_free == true && DateTime.Parse(MeetingDateTime.Text.Trim()) > DateTime.Now)
                    {
                        StreamWriter sw = new StreamWriter(fs);
                        sw.WriteLine($"{MeetingName.Text.Trim()}|{MeetingDescription.Text} |{TypeMeeting.Text.Trim()}|{MeetingDateTime.Text.Trim()}");
                        sw.Close();
                    }
                    else
                    {
                        MessageBox.Show("Нельзя создать несколько встреч на одно и то же время или в прошлом.", "Неверное время!", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    fs.Close();//*/
                }
                else
                { 
                    MessageBox.Show("Не все данные введены! Пожалуйста, введите данные.", "Ошибка данных!", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
            (this.Owner as MainWindow).Show_List();

            this.Hide();
        }

        private void CreateCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            string txt_new = null;
            string text = File.ReadAllText(path);
            string[] meeting = text.Split('\n');
            data_to_text();

            if (DateTime.Parse(MeetingDateTime.Text.Trim()) > DateTime.Now)
            {
                for (int i = 0; i < meeting.Length; i++)
                {
                    if (meeting[i].Contains(datetime))
                    {
                        txt_new += $"{MeetingName.Text}|{MeetingDescription.Text}|{TypeMeeting.Text}|{MeetingDateTime.Text}\n";
                    }
                    else
                    {
                        txt_new += meeting[i] + "\n";
                    }
                }
                File.WriteAllText(path, string.Empty);
                while (txt_new.Contains($"\n\n"))
                {
                    txt_new = txt_new.Replace($"\n\n", $"\n");
                }

                using (System.IO.StreamWriter file = new System.IO.StreamWriter(path))
                {
                    file.Write(txt_new);
                }
                (this.Owner as MainWindow).Show_List();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Нельзя перенести встречу в прошлое.", "Неверное время!", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

    }
}
