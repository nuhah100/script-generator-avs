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

namespace ScriptGeneratorAVS
{
    /// <summary>
    /// Interaction logic for Frames.xaml
    /// </summary>
    public partial class Frames : Window
    {
        public Frames(string FileName)
        {
            InitializeComponent();
            lblHeader.Content = FileName;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private bool CheckFrames()
        {
            if (string.IsNullOrEmpty(txtFEnd.Text) || string.IsNullOrEmpty(txtFStart.Text))
            {
                MessageBox.Show("You must set start and end frames!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }
        public static Tuple<string,string> GetFrames(string File)
        {
            Frames window = new Frames(File);
            window.ShowDialog();


            if (window.CheckFrames())
            {
                Tuple<string, string> ok = new Tuple<string, string>
                    (
                    window.txtFStart.Text,
                        window.txtFEnd.Text
                    );

                return ok;
            }
            else
                return null;
        }

    }
}
