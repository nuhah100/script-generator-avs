using Microsoft.Win32;
using ScriptGeneratorAVS.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ScriptGeneratorAVS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SetUpDLL dl = new SetUpDLL();

        public MainWindow()
        {
            InitializeComponent();
            if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\" + Paths.SaveName))
            {
                string[] a = File.ReadAllLines(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\" + Paths.SaveName);
                Builder.SetPlugins(a);
            }
            else
            {
                MessageBox.Show("You must first set all the dll files.", "Warning");
                dl.Show();
            }
            Builder.SetSound(false);
        }
        private void BtnFindVideo_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog f = new OpenFileDialog
            {
                Filter = " Matroska Multimedia Container (*.mkv)|*.mkv|All files (*.*)|*.*",//
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                Multiselect = false
            };

            if (f.ShowDialog() == true)
            {
                txtVideoUrl.Text = System.IO.Path.GetFileName(f.FileName);
                Builder.SetMainVideo(f.FileName);

            }

        }

        private void BtnFindSubs_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog f = new OpenFileDialog
            {
                Multiselect = true,
                Filter = " Aegisub Advanced Subtitle (*.ass)|*.ass|All files (*.*)|*.*",//Aegisub Advanced Subtitle
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };

            if (f.ShowDialog() == true)
            {
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
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };

            if (f.ShowDialog() == true)
            {
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
            SaveFileDialog f = new SaveFileDialog();
            f.Filter = "AVS Files (*.avs)| *.avs";
            f.DefaultExt = "avs";
            bool? result = f.ShowDialog();
            if (result == true)
            {
                System.IO.Stream fileStream = f.OpenFile();
                System.IO.StreamWriter sw = new System.IO.StreamWriter(fileStream);
                sw.WriteLine(Builder.Build());
                sw.Flush();
                sw.Close();
            }
        }
    }
}
