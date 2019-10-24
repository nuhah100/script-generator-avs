using Google.Protobuf;
using Microsoft.Win32;
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
    [Serializable]
    [DataContract]
    static class Paths
    {
        [DataMember]
        public static string FolderName= "ScriptGenerator"
            , SaveName = "Settings.txt";
        [DataMember]
        public static string LastPathUse  = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        //public static void Save()
        //{
        //    //StringBuilder q = new StringBuilder();
        //    //q.Append(FolderName + "\n");
        //    //q.Append(SaveName + "\n");
        //    //q.Append(LastPathUse + "\n");
            
        //}
    }
}
