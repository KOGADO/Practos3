using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace WpfApp1
{
   
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            VolumeSlider.Minimum = 0;
            VolumeSlider.Maximum = 1;
            VolumeSlider.Value = 1;
            songList.DisplayMemberPath = "songName";
        }


            int fileIndex = 0;
            Random rand = new Random();
            List<Song> fileList = new List<Song>();
            List<Song> randomList = new List<Song>();
            bool pause = false;
            bool random = false;

        private void audioSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            media.Position = new TimeSpan(Convert.ToInt64(audioSlider.Value));
        }

        private void media_MediaOpened(object sender, RoutedEventArgs e)
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += new EventHandler(Timer_Tick);
            timer.Interval = new TimeSpan(0, 0, 0, 0, 1);

            timer.Start();
            audioSlider.Maximum = media.NaturalDuration.TimeSpan.Ticks;
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            if (songList.SelectedIndex == 0 | songList.SelectedIndex == -1)
            {
                songList.SelectedIndex = songList.Items.Count - 1;
            }
            else
            {
                songList.SelectedIndex--;
            }
        }

        private void play_Click(object sender, RoutedEventArgs e)
        {
            if (fileList.Count != 0)
            {
                media.Stop();
                audioSlider.Value = 0;
                if (!random)
                {
                    media.Source = new Uri(fileList[songList.SelectedIndex].path);
                }
                else
                {
                    media.Source = new Uri(randomList[songList.SelectedIndex].path);
                }

                media.Play();
            }
        }

        private void next_Click(object sender, RoutedEventArgs e)
        {
            if (songList.SelectedIndex + 1 == songList.Items.Count)
            {
                songList.SelectedIndex = 0;
            }
            else
            {
                songList.SelectedIndex++;
            }
        }

        private void pause_Click(object sender, RoutedEventArgs e)
        {
            if (!pause)
            {
                media.Pause();
                pause = true;
            }
            else
            {
                media.Play();
                pause = false;
            }
        }

        private void randomize_Click(object sender, RoutedEventArgs e)
        {
            RandomList();
        }

        private void open_folder_Click(object sender, RoutedEventArgs e)
        {
            if (random) randomize_Click(sender, e);
            if (pause) randomize_Click(sender, e);
            media.Stop();

            CommonOpenFileDialog dialog = new CommonOpenFileDialog { IsFolderPicker = true };
            dialog.Title = "Выберите папку с музыкой";

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                string[] files = Directory.GetFiles(dialog.FileName);

                fileList.Clear();
                foreach (string file in files)
                {
                    if (file.EndsWith(".mp3") | file.EndsWith(".waw") | file.EndsWith(".m4a"))
                    {
                        fileList.Add(new Song(file));
                    }
                }

                songList.ItemsSource = fileList;
                songList.SelectedIndex = 0;

                play_Click(sender, e);
            }
        }

        private void history_Click(object sender, RoutedEventArgs e)
        {

        }
        private void RandomList()
        {
            if (!random)
            {
                random = true;

                fileIndex = songList.SelectedIndex;

                foreach (Song song in fileList)
                {
                    randomList.Add(song);
                }

                for (int i = randomList.Count - 1; i >= 1; i--)
                {
                    int j = rand.Next(i + 1);

                    var temp = randomList[j];
                    randomList[j] = randomList[i];
                    randomList[i] = temp;
                }

                songList.ItemsSource = randomList;
                songList.SelectedIndex = 0;

            }
            else
            {
                random = false;

                songList.ItemsSource = fileList;
                songList.SelectedIndex = fileIndex;
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            audioSlider.Value = media.Position.Ticks;

            if (media.NaturalDuration.HasTimeSpan)
            {
                if (media.Position.Seconds < 10)
                {
                    NowTime.Text = media.Position.Minutes.ToString() + ":0" + media.Position.Seconds.ToString();
                    RemainingTime.Text = (media.NaturalDuration.TimeSpan.Minutes - media.Position.Minutes).ToString() + ":" + (media.NaturalDuration.TimeSpan.Seconds - media.Position.Seconds).ToString();
                }
                else if (media.NaturalDuration.TimeSpan.Seconds - media.Position.Seconds < 10)
                {
                    NowTime.Text = media.Position.Minutes.ToString() + ":" + media.Position.Seconds.ToString();
                    RemainingTime.Text = (media.NaturalDuration.TimeSpan.Minutes - media.Position.Minutes).ToString() + ":" + (media.NaturalDuration.TimeSpan.Seconds - media.Position.Seconds).ToString();
                }
                else
                {
                    NowTime.Text = media.Position.Minutes.ToString() + ":" + media.Position.Seconds.ToString();
                    RemainingTime.Text = (media.NaturalDuration.TimeSpan.Minutes - media.Position.Minutes).ToString() + ":" + (media.NaturalDuration.TimeSpan.Seconds - media.Position.Seconds).ToString();
                }
            }
        }
    }
}