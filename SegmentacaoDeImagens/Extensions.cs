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
            if (point.X < img.Size.Width - 1)
            {
                yield return new Point(point.X + 1, point.Y);
            }
            if (point.Y > 0)
            {
                yield return new Point(point.X, point.Y - 1);
            }
            if (point.Y < img.Size.Height - 1)
            {
                yield return new Point(point.X, point.Y + 1);
            }
        }
    }
}
