﻿using Emgu.CV;
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
            if (point.X < img.Cols - 1)
            {
                yield return new Point(point.X + 1, point.Y);
            }
            if (point.Y > 0)
            {
                yield return new Point(point.X, point.Y - 1);
            }
            if (point.Y < img.Rows - 1)
            {
                yield return new Point(point.X, point.Y + 1);
            }
        }

        public static IEnumerable<Point> GetPixels(this Imagem img)
        {
            var data = img.Data;
            for (int i = 0; i < img.Rows; i++)
            {
                for (int j = 0; j < img.Cols; j++)
                {
                    yield return new Point(j, i);
                }
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
