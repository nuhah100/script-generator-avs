using Microsoft.Win32;
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
using System.Diagnostics;
using CG.Web.MegaApiClient;
using ScriptGeneratorAVS.Resources;
using System.Threading;

namespace ScriptGeneratorAVS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        FFMpeg encoder;
        System.Windows.Threading.DispatcherTimer tt = new System.Windows.Threading.DispatcherTimer();
        public System.Windows.Threading.DispatcherTimer Upt = new System.Windows.Threading.DispatcherTimer();
        string a,V;
        ArgumentContainer container = new ArgumentContainer();// FFMpegOptions.Configure(new FFMpegOptions { RootDirectory = @"C:\Users\moish\source\repos\ScriptGeneratorAVS\ScriptGeneratorAVS\FFMPEG\bin\" });
        private CancellationTokenSource uploadCancellationTokenSource = new CancellationTokenSource();
        MegaApiClient client = new MegaApiClient();
        Task<INode> td;
        public MainWindow()
        {
            string y = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)));
            string ff = y + @"\FFMPEG\bin\";
            string[] files = Directory.GetFiles(y, "*.exe", SearchOption.AllDirectories);
            List<string> ls = new List<string>();
            foreach (var item in files)
            {
                if (Path.GetFileName(item).Equals("ffmpeg.exe"))
                    ff = Path.GetDirectoryName(item);

            }
            try 
            {
                //FFMpegOptions.Configure(new FFMpegOptions { RootDirectory = y +@"\FFMPEG\bin\" });
                FFMpegOptions.Configure(new FFMpegOptions { RootDirectory = ff });
                encoder = new FFMpeg();

            }
            catch(FFMpegCore.FFMPEG.Exceptions.FFMpegException)
            {
                FFMpegOptions.Configure(new FFMpegOptions { RootDirectory =Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\FFMPEG\bin\" });
                encoder = new FFMpeg();
            }
            Console.WriteLine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\FFMPEG\bin\");
            InitializeComponent();
            files = Directory.GetFiles(y, "*.dll", SearchOption.AllDirectories);
            ls = new List<string>();
            foreach(string s in files)
            {
                string ss = Path.GetFileName(s);
                if (ss.Equals("ffms2.dll") || ss.Equals("ImageSeq.dll") || ss.Equals("VSFilterMod.dll"))
                    ls.Add(s);
            }
            //Builder.SetPlugins(new string[]{
            //y+@"\FFMPEG\Plugins\ffms2.dll",
            //y+@"\FFMPEG\Plugins\ImageSeq.dll",
            //y+@"\FFMPEG\Plugins\VSFilterMod.dll"
            //});
            Builder.SetPlugins(ls.ToArray());
        }


        private void BtnFindVideo_Click(object sender, RoutedEventArgs e)
        {
            try
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
                        for (int i = 0; i < r.Length - 1; i++)
                        {
                            a.Append(r[i] + "\n");
                        }
                        a.Append("Size: " + (Paths.ConvertToSize(new FileInfo(url).Length)));
                        lblVideoDetails.Content = a.ToString();
                    }
                }
            }
            catch(Exception es)
            {
                MessageBox.Show(es.Message,es.Source,MessageBoxButton.OK,MessageBoxImage.Error);
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
                string FileNameNoPath;
                foreach (string filename in f.FileNames)
                {
                    FileNameNoPath = System.IO.Path.GetFileName(filename);
                    lbEffects.Items.Add(FileNameNoPath);
                    Builder.AddEffect(filename);
                    //Frames w = new Frames();
                    FileNameNoPath = Path.GetFileNameWithoutExtension(FileNameNoPath);
                    if(FileNameNoPath.IndexOf('_') != -1)
                    {
                        FileNameNoPath = FileNameNoPath.Split('_')[1];
                    }
                    if (FileNameNoPath.IndexOf("-") == -1)
                    {
                        Tuple<string, string> a;
                        do
                        {
                            a = Frames.GetFrames(System.IO.Path.GetFileName(filename));
                        }
                        while (a == null);
                        Builder.EffectsFrames.Add(a);
                    }
                    else
                    {
                        Builder.EffectsFrames.Add(new Tuple<string, string>(FileNameNoPath.Split('-')[0], FileNameNoPath.Split('-')[1]));
                    }
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

        private void btnEffectRemove_Click(object sender, RoutedEventArgs e)
        {
            Builder.RemoveEffect(lbEffects.SelectedIndex);
            Builder.EffectsFrames.RemoveAt(lbEffects.SelectedIndex);
            lbEffects.Items.RemoveAt(lbEffects.SelectedIndex);
        }

        private void btnEffectsClear_Click(object sender, RoutedEventArgs e)
        {
            Builder.RemoveEffects();
            lbEffects.Items.Clear();
        }


        private void BtnBuild_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtVideoUrl.Text))
            {
                MessageBox.Show("You Must Set a Video!", "Warning", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            SaveFileDialog f = new SaveFileDialog
            {
                Filter = "AVS Files (*.avs)| *.avs",
                DefaultExt = "avs",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };
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
            try
            {
                if (string.IsNullOrEmpty(txtVideoUrl.Text))
                {
                    MessageBox.Show("You Must Set a Video!", "Warning", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                container.Clear();
                var ran = new Random();
                a = DateTime.Now.ToString("MM-dd-hh-mm");
                File.WriteAllText(Path.GetTempPath() + "Script" + a + ".avs", Builder.Build(false));

                SaveFileDialog s = new SaveFileDialog
                {
                    Filter = "Video Files(*.mp4/*.mkv)|*.mp4;*.mkv",
                    DefaultExt = '.' + cbFormat.Text.ToLower()
                };
                if (s.ShowDialog() == false)
                {
                    MessageBox.Show("You must choose a path.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                container.Add(new InputArgument(new string[] { Builder.GetMainVideo(), Path.GetTempPath() + "Script" + a + ".avs" }));
                container.Add(new ThreadsArgument(true));
                container.Add(new LogArgument(Path.GetTempPath() + @"log" + a + ".txt"));

                
                string u = cbEncoder.Text.ToLower();
                switch (u)
                {
                    case "libx264":
                        {
                            container.Add(new VideoCodecArgument(VideoCodec.LibX264));
                            break;
                        }
                    case "libx265":
                        {
                            container.Add(new BestCodecArgument(true));
                            break;
                        }
                    case "vp9":
                        {
                            container.Add(new BestCodecArgument(false));
                            break;
                        }
                }
                u = cbSpeed.Text.Replace(" ", "");
                SpeedArgument sa = new SpeedArgument();
                switch (u)
                {
                    case "VerySlow":
                        {
                            sa.Value = Speed.VerySlow;
                            break;
                        }
                    case "Slower":
                        {
                            sa.Value = Speed.Slower;
                            break;
                        }
                    case "Slow":
                        {
                            sa.Value = Speed.Slow;
                            break;
                        }
                    case "Medium":
                        {
                            sa.Value = Speed.Medium;
                            break;
                        }
                    case "Fast":
                        {
                            sa.Value = Speed.Fast;
                            break;
                        }
                    case "Faster":
                        {
                            sa.Value = Speed.Faster;
                            break;
                        }
                    case "VeryFast":
                        {
                            sa.Value = Speed.VeryFast;
                            break;
                        }
                    case "SuperFast":
                        {
                            sa.Value = Speed.SuperFast;
                            break;
                        }
                    case "UltraFast":
                        {
                            sa.Value = Speed.UltraFast;
                            break;
                        }
                }
                container.Add(sa);
                AudioCodecArgument ac = new AudioCodecArgument();
                switch (cbAudioCodec.Text.Replace(" ", "").ToLower())
                {
                    case "aac":
                        {
                            ac.Value = AudioCodec.Aac;
                            break;
                        }
                    default:
                        {
                            ac.Value = AudioCodec.LibVorbis;
                            break;
                        }
                }
                container.Add(ac);
                container.Add(new MapArgument(new string[] { "1:v:0", "0:a:0" }));
                container.Add(new OutputArgument(new Uri(s.FileName)));
                container.Add(new OverrideArgument());
                V = s.FileName;
                Task t = Task.Run(() =>
                {
                    try
                    {
                        encoder.Convert(container);
                    }
                    catch(FFMpegCore.FFMPEG.Exceptions.FFMpegException er)
                    {
                        MessageBox.Show(er.Message+"\n"+er.Source+"\n"+er.Type, er.InnerException?.Message,MessageBoxButton.OK,MessageBoxImage.Error,MessageBoxResult.OK);
                    }
                });
                
                tt.Interval = TimeSpan.FromMilliseconds(400);
                tt.Tick += Tt_Tick;
                tt.Start();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private List<string> InputData()
        {
            List<string> q = new List<string>();
            string s;
            char sinq = char.Parse("'")
                , a = 'a';
            List<string> Su = Builder.Subtitles;
            if (Su.Count > 1)
            {
                for (int i = 0; i < Su.Count; i++)
                {
                    s = Su[i].Replace(@"\\", @"\").Replace('\\','/').Replace(":",@"\:");
                    if (i == 0)
                        q.Add(@"ass=" + sinq + s  + sinq + "[" + (++a) + "];");
                    else
                        if (i == Su.Count - 1)
                        q.Add("[" + a + "]" + @"ass=" + sinq + s + sinq);
                    else
                        q.Add("[" + a + "]" + @"ass=" + sinq + s + sinq + "[" + (++a) + "];");

                } 
            }
            else
                q.Add(@"ass=\" + sinq + Su[0].Replace(@"\\", @"\").Replace('\\', '/').Replace(":", @"\:") + @"\" + sinq);
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
                string ls = q.ToString();
                if (string.IsNullOrEmpty(ls))
                    return;
                string[] ss = new string[12],
                    y = ls.Split(new char[] { '\n' });
                Array.Reverse(y);
                for (int ii = 0; ii < ss.Length; ii++)
                {
                    ss[ii] = y[12 - ii];
                    
                }
                double d = (100 * double.Parse(ss[0].Split('=')[1].ToString())) / Builder.VideoFrames;
                bool isUpload = false;
                d = Math.Round(d * 10000) / 10000d;
                if (d >= 100)
                    d = 99.999;
                if (ss[11].Split('=')[1] == "end")
                {
                    tt.Stop();
                    StopEncoder();
                    Process.Start("explorer.exe", "/select," + V + @"\");
                    d = 100;
                    File.Delete(Path.GetTempPath() + "log" + a + ".txt");
                    File.Delete(Path.GetTempPath() + "Script" + a + ".avs");
                    string yi = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)));
                    string pa = @"\FFMPEG\doc\";
                    string n = "Saves.txt";
                    string c = yi + pa + n;
                    if (File.Exists(c))
                    {
                        if (cbUpload.IsChecked == true)
                        {
                            isUpload = true;
                        }
                    }
                    else
                    {
                        MessageBox.Show("You didnt set mega account", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                lblVideoInfo.Content = d + "%" + " (Encoding)";
                prbPro.Value = d;
                Thread.Sleep(200);
                if(isUpload)
                {
                    Upload();
                }
            }
        }
 

        private void btnStopEncode_Click(object sender, RoutedEventArgs e)
        {
            StopEncoder();
        }

        private void MenuItem_Upload(object sender, RoutedEventArgs e)
        {
            Upload u = new Upload();
            u.Show();
        }
        private void Upload()
        {
            string yi = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)));
            string pa = @"\FFMPEG\doc\";
            string n = "Saves.txt";
            string c = yi + pa + n;
            if (File.Exists(c))
            {
                string[] lines = File.ReadAllLines(c);
                
                client.Login(lines[0], lines[1]);
                Progress<double> ze = new Progress<double>(p =>
                {
                    Console.WriteLine($"Progress updated: {p}");
                    double d = Math.Round(p * 10000) / 10000d;
                    lblVideoInfo.Content = d + "%" + " (Upload)";
                    prbPro.Value = d;

                });
               

                IEnumerable<INode> nodes = client.GetNodes();

                INode root = nodes.Single(x => x.Type == NodeType.Root);
                INode myFolder = client.CreateFolder("Upload-s" + a, root);

                td = Task.Run(() =>
                {
                    return client.UploadFileAsync(V, myFolder, ze);
                });

                Upt.Interval = TimeSpan.FromMilliseconds(30);
                Upt.Tick += Upt_Tick;
                Upt.Start();
            }
        }

        private void Upt_Tick(object sender, EventArgs e)
        {
            if (td.IsCompleted)
            {
                INode m = td.Result;
                Uri downloadLink = client.GetDownloadLink(m);
                Clipboard.SetText(downloadLink.ToString());
                Console.WriteLine(downloadLink.ToString());
                System.Diagnostics.Process.Start(downloadLink.ToString());
                MessageBox.Show("Upload Complete!\n" + downloadLink.ToString() + "\nCopied to Clipboard!", "Finished!", MessageBoxButton.OK, MessageBoxImage.Information);

                client.Logout();
                Upt.Stop();
            }
        }

        private void StopEncoder()
        {
            encoder.Stop();
            encoder.Kill();
        }
        //public uploadFileData UploadToMegaAsync(string Userrrr,string Passss,  string filePathOnComputer, string newFileNameOnMega)
        //{
        //    //Implemnt Struct
        //    uploadFileData myMegaFileData = new uploadFileData();

        //    //Start Mega Cient
        //    var myMegaClient = new MegaApiClient();

            
        //    //Login To Mega
        //    myMegaClient.Login(Userrrr, Passss);


        //    IEnumerable<INode> nodes = myMegaClient.GetNodes();

        //    INode root = nodes.Single(x => x.Type == NodeType.Root);
        //    INode myFolder = myMegaClient.CreateFolder("Upload", root);
        //    ////Get All (File & Folders) in Mega Account
        //    //IEnumerable<INode> nodes = myMegaClient.GetNodes();

        //    ////Creat List Of All Folders In Mega Account
        //    //List<INode> megaFolders = nodes.Where(n => n.Type == NodeType.Directory).ToList();

        //    ////Choose Exist Folder In Mega Account By Name & Id
        //    //INode myFolderOnMega = megaFolders.Where(folder => folder.Name == megaFolderName && folder.Id == megaFolderID).FirstOrDefault();

        //    //Upload The File
        //    //Normal Upload
        //    //INode myFile = myMegaClient.UploadFile(filePathOnComputer, myFolderOnMega);

        //    //NEWLY ADDED
        //    var progress = new Progress<double>();
        //    progress.ProgressChanged += (s, progressValue) =>
        //    {
        //        //Update the UI (or whatever) with the progressValue 
        //        int d = Convert.ToInt32(progressValue);
        //        lblVideoInfo.Content = d + "%";
        //        prbPro.Value = d /100;
        //    };

        //    //NEWLY ADDED
        //    if (uploadCancellationTokenSource.IsCancellationRequested)
        //    {
        //        uploadCancellationTokenSource.Dispose();
        //        uploadCancellationTokenSource = new CancellationTokenSource();
        //    }

        //    // Upload With progress bar
        //    Task<INode> td = Task.Run(() =>
        //            {
        //                return myMegaClient.UploadFileAsync(filePathOnComputer, myFolder, progress, uploadCancellationTokenSource.Token);
        //            }
        //    );
            
        //    INode myFile = td.Result;
        //    //Rename The File In Mega Server
        //    if (string.IsNullOrEmpty(newFileNameOnMega))
        //    {

        //    }
        //    else
        //    {
        //        myMegaClient.Rename(myFile, newFileNameOnMega);
        //    }

        //    //Get Download Link
        //    Uri downloadLink = myMegaClient.GetDownloadLink(myFile);

        //    myMegaFileData.megaFileId = myFile.Id;
        //    Clipboard.SetText(myMegaFileData.megaFileId);
        //    myMegaFileData.megaFileType = myFile.Type.ToString();
        //    myMegaFileData.megaFileName = myFile.Name;
        //    myMegaFileData.megaFileOwner = myFile.Owner;
        //    myMegaFileData.megaFileParentId = myFile.ParentId;
        //    myMegaFileData.megaFileCreationDate = myFile.CreationDate.ToString();
        //    myMegaFileData.megaFileModificationDate = myFile.ModificationDate.ToString();
        //    myMegaFileData.megaFileSize = myFile.Size.ToString();
        //    myMegaFileData.megaFileDownloadLink = downloadLink.ToString();

        //    myMegaClient.Logout();

        //    return myMegaFileData;
        //}
        //private void StopUpload(object sender, EventArgs e)
        //{
        //    if (!uploadCancellationTokenSource.IsCancellationRequested)
        //        uploadCancellationTokenSource.Cancel();
        //}

    }
}
