using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptGeneratorAVS.Classes
{
    class Builder
    {

        private static string MainVideo;
        public static List<string> Subtitles = new List<string>();
        public static List<string> Plugins = new List<string>();
        public static List<Tuple<string,string>> EffectsFrames = new List<Tuple<string, string>>();
        public static List<string> Effects = new List<string>();
        private static double videoFrames;
        public static double VideoFrameRate;
        public static double VideoFrames { get => videoFrames; set => videoFrames = value; }

        public static void SetPlugins(string[] names)
        {
            foreach (var l in names)
                Plugins.Add(l);
        }
        public static void SetMainVideo(string url)
        {
            Builder.MainVideo = url;
        }

        public static string GetMainVideo()
        {
            return Builder.MainVideo;
        }

        public static void AddSubtitle(string url)
        {
            Subtitles.Add(url);
        }
        public static string GetSubtitles()
        {
            StringBuilder s = new StringBuilder();
            foreach (var l in Subtitles)
                s.Append(l.ToString() + "\n");
            return s.ToString();
        }
        public static void RemoveSubtitle(int n)
        {
            Subtitles.RemoveAt(n);
        }
        public static void RemoveSubtitles()
        {
            Subtitles.Clear();
        }
        public static void AddEffect(string url)
        {
            Effects.Add(url);
        }
        public static void RemoveEffect(int n)
        {
            Effects.RemoveAt(n);
        }
        public static void RemoveEffects()
        {
            Effects.Clear();
        }
        public static string Build(bool isFFmpeg)
        {
            StringBuilder s = new StringBuilder();
            string sm = "Source = Source.";
            const char qu = '"';
            if (!isFFmpeg)
            {
                foreach (var a in Plugins)
                    s.Append("LoadPlugin(" + qu + a + qu + ")" + "\n");
                s.Append("FFmpegSource2(" + qu + MainVideo + qu + ")\n");
            }
            else
                s.Append("DirectShowSource(" + qu + MainVideo + qu + ")\n");
            s.Append("Source = Last\n");
            string Ef;
            string[] z;
            for(int i = 0;i < Effects.Count;i++)
            {
                string t = Effects[i];
                z = new string[] { EffectsFrames[i].Item1, EffectsFrames[i].Item2 };
                if (Path.GetExtension(t) == ".avi")
                    Ef = "Ef"+i+" = AVISource(" +qu+ t +qu+ ",pixel_type=" + qu + "RGB32" + qu+ ")\n";
                else
                    if (Path.GetExtension(t) == ".png")
                    Ef = "Ef"+i+" = ImageReader(" + qu + t + qu + ",pixel_type="+qu+"RGB32"+qu+")\n";
                else
                    throw new System.InvalidOperationException("The File Type Isnt As Requsted.");
                s.Append(Ef);
                s.Append("Over"+i+" = Overlay(Source.trim(" + z[0] + ", " + z[1] + "), mask=Ef"+i+".showAlpha(pixel_type=" + qu + "RGB32" + qu + "),Ef"+i+")\n");
                s.Append("Source = Source.trim(0, "+z[0]+"-1"+") + Over"+i+" + Source.trim("+z[1]+"+1,0)\n");
            }

            if (!isFFmpeg)
            {
                foreach (var q in Subtitles)
                {
                    s.Append(sm + "TextSubMod(" + qu + q + qu + ")\n");
                } 
            }


            s.Append("return Source");

            return s.ToString();
        }
        /*
         * private Queue<string> InputData()
        {
            Queue<string> q = new Queue<string>();
            string s;
            char sinq = char.Parse("'")
                ,a = 'a';
            string[] Ef = new string[lbEffects.Items.Count];
            lbEffects.Items.CopyTo(Ef,0);
            List<string> Su = Builder.Subtitles;
            int i = 1;
            if (Ef.Length >0)
            {
                s = Ef[0];
                q.Enqueue("[" + i + "]setpts=PTS+" + Paths.ConvertFramesToSeconds(s.Split('-')[0]) + @"/TB[" + a + "];");
                q.Enqueue("[0:v][" + a + "]overlay=eof_action=pass[" + (++a) + "];");
                i++;
            }
            while(i<=Ef.Length)
            {
                s = Ef[i - 1];
                q.Enqueue("[" + i + "]setpts=PTS+" + Paths.ConvertFramesToSeconds(s.Split('-')[0]) +@"/TB["+(++a)+"];");
                if (i == Ef.Length && Su.Count <= 0)
                    q.Enqueue("[" + (char)(a - 1) + "][" + a + "]overlay=eof_action=pass"); 
                else
                    q.Enqueue("[" + (char)(a - 1) + "][" + a + "]overlay=eof_action=pass[" + (++a) + "];"); 
                i++;
            }
            bool isEnd;
            if (Ef.Length != 0)
                isEnd = true;
            else
                isEnd = false;
            for (i = 0; i < Su.Count; i++)
            {
                s = new Uri(Su[i]).AbsolutePath;
                if(Su.Count == 1)
                {           
                    if (isEnd)
                    {
                        q.Enqueue("[" + a + "]" + @"ass=\" + sinq + s + @"\" + sinq);
                    }
                    else
                    {
                        q.Enqueue("" + @"ass=\" + sinq + s + @"\" + sinq);
                    }
                    break;
                }
                if(isEnd)
                {
                    if (i == Su.Count - 1)
                    {
                        q.Enqueue("[" + a + "]" + @"ass=\" + sinq + s + @"\" + sinq);
                    }
                    else
                    {
                        q.Enqueue("[" + a + "]" + @"ass=\" + sinq + s + @"\" + sinq + "[" + (++a) + "];");
                    }
                }
                else 
                {
                    if (i == Su.Count - 1)
                    {
                        q.Enqueue("[" + a + "]" + @"ass=\" + sinq + s + @"\" + sinq);
                    }
                    else
                    {
                        if (i == 0)
                        {
                            q.Enqueue("[0:v]" + @"ass=\" + sinq + s + @"\" + sinq + "[" + (++a) + "];"); 
                        }
                        else
                        {
                            q.Enqueue("[" + a + "]" + @"ass=\" + sinq + s + @"\" + sinq + "[" + (++a) + "];");
                        }
                    }
                }
            }

            return q;
        }
         */
    }
}
