using Microsoft.Win32;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Threading;

namespace CLRCM
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer timeTimer = new DispatcherTimer();
        DispatcherTimer lrcTimer = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();;
            timeTimer.Interval = TimeSpan.FromMilliseconds(1);
            timeTimer.Tick += new EventHandler(timeTimer_Tick);
            timeTimer.Start();
            lrcTimer.Interval = TimeSpan.FromMilliseconds(10);
            lrcTimer.Tick += new EventHandler(lrcTimer_Tick);

        }

        void timeTimer_Tick(object sender, EventArgs e)
        {
            if (mediaElement.NaturalDuration.HasTimeSpan)
            {
                string ms = mediaElement.Position.Milliseconds.ToString();
                string s = mediaElement.Position.Seconds.ToString();
                string m = mediaElement.Position.Minutes.ToString();
                switch (ms.Length)
                {
                    case 1:
                        ms = "0" + ms;
                        break;
                    case 2:
                        ms = "0" + ms.Substring(0, 1);
                        break;
                    case 3:
                        ms = ms.Substring(0, 2);
                        break;
                }
                if (s.Length == 1)
                {
                    s = "0" + s;
                }
                if (m.Length == 1)
                {
                    m = "0" + m;
                }
                label.Content = m + ":" + s + "." + ms;
                progressBar.Value = mediaElement.Position.TotalSeconds;
            }
        }

        private void lrcTimer_Tick(object sender, EventArgs e)
        {
            MatchCollection mc = new Regex(@"\[([0-9.:]*)\]+(.*)", RegexOptions.Compiled).Matches(textBox.Text);
            int istart = 0;
            int iend = 0;
            foreach (Match m in mc)
            {
                if (TimeSpan.Parse("00:" + m.Groups[1].Value) <= mediaElement.Position)
                {
                    istart = textBox.Text.IndexOf(m.Value);
                    iend = m.Value.Length;
                }
            }
            textBox.Select(istart, iend);
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "音频文件|*.aac;*.ape;*.mp3;*.ogg;*.wav;*.wma";
            dialog.Multiselect = false;
            if (dialog.ShowDialog() == true)
            {
                mediaElement.Source = new Uri(dialog.FileName);
                mediaElement.Play();
                progressBar.IsIndeterminate = true;
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            mediaElement.Play();
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            mediaElement.Pause();
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            mediaElement.Position += TimeSpan.FromSeconds(5);
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            mediaElement.Position -= TimeSpan.FromSeconds(5);
        }

        private void checkBox_Checked(object sender, RoutedEventArgs e)
        {
            mediaElement.SpeedRatio = 0.75;
        }

        private void checkBox_Unchecked(object sender, RoutedEventArgs e)
        {
            mediaElement.SpeedRatio = 1;
        }

        private void checkBox1_Checked(object sender, RoutedEventArgs e)
        {
            lrcTimer.Start();
        }

        private void checkBox1_Unchecked(object sender, RoutedEventArgs e)
        {
            lrcTimer.Stop();
        }

        private void button5_Click(object sender, RoutedEventArgs e)
        {
            int line = textBox.GetLineIndexFromCharacterIndex(textBox.CaretIndex);
            textBox.Text = textBox.Text.Insert(textBox.GetCharacterIndexFromLineIndex(line), "[" + label.Content + "]");
            if (textBox.GetLineIndexFromCharacterIndex(textBox.Text.Length - 1) > line)
            {
                textBox.CaretIndex = textBox.GetCharacterIndexFromLineIndex(line + 1);
            }
        }

        private void mediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            progressBar.IsIndeterminate = false;
            progressBar.Maximum = mediaElement.NaturalDuration.TimeSpan.TotalSeconds;
        }

        private void mediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            mediaElement.Position = TimeSpan.FromMilliseconds(0);
        }
    }
}
