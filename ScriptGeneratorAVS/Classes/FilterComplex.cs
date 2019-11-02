using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFMpegCore.FFMPEG.Argument;

namespace ScriptGeneratorAVS.Classes
{
    class FilterComplex : Argument<List<string>>
    {
        public FilterComplex()
        {
        }

        public FilterComplex(List<string> QueArgu) : base(QueArgu)
        {
        }

        public override string GetStringValue()
        {
            char c = '"';
            StringBuilder s = new StringBuilder();
            //if (Value.Count == 0)
            //    return string.Empty;
            foreach (var a in Value)
            {
                s.Append(a);
            }    
            return "-filter_complex "+c+ s.ToString() + c;

        }
    }
}
