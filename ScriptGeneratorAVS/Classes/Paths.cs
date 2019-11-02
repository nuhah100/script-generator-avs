using Google.Protobuf;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using ScriptGeneratorAVS.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
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

namespace ScriptGeneratorAVS.Classes
{

    static class Paths
    {
  
        public static string FolderName= "ScriptGenerator"
            , SaveName = "Settings.txt";
        public static string LastPathUse  = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        internal static object ConvertToSize(long length)
        {
            string[] Suffix = { "B", "KB", "MB", "GB", "TB" };
            int i;
            double dblSByte = length;
            for (i = 0; i < Suffix.Length && length >= 1024; i++, length /= 1024)
            {
                dblSByte = length / 1024.0;
            }

            return String.Format("{0:0.##} {1}", dblSByte, Suffix[i]);
        }
        public static TimeSpan GetVideoDuration(string filePath)
        {
            using (var shell = ShellObject.FromParsingName(filePath))
            {
                IShellProperty prop = shell.Properties.System.Media.Duration;
                var t = (ulong)prop.ValueAsObject;
                return TimeSpan.FromTicks((long)t);
            }
        }
        public static double ConvertFramesToSeconds(int frame)
        {
            return frame / Builder.VideoFrameRate;
        }
        public static double ConvertFramesToSeconds(string frame)
        {
            return int.Parse(frame) / Builder.VideoFrameRate;
        }
        //public static void Save()
        //{
        //    //StringBuilder q = new StringBuilder();
        //    //q.Append(FolderName + "\n");
        //    //q.Append(SaveName + "\n");
        //    //q.Append(LastPathUse + "\n");

        //}
    }
}
