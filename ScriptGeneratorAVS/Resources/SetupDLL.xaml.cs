using Microsoft.Win32;
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
using System.Windows.Shapes;
using System.IO;
using ScriptGeneratorAVS.Classes;

namespace ScriptGeneratorAVS
{
    /// <summary>
    /// Interaction logic for SetUpDLL.xaml
    /// </summary>
    public partial class SetUpDLL : Window
    {
        public SetUpDLL()
        {
            InitializeComponent();
        }

        private void BtnVS_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog f = new OpenFileDialog
            {
                Filter = " Dynamic-Link Library (*.dll)|*.dll|All files (*.*)|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                Multiselect = false
            };

            if (f.ShowDialog() == true)
            {
                Console.WriteLine(((Button)sender).Name);
                switch (((Button)sender).Name)
                {
                    case "btnVS":
                        {
                            txtVS.Text = f.FileName;
                            break;
                        }
                    case "btnFF":
                        {
                            txtFF.Text = f.FileName;
                            break;
                        }
                    case "btnIQ":
                        {
                            txtIQ.Text = f.FileName;
                            break;
                        }
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(txtFF.Text == "" || txtIQ.Text == ""||txtVS.Text =="")
            {
                MessageBox.Show("You didnt fill all the fields.", "Notification",MessageBoxButton.OK,MessageBoxImage.Information);
                return;
            }
            string s = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string name = Paths.SaveName;
            
            File.WriteAllText(s+@"/"+name,txtFF.Text + "\n"+txtIQ.Text+"\n"+txtVS.Text+"\n");
            this.Close();
            
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            
        }
    }
}
