using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SegmentacaoDeImagens
{
    public static class Extensions
    {
        public static IEnumerable<Point> Vizinhos<T>(this Image<T, byte> img, Point point) where T : struct, IColor
        {
            if (point.X > 0)
            {
                yield return new Point(point.X - 1, point.Y);
            }
            if (point.X < img.Rows - 1)
            {
                yield return new Point(point.X + 1, point.Y);
            }
            if (point.Y > 0)
            {
                yield return new Point(point.X, point.Y - 1);
            }
            if (point.Y < img.Cols - 1)
            {
                yield return new Point(point.X, point.Y + 1);
            }
        }

        public static string GetStringOrDefault(this Dictionary<string, string> dict, string key, string defaultValue) =>
            dict.TryGetValue(key, out string value) ? value : defaultValue;

        public static int GetIntOrDefault(this Dictionary<string, string> dict, string key, int defaultValue) =>
            dict.TryGetValue(key, out string value) ? 
                int.TryParse(value, out int result) ? result : throw new ArgumentException()
            : defaultValue;

        public static double GetDoubleOrDefault(this Dictionary<string, string> dict, string key, double defaultValue) =>
            dict.TryGetValue(key, out string value) ?
                double.TryParse(value, out double result) ? result : throw new ArgumentException()
            : defaultValue;
        
    }
}
