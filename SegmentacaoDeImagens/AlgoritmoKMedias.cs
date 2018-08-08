using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Emgu.CV.CvInvoke;

namespace SegmentacaoDeImagens
{
    class AlgoritmoKMedias : IAlgoritmo
    {
        #region Parametros
        public int IteracoesMaximas { get; set; } = 1000;

        public double Epsilon { get; set; } = 0.001;

        public int Tentativas { get; set; } = 5;

        public KMeansInitType KMeansInitType { get; set; } = KMeansInitType.PPCenters;
        #endregion

        public string Sufixo => "kmedias";

        public ResultadoAlgoritmo Executa(Imagem img)
        {
            var data = img.Data;

            var input = new Matrix<float>(img.Rows * img.Cols, 1, 3);
            var output = new Matrix<int>(img.Rows * img.Cols, 1);

            for (int i = 0; i < img.Rows; i++)
            {
                for (int j = 0; j < img.Cols; j++)
                {
                    input.Data[i * img.Cols + j, 0] = data[i, j, 0];
                    input.Data[i * img.Cols + j, 1] = data[i, j, 1];
                    input.Data[i * img.Cols + j, 2] = data[i, j, 2];
                }
            }

            var term = new MCvTermCriteria(IteracoesMaximas, Epsilon);
            term.Type = TermCritType.Iter | TermCritType.Eps;
            
            Kmeans(input, 2, output, term, Tentativas, KMeansInitType, null);

            var saida = new Imagem(img.Width, img.Height);
            var coresSaida = new[]
            {
                new Bgr(Color.Black),
                new Bgr(Color.Red),
            };

            for (int i = 0; i < img.Rows; i++)
            {
                for (int j = 0; j < img.Cols; j++)
                {
                    var point = new PointF(j, i);
                    var circle = new CircleF(point, 1);
                    saida.Draw(circle, coresSaida[output[i * img.Cols + j, 0]]);
                }
            }

            var corCentro = saida[saida.Rows / 2, saida.Cols / 2];
            var pixels = saida.GetPixels().Where(p => saida[p].Equals(corCentro)).ToList();

            return new ResultadoAlgoritmo
            {
                Imagem = saida,
                Area = pixels.Count,
                Perimetro = pixels.Count(p => saida.Vizinhos(p).Count(v => saida[v].Equals(corCentro)) < 4)
            };
        }
    }
}
