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

        public Imagem Entrada { get; set; }

        public Imagem Executa()
        {
            var img = Entrada;
            var data = img.Data;

            var input = new Matrix<float>(img.Width * img.Height, 1, 3);
            var output = new Matrix<int>(img.Rows * img.Cols, 1);

            for (int i = 0; i < img.Height; i++)
            {
                for (int j = 0; j < img.Width; j++)
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
                    var point = new PointF(i, j);
                    var circle = new CircleF(point, 1);
                    saida.Draw(circle, coresSaida[output[i * img.Cols + j, 0]]);
                }
            }

            return saida;
        }
    }
}
