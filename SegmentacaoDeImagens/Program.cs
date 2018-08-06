using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Emgu.CV.CvInvoke;
using static System.Math;

namespace SegmentacaoDeImagens
{
    class Program
    {
        static void Main(string[] args)
        {
            var dict = LeParametros(args);


        }

        private static Dictionary<string, string> LeParametros(string[] args)
        {
            int n = 0;
            var dict = new Dictionary<string, string>();

            while (n < args.Length)
            {
                var arg = args[n++];
                if (arg.StartsWith("-"))
                {
                    arg = arg.Remove(0, 1);
                    dict[arg] = args[n++];
                }
                else
                {
                    throw new ArgumentException();
                }
            }

            return dict;
        }
    }
}
