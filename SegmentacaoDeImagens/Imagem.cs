using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SegmentacaoDeImagens
{
    public class Imagem : Image<Bgr, byte>
    {
        public Imagem(string filename) : base(filename) { }

        public Imagem(byte[,,] data) : base(data) { }

        public Imagem(int width, int height) : base(width, height) { }
    }
}
