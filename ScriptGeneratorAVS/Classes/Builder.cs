using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptGeneratorAVS.Classes
{
    class Builder
    {

        private static string MainVideo;
        private static List<string> Subtitles = new List<string>();
        private static List<string> Plugins = new List<string>();

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
        public static string Build()
        {
            return null;
        }
    }
}
