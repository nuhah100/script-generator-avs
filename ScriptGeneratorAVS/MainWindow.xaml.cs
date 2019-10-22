using Microsoft.Win32;
using ScriptGeneratorAVS.Classes;
using System;
using System.Collections.Generic;
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
        }
        private void BtnFindVideo_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog f = new OpenFileDialog();
            f.Filter = " Matroska Multimedia Container (*.mkv)|*.mkv|All files (*.*)|*.*";//
            f.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            f.Multiselect = false;

            if (f.ShowDialog() == true)
            {
                txtVideoUrl.Text = System.IO.Path.GetFileName(f.FileName);
                Builder.SetMainVideo(f.FileName);
            }

        }

        private void BtnFindSubs_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog f = new OpenFileDialog();
            f.Multiselect = true;
            f.Filter = " Aegisub Advanced Subtitle (*.ass)|*.ass|All files (*.*)|*.*";//Aegisub Advanced Subtitle
            f.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            if (f.ShowDialog() == true)
            {
                foreach (string filename in f.FileNames)
                {
                    lbSubs.Items.Add(System.IO.Path.GetFileName(filename));
                    Builder.AddSubtitle(filename);
                }
            }

        }

        private void BtnBuild_Click(object sender, RoutedEventArgs e)
        {
            Console.BackgroundColor = ConsoleColor.Green;
            Console.WriteLine(Builder.GetMainVideo() + "\n" + Builder.GetSubtitles());
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            dl.Show();
        }
    }
}
