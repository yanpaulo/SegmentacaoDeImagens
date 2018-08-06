using Emgu.CV.Structure;
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

        public Imagem Executa(Imagem entrada)
        {
            var gray = entrada.Convert<Gray, byte>();

            var size = entrada.Size;
            var centro = new Point(size.Width / 2, size.Height / 2);

            var seed = new Rectangle(
                    new Point(centro.X - size.Width * PorcentagemInicial / 2 / 100, centro.Y - size.Height * PorcentagemInicial / 2 / 100),
                    new Size(size.Width * PorcentagemInicial / 100, size.Height * PorcentagemInicial / 100));

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

            var data = gray.Data;

            while (queue.Any())
            {
                var proximo = queue.Dequeue();
                var vizinhos = entrada.Vizinhos(proximo);

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

            var saida = new Imagem(map);
            return saida;
        }
    }
}
