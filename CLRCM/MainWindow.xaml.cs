using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Threading;
namespace CLRCM
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer timeTimer;
        DispatcherTimer lrcTimer;

        public MainWindow()
        {
            InitializeComponent();
            timeTimer = new DispatcherTimer();
            timeTimer.Interval = TimeSpan.FromTicks(1);
            timeTimer.Tick += new EventHandler(timeTimer_Tick);
            timeTimer.Start();
            lrcTimer = new DispatcherTimer();
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
            string[] lines = System.Text.RegularExpressions.Regex.Split(textBox.Text, Environment.NewLine);
            foreach (var line in lines)
            {
                if (line.IndexOf("[" + label.Content + "]") == 0)
                {
                    textBox.Select(textBox.Text.IndexOf(line), line.Length);
                }
            }
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
            MessageBox.Show("由于程序设计缺陷，在性能较低的机器上运行会出现部分歌词没有扫描的BUG。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            lrcTimer.Start();
        }

        private void checkBox1_Unchecked(object sender, RoutedEventArgs e)
        {
            lrcTimer.Stop();
        }

        private void button5_Click(object sender, RoutedEventArgs e)
        {
            mediaElement.Pause();
            int line = textBox.GetLineIndexFromCharacterIndex(textBox.CaretIndex);
            textBox.Text = textBox.Text.Insert(textBox.GetCharacterIndexFromLineIndex(line), "[" + label.Content + "]");
            if (textBox.GetLineIndexFromCharacterIndex(textBox.Text.Length - 1) > line)
            {
                textBox.CaretIndex = textBox.GetCharacterIndexFromLineIndex(line + 1);
            }
            mediaElement.Play();
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
