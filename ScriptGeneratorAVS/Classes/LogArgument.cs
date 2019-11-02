using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFMpegCore.FFMPEG.Argument;
namespace ScriptGeneratorAVS.Classes
{
    class LogArgument : Argument<string>
    {
        public LogArgument()
        {
        }

        public LogArgument(string value) : base(value)
        {
        }

        public override string GetStringValue()
        {
            char c = '"';
            return " -progress "+c+@Value+c+" ";
        }
    }
}
