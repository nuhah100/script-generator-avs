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
    class BestCodecArgument : Argument
    {
        public BestCodecArgument()
        {

        }


        public override string GetStringValue()
        {
            return "-c:v libx265";
        }
    }
}
