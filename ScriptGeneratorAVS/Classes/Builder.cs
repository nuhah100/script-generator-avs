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
        private static bool Sound;
        private static List<string> Subtitles = new List<string>();
        private static List<string> Plugins = new List<string>();
        private static List<string> Effects = new List<string>();


        public static void SetSound(bool b)
        {
            Sound = b;
        }
        public static bool GetSound()
        {
            return Sound;
        }
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
        public static string Build()
        {
            StringBuilder s = new StringBuilder();
            string sm = "Source = Source.";
            const char qu = '"';
            foreach (var a in Plugins)
                s.Append("LoadPlugin(" + qu + a + qu + ")" + "\n");
            if (Sound)
                s.Append("FFmpegSource2(" + qu + MainVideo + qu + ")\n");
            else
                s.Append("FFVideoSource(" + qu + MainVideo + qu + ")\n");
            s.Append("Source = Last\n");
            string Fn,Ef;
            string[] z;
            foreach (var t in Effects)
            {
                Fn = Path.GetFileNameWithoutExtension(t);
                z = Fn.Split(new char[] {'-','|'});
                if (Path.GetExtension(t) == ".avi")
                    Ef = "Ef = AVISource(" + t + ")\n";
                else
                    if (Path.GetExtension(t) == ".png")
                    Ef = "Ef = ImageReader(" + qu + t + qu + ",pixel_type="+qu+"RGB32"+qu+"))\n";
                else
                    throw new System.InvalidOperationException("The File Type Isnt As Requsted.");
                s.Append(Ef);
                s.Append("Over = Overlay(Source.trim(" + z[0] + ", " + z[1] + "), mask=Ef.showAlpha(pixel_type=" + qu + "RGB32" + qu + "),Ef)\n");
                s.Append("Source = Source.trim(0, "+z[0]+"-1"+") + Over + Source.trim("+z[1]+"+1,0)\n");
            }

            foreach (var q in Subtitles)
            {
                s.Append(sm + "TextSubMod(" + qu + q + qu + ")\n");
            }


            s.Append("return Source");


            return s.ToString();
        }
    }
}
