using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFMpegCore.FFMPEG;
using FFMpegCore;
using FFMpegCore.FFMPEG.Argument;


namespace ScriptGeneratorAVS.Classes
{
    class TrimArgument : Argument<string, string>
    {
        public TrimArgument()
        {

        }

        public TrimArgument(string first, string second) : base(first, second)
        {

        }

        public override string GetStringValue()
        {
            return " -ss "+First+ " -t "+Second ;
        }
    }
}
