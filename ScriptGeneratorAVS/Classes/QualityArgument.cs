using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFMpegCore.FFMPEG.Argument;

namespace ScriptGeneratorAVS.Classes
{
    class QualityArgument : Argument<double>
    {
        public QualityArgument(double value) : base(value)
        {
        }

        public override string GetStringValue()
        {
            return "-crf " + Value + " ";
        }
    }
}
