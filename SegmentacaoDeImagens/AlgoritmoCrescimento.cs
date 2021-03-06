﻿using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SegmentacaoDeImagens
{
    class AlgoritmoCrescimento : IAlgoritmo
    {
        private const int Canal = 2;

        #region Parametros
        public int PorcentagemInicial { get; set; } = 10;

        public int Limiar { get; set; } = 15;
        #endregion

        public string Sufixo => "cresc";

        public ResultadoAlgoritmo Executa(Imagem entrada)
        {
            var gray = entrada.Convert<Gray, byte>();

            var size = entrada.Size;
            var centro = new Point(size.Width / 2, size.Height / 2);

            var seed = new Rectangle(
                    new Point(centro.X - size.Width * PorcentagemInicial / 2 / 100, centro.Y - size.Height * PorcentagemInicial / 2 / 100),
                    new Size(size.Width * PorcentagemInicial / 100, size.Height * PorcentagemInicial / 100));

            var map = new byte[entrada.Rows, entrada.Cols, 3];
            var queue = new Queue<Point>();

            for (int i = seed.Left; i <= seed.Right; i++)
            {
                for (int j = seed.Top; j <= seed.Bottom; j++)
                {
                    map[j, i, Canal] = 255;
                    queue.Enqueue(new Point(i, j));
                }
            }

            var data = gray.Data;

            while (queue.Any())
            {
                var proximo = queue.Dequeue();
                var vizinhos = entrada.Vizinhos(proximo);

                var pixel = data[proximo.Y, proximo.X, 0];

                foreach (var v in vizinhos)
                {
                    if (map[v.Y, v.X, Canal] != 255)
                    {
                        var diff = Math.Abs(pixel - data[v.Y, v.X, 0]);
                        if (diff <= Limiar)
                        {
                            queue.Enqueue(v);
                            map[v.Y, v.X, Canal] = 255;
                        }
                    }
                }
            }

            var saida = new Imagem(map);
            var corArea = Canal == 2 ? new Bgr(0, 0, 255) : throw new InvalidOperationException();
            var pixels = saida.GetPixels().Where(p => saida[p].Equals(corArea)).ToList();
            
            return new ResultadoAlgoritmo
            {
                Imagem = saida,
                Area = pixels.Count,
                Perimetro = pixels.Count(p => saida.Vizinhos(p).Count(v => saida[v].Equals(corArea)) < 4)
            };
        }
    }
}
