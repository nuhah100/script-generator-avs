using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFMpegCore.FFMPEG.Argument;

namespace ScriptGeneratorAVS.Classes
{
    class MapArgument : Argument<string>
    {
        string[] fa;
        public MapArgument(string value) : base(value)
        {

        }
        public MapArgument(string[] value)
        {
            fa = value.Clone() as string[];
        }

        public override string GetStringValue()
        {
            char c = '"';
            if(fa != null)
            {
                StringBuilder s = new StringBuilder();
                foreach(var a in fa)
                {
                    s.Append("-map " + c +  a + "?" + c+ " ");
                }
                return s.ToString();
            }
            return "-map " +c + Value +"?"+c;
        }
    }
}
