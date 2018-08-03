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

namespace SegmentacaoDeImagens
{
    class Program
    {
        static void Main(string[] args)
        {
            HoughCirclesDetect();
        }

        private static void HoughCirclesDetect()
        {
            const string path = "images";
            Directory.CreateDirectory(path);

            var files = Directory.GetFiles(path).Where(f => !f.Contains("-circle"));

            foreach (var filename in files)
            {
                var img = new Image<Bgr, byte>(filename);

                UMat uimage = new UMat();
                CvtColor(img, uimage, ColorConversion.Bgr2Gray);
                double minDist = 30;
                double cannyThreshold = 100;
                double circleAccumulatorThreshold = 100;

                IEnumerable<CircleF> circles =
                    HoughCircles(uimage, HoughType.Gradient, 2.0, minDist, cannyThreshold, circleAccumulatorThreshold, minRadius: 30, maxRadius: 100)
                    .Where(c => !OverflowsImage(c, img));

                if (!circles.Any())
                {
                    circleAccumulatorThreshold = 75;
                    circles =
                        HoughCircles(uimage, HoughType.Gradient, 2.0, minDist, cannyThreshold, circleAccumulatorThreshold, minRadius: 30, maxRadius: 100)
                        .Where(c => !OverflowsImage(c, img));
                }

                circles = circles.OrderByDescending(c => c.Radius).Take(2);

                if (Math.Abs(circles.First().Radius - circles.Last().Radius) > 5)
                {
                    circles = circles.Take(1);
                }


                var @new = img.Copy();

                foreach (var c in circles)
                {
                    @new.Draw(c, new Bgr(Color.Red));
                }
                var dest = $"{path}\\{Path.GetFileNameWithoutExtension(filename)}-circle{Path.GetExtension(filename)}";
                @new.Save(dest);

            }
        }

        static bool OverflowsImage(CircleF circle, Image<Bgr, byte> image) =>
            circle.Center.X + circle.Radius > image.Width ||
            circle.Center.X - circle.Radius < 0 ||
            circle.Center.Y + circle.Radius > image.Height ||
            circle.Center.Y - circle.Radius < 0;
    }
}
