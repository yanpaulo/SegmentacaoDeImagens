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
        }

        private static void Crescimento(string filename = @"C:\Users\yanpa\source\repos\SegmentacaoDeImagens\SegmentacaoDeImagens\bin\Debug\images\3026_lg.jpg")
        {
            const int Limiar = 15;
            const int Canal = 2;
            var img = new Image<Bgr, byte>(filename);

            var percent = 10;
            var size = img.Size;
            var centro = new Point(size.Width / 2, size.Height / 2);

            var seed = new Rectangle(
                    new Point(centro.X - size.Width * percent / 2 / 100, centro.Y - size.Height * percent / 2 / 100),
                    new Size(size.Width * percent / 100, size.Height * percent / 100));

            var gray = img.Convert<Gray, byte>();
            var map = new byte[size.Width, size.Height, 3];
            var queue = new Queue<Point>();

            for (int i = seed.Left; i <= seed.Right; i++)
            {
                for (int j = seed.Top; j <= seed.Bottom; j++)
                {
                    map[i, j, Canal] = 255;
                    queue.Enqueue(new Point(i, j));
                }
            }

            //Instrução fora do while pois gray.Data tem complexidade significativa - O(1).
            var data = gray.Data;

            while (queue.Any())
            {
                var proximo = queue.Dequeue();
                var vizinhos = img.Vizinhos(proximo);

                var pixel = data[proximo.X, proximo.Y, 0];

                foreach (var v in vizinhos)
                {
                    if (map[v.X, v.Y, Canal] != 255)
                    {
                        var diff = Math.Abs(pixel - data[v.X, v.Y, 0]);
                        if (diff <= Limiar)
                        {
                            queue.Enqueue(v);
                            map[v.X, v.Y, Canal] = 255;
                        }
                    }
                }
            }

            var dest = new Image<Bgr, byte>(map);
            dest.Save("dest.jpg");
        }

        #region Hough
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
                var minDist = 30;
                var minRadius = 35;
                var cannyThreshold = 100;
                var circleAccumulatorThreshold = 100;

                IEnumerable<CircleF> circles;

                do
                {
                    circles =
                        HoughCircles(uimage, HoughType.Gradient, 2.0, minDist, cannyThreshold, circleAccumulatorThreshold, minRadius, maxRadius: 100)
                        .Where(c => !OverflowsImage(c, img));
                    circleAccumulatorThreshold -= 20;
                } while (!circles.Any());

                circles = circles.OrderByDescending(c => c.Radius).Take(2);

                if (Math.Abs(circles.FirstOrDefault().Radius - circles.LastOrDefault().Radius) > 5)
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
        #endregion
    }
}
