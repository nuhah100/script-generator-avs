using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFMpegCore;
using FFMpegCore.FFMPEG.Argument;

namespace ScriptGeneratorAVS.Classes
{
    class SubArgument : Argument<string>
    {
        public SubArgument()
        {
        }

        public SubArgument(string value) : base(value)
        {

        }

        public override string GetStringValue()
        {
            char c = '"', f = (char)(39); 
            string t = @Value;
            return " -filter:v " + c+@"ass=\"+f+t+@"\"+f+c;
        }
    }
}
