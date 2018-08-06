using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SegmentacaoDeImagens
{
    class AlgoritmoHough : IAlgoritmo
    {
        #region Parametros
        public int DistanciaMinima { get; set; } = 30;

        public int RaioMinimo { get; set; } = 35;

        public int RaioMaximo { get; set; } = 100; 

        public int LimiarCanny { get; set; } = 100;

        public int LimiarAcumuladorCirculo { get; set; } = 100;
        #endregion

        public string Sufixo => "hough";

        public ResultadoAlgoritmo Executa(Imagem entrada)
        {
            var img = entrada.Convert<Gray, byte>();
            

            var circulos =
                    img
                    .HoughCircles(new Gray(LimiarCanny), new Gray(LimiarAcumuladorCirculo), 2.0, DistanciaMinima, RaioMinimo, RaioMaximo)
                    .SelectMany(c => c)
                    .OrderByDescending(c => c.Radius)
                    .Take(2);

            if (Math.Abs(circulos.FirstOrDefault().Radius - circulos.LastOrDefault().Radius) > 5)
            {
                circulos = circulos.Take(1);
            }

            var saida = new Imagem(entrada.Data);

            foreach (var c in circulos)
            {
                var centro = new CircleF(c.Center, 5);

                saida.Draw(c, new Bgr(Color.Red));
                saida.Draw(centro, new Bgr(Color.Red));
            }

            return new ResultadoAlgoritmo
            {
                Imagem = saida,
                Area = circulos.Sum(c => c.Area),
                Perimetro = circulos.Sum(c => 2 * Math.PI * c.Radius)
            };
        }
    }
}
