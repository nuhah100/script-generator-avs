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
using Path = System.IO.Path;
using static System.IO.Path;
using System.IO;

namespace ScriptGeneratorAVS.Resources
{
    /// <summary>
    /// Interaction logic for Upload.xaml
    /// </summary>
    public partial class Upload : Window
    {
        public Upload()
        {
            InitializeComponent();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if(!string.IsNullOrEmpty(pws.Password) || pws.IsFocused)
            {
                weq.Visual.SetValue(Label.ContentProperty,string.Empty);
            }
            if(string.IsNullOrEmpty(pws.Password) && !pws.IsFocused)
            {
                weq.Visual.SetValue(Label.ContentProperty, "Password");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string y = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)));
            //string y = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string pa = @"\FFMPEG\doc\";
            string n = "Saves.txt";
            string c = y + pa + n;
            FileStream fa;
            if (File.Exists(c))
            {
                fa = File.OpenWrite(c);
            }
            else
            {
                fa = File.Create(c);
            }
            fa.SetLength(0);
            fa.Flush();
            byte[] info = new UTF8Encoding(true).GetBytes(em.Text + "\n" + pws.Password);
            fa.Write(info, 0, info.Length);

            // writing data in bytes already
            byte[] data = new byte[] { 0x0 };
            fa.Write(data, 0, data.Length);
            fa.Flush();
            fa.Close();
            Application.Current.MainWindow.Focus();
            this.Close();
        }
    }
}
