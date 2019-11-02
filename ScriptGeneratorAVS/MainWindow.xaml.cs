﻿using Microsoft.Win32;
using ScriptGeneratorAVS.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Path = System.IO.Path;
using static System.IO.Path;
using FFMpegCore.FFMPEG;
using FFMpegCore;
using FFMpegCore.Enums;
using FFMpegCore.FFMPEG.Enums;
using FFMpegCore.FFMPEG.Argument;
using System.Timers;

namespace ScriptGeneratorAVS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SetUpDLL dl = new SetUpDLL();
        FFMpeg encoder;
        System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();
        System.Windows.Threading.DispatcherTimer tt =  new System.Windows.Threading.DispatcherTimer();
        private bool mediaPlayerIsPlaying = false;
        private bool userIsDraggingSlider = false;
        string a,AvsFilePath = null;
        ArgumentContainer container = new ArgumentContainer();
        public MainWindow()
        {
            FFMpegOptions.Configure(new FFMpegOptions { RootDirectory = @"C:\Users\moish\source\repos\ScriptGeneratorAVS\ScriptGeneratorAVS\FFMPEG\bin\" });
            encoder = new FFMpeg();
            InitializeComponent();
            if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\" + Paths.SaveName))
            {
                string[] a = File.ReadAllLines(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\" + Paths.SaveName);
                Builder.SetPlugins(a);
            }
            else
            {
                MessageBox.Show("You must first set all the dll files.", "Warning", MessageBoxButton.OK, MessageBoxImage.Information);
                dl.Show();
            }
            Builder.SetSound(false);
            timer.Interval = TimeSpan.FromSeconds(0.5);
            timer.Tick += timer_Tick;
            //mpVideo.Source = new Uri(@"G:\Boku\[HorribleSubs] Boku no Hero Academia - 66 [1080p].mkv");
            timer.Start();
        }


        private void BtnFindVideo_Click(object sender, RoutedEventArgs e)
        {
            string url;
            OpenFileDialog f = new OpenFileDialog
            {
                Filter = " Video Files (*.mkv/*.mp4)|*.mkv;*.mp4|All files (*.*)|*.*",//" Matroska Multimedia Container (*.mkv)|*.mkv|All files (*.*)|*.*",//
                InitialDirectory = Paths.LastPathUse,
                Multiselect = false
            };

            if (f.ShowDialog() == true)
            {
                url = f.FileName;
                Paths.LastPathUse = GetDirectoryName(url);
                txtVideoUrl.Text = GetFileName(url);
                Builder.SetMainVideo(url);
                //mpVideo.Source =new Uri(url);
                lblVideoDetails.Content = new VideoInfo(f.FileName).ToString();
                string s = lblVideoDetails.Content.ToString();
                string[] r = s.Split('\n');
                double n = Paths.GetVideoDuration(f.FileName).TotalSeconds;
                string[] qq = r[8].Split(' ');
                Builder.VideoFrameRate = double.Parse(qq[2].Split('f')[0]);
                Builder.VideoFrames = n * Builder.VideoFrameRate;
                if (Path.GetExtension(url) == ".mkv")
                {
                    r[4] = "Video Duration: " + Paths.GetVideoDuration(f.FileName).ToString(@"hh\:mm\:ss");
                    StringBuilder a = new StringBuilder();
                    for (int i = 0; i < r.Length -1; i++)
                    {
                        a.Append(r[i] + "\n");
                    }
                    a.Append("Size: " + (Paths.ConvertToSize(new FileInfo(url).Length)));
                    lblVideoDetails.Content = a.ToString();
                }
            }
        }

        private void BtnFindSubs_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog f = new OpenFileDialog
            {
                Multiselect = true,
                Filter = " Aegisub Advanced Subtitle (*.ass)|*.ass|All files (*.*)|*.*",//Aegisub Advanced Subtitle
                InitialDirectory = Paths.LastPathUse
            };

            if (f.ShowDialog() == true)
            {
                Paths.LastPathUse = GetDirectoryName(f.FileNames[0]);
                foreach (string filename in f.FileNames)
                {
                    lbSubs.Items.Add(System.IO.Path.GetFileName(filename));
                    Builder.AddSubtitle(filename);
                }
            }

        }



        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            dl.Show();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Window_Closed(object sender, System.ComponentModel.CancelEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void btnEffects_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog f = new OpenFileDialog
            {
                Multiselect = true,
                Filter = " Portable Network Graphics Audio Video Interleave (*.png;*avi)|*.png;*.avi|All files (*.*)|*.*",//Aegisub Advanced Subtitle
                InitialDirectory = Paths.LastPathUse
            };

            if (f.ShowDialog() == true)
            {
                Paths.LastPathUse = GetDirectoryName(f.FileNames[0]);
                foreach (string filename in f.FileNames)
                {
                    lbEffects.Items.Add(System.IO.Path.GetFileName(filename));
                    Builder.AddEffect(filename);
                }
            }
        }

        private void btnClearSubs_Click(object sender, RoutedEventArgs e)
        {
            lbSubs.Items.Clear();
            Builder.RemoveSubtitles();
        }

        private void btnRemoveSubs_Click(object sender, RoutedEventArgs e)
        {
            int n = lbSubs.SelectedIndex;
            if (n == -1)
                return;
            lbSubs.Items.RemoveAt(n);
            Builder.RemoveSubtitle(n);
        }

        private void cbSound_Click(object sender, RoutedEventArgs e)
        {
            bool b = cbSound.IsChecked.Value;
            Builder.SetSound(b);
        }

        private void btnEffectRemove_Click(object sender, RoutedEventArgs e)
        {
            Builder.RemoveEffect(lbEffects.SelectedIndex);
            lbEffects.Items.RemoveAt(lbEffects.SelectedIndex);
        }

        private void btnEffectsClear_Click(object sender, RoutedEventArgs e)
        {
            Builder.RemoveEffects();
            lbEffects.Items.Clear();
        }


        private void BtnBuild_Click(object sender, RoutedEventArgs e)
        {
            if (txtVideoUrl.Text == "" || txtVideoUrl.Text == null)
            {
                MessageBox.Show("You Must Set a Video!", "Warning", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            SaveFileDialog f = new SaveFileDialog();
            f.Filter = "AVS Files (*.avs)| *.avs";
            f.DefaultExt = "avs";
            f.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            bool? result = f.ShowDialog();
            if (result == true)
            {
                System.IO.Stream fileStream = f.OpenFile();
                System.IO.StreamWriter sw = new System.IO.StreamWriter(fileStream);
                sw.WriteLine(Builder.Build(false));
                sw.Flush();
                sw.Close();
            }
        }

        private void Window_Closed(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
        private void btnPre_Click(object sender, RoutedEventArgs e)
        {
            container.Clear();
            var ran = new Random();
            a = DateTime.Now.ToString("MM-dd-hh-mm"); 
            File.WriteAllText(Path.GetTempPath() + "Script" + a + ".avs",Builder.Build(true));



            SaveFileDialog s = new SaveFileDialog();
            s.Filter = "Video Files(*.mp4/*.mkv)|*.mp4;*.mkv";
            s.DefaultExt = ".mkv";
            if(s.ShowDialog() == false)
            {
                MessageBox.Show("You must choose a path.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
                return;
            }

            container.Add(new InputArgument(Path.GetTempPath() + "Script" + a + ".avs"));
            //container.Add(new SubArgument(new Uri(Builder.Subtitles[0]).AbsolutePath));
            //container.Add(new VideoCodecArgument(VideoCodec.LibX264));
            //container.Add(new TrimArgument(" 00:02:20", " 00:00:30"));
            container.Add(new ThreadsArgument(true));
            container.Add(new LogArgument(Path.GetTempPath() + @"log"+a+".txt"));
            container.Add(new FilterComplex(InputData()));
            container.Add(new OutputArgument(new Uri(s.FileName)));
            container.Add(new OverrideArgument());

            Task t = Task.Run(() => {
                encoder.Convert(container);
            });

            tt.Interval = TimeSpan.FromMilliseconds(400);
            tt.Tick += Tt_Tick;
            tt.Start();
            //t.Wait();

        }

        private List<string> InputData()
        {
            List<string> q = new List<string>();
            string s;
            char sinq = char.Parse("'")
                , a = 'a';
            List<string> Su = Builder.Subtitles;
            for (int i = 0; i < Su.Count; i++)
            {
                s = new Uri(Su[i]).AbsolutePath;
                if(i == 0)
                    q.Add(@"ass=\" + sinq + s + @"\" + sinq + "[" + (++a) + "];");
                else
                    if(i == Su.Count - 1)
                        q.Add("[" + a + "]" + @"ass=\" + sinq + s + @"\" + sinq);
                    else
                        q.Add("[" + a + "]" + @"ass=\" + sinq + s + @"\" + sinq + "[" + (++a) + "];");

            }

            return q;
        }

        private void Tt_Tick(object sender, EventArgs e)
        {


            if (File.Exists(Path.GetTempPath() + @"log"+a+".txt"))
            {
                StringBuilder q = new StringBuilder();
                using FileStream stream = File.Open(Path.GetTempPath() + @"log" + a + ".txt", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using (StreamReader reader = new StreamReader(stream))
                {
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        q.Append(line + "\n");
                    }
                }
                if (container.Contains<TrimArgument>())
                {
                    TimeSpan rq = new TimeSpan();
                }
                string ls = q.ToString();
                if (ls == null || ls == "")
                    return;
                string[] ss, y = ls.Split(new char[] { '\n' });
                Array.Reverse(y);
                ss = new string[12];
                for (int ii = 0; ii < ss.Length; ii++)
                {
                    ss[ii] = y[12 - ii];
                }
                double d = (100 * double.Parse(ss[0].Split('=')[1].ToString())) / Builder.VideoFrames;
                d = Math.Round(d * 10000) / 10000d;
                if (ss[11].Split('=')[1] == "end")
                {
                    tt.Stop();
                    encoder.Kill();
                    mpVideo.Source = new Uri(Path.GetDirectoryName(Builder.GetMainVideo()) + @"\gt.mp4");
                    d = 100;
                    File.Delete(Path.GetTempPath() + @"log" + a + ".txt");
                }
                lblVideoInfo.Content = d + "%";
                prbPro.Value = d;
            }
        }
 

        private void btnStopEncode_Click(object sender, RoutedEventArgs e)
        {
            encoder.Kill();
        }

        private void mpVideo_MediaOpened(object sender, RoutedEventArgs e)
        {
            //mediaPlayerIsPlaying = true;
            timer.Start();
        }

        private void slVideo_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            
            
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (mediaPlayerIsPlaying)
                mpVideo.Play();
            else
                mpVideo.Pause();
            if ((mpVideo.Source != null) && (mpVideo.NaturalDuration.HasTimeSpan) && (!userIsDraggingSlider))
            {
                slVideo.Minimum = 0;
                slVideo.Maximum = mpVideo.NaturalDuration.TimeSpan.TotalSeconds;
                slVideo.Value = mpVideo.Position.TotalSeconds;
            }
        }

        private void slVideo_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            userIsDraggingSlider = true;
        }

        private void slVideo_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            userIsDraggingSlider = false;
            mpVideo.Position = TimeSpan.FromSeconds(slVideo.Value);
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayerIsPlaying = !mediaPlayerIsPlaying;
        }
    }
}
