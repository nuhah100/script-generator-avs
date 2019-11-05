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
    class BestCodecArgument : Argument<bool>
    {
        public BestCodecArgument()
        {

        }

        public BestCodecArgument(bool value) : base(value)
        {
        }

        public override string GetStringValue()
        {
            if (Value)
            {
                return "-c:v libx265";
            }
            else
                return "-c:v vp9";
        }
    }
}
