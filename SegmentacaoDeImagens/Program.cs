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
            try
            {
                ExecutaAlgoritmo(args);
            }
            catch (ArgumentException)
            {
                MostraAjuda();
            }
            catch (KeyNotFoundException)
            {
                Console.WriteLine("Caminho do arquivo nao foi especificado.");
                MostraAjuda();
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void ExecutaAlgoritmo(string[] args)
        {
            var dict = LeParametros(args);

            IAlgoritmo algoritmo;

            switch (dict.GetStringOrDefault("a", "k"))
            {
                case "h":
                    algoritmo = CriaAlgoritmoHough(dict);
                    break;

                case "k":
                    algoritmo = CriaAlgoritmoKMedias(dict);
                    break;

                case "c":
                    algoritmo = CriaAlgoritmoCrescimento(dict);
                    break;

                default:
                    throw new ArgumentException();
            }

            var path = dict["path"];
            var fileInfo = File.GetAttributes(path);
            IEnumerable<string> pathList;

            if (fileInfo.HasFlag(FileAttributes.Directory))
            {
                pathList = Directory.GetFiles(path);
            }
            else
            {
                pathList = new[] { path };
            }

            var outPath = dict.GetStringOrDefault("o", "output");
            var resultados = new List<ResultadoAlgoritmo>();
            Directory.CreateDirectory(outPath);

            foreach (var filename in pathList)
            {
                var img = new Imagem(filename);
                var resultado = algoritmo.Executa(img);

                resultados.Add(resultado);
                img.Save(Path.Combine(outPath, Path.GetFileName(filename)));

                resultado.Imagem.Save(Path.Combine(outPath, $"{Path.GetFileNameWithoutExtension(filename)}-{algoritmo.Sufixo}{Path.GetExtension(filename)}"));
                File.WriteAllText(
                    Path.Combine(outPath, $"{Path.GetFileNameWithoutExtension(filename)}-{algoritmo.Sufixo}.txt"),
                    $"Area: {resultado.Area}, Perímetro: {resultado.Perimetro}"
                    );
            }

            Console.WriteLine(
                $"Área: {resultados.Average(r => r.Area)}\n" +
                $"Perímetro: {resultados.Average(r => r.Perimetro)}\n" +
                $"Razão Área/Perímetro: {resultados.Average(r => r.Area) / resultados.Average(r => r.Perimetro)}");
        }

        private static IAlgoritmo CriaAlgoritmoCrescimento(Dictionary<string, string> dict) =>
            new AlgoritmoCrescimento
            {
                PorcentagemInicial = dict.GetIntOrDefault("p", 10),
                Limiar = dict.GetIntOrDefault("l", 15),
            };


        private static IAlgoritmo CriaAlgoritmoKMedias(Dictionary<string, string> dict) =>
            new AlgoritmoKMedias
            {
                IteracoesMaximas = dict.GetIntOrDefault("i", 1000),
                Epsilon = dict.GetDoubleOrDefault("e", 0.001),
                Tentativas = dict.GetIntOrDefault("t", 5),
            };

        private static IAlgoritmo CriaAlgoritmoHough(Dictionary<string, string> dict) =>
            new AlgoritmoHough
            {
                DistanciaMinima = dict.GetIntOrDefault("d", 30),
                RaioMinimo = dict.GetIntOrDefault("n", 35),
                RaioMaximo = dict.GetIntOrDefault("m", 100),
                LimiarCanny = dict.GetIntOrDefault("c", 100),
                LimiarAcumuladorCirculo = dict.GetIntOrDefault("u", 100),
            };

        private static Dictionary<string, string> LeParametros(string[] args)
        {
            int n = 0;
            var dict = new Dictionary<string, string>();

            while (n < args.Length)
            {
                var arg = args[n];
                if (arg.StartsWith("-"))
                {
                    arg = arg.Remove(0, 1);
                    dict[arg] = args[++n];
                }
                else if (!dict.ContainsKey("path"))
                {
                    dict.Add("path", arg);
                }
                else
                {
                    throw new ArgumentException();
                }
                ++n;
            }

            return dict;
        }

        static void MostraAjuda()
        {
            Console.WriteLine(
@"Uso: 
SegmentacaoDeImagens.exe entrada [-a algoritmo] [opções] [-o saída]

Onde:
    entrada: Arquivo ou diretório de entrada
    algoritmo: algoritmo a utilizar, sendo válidos os seguintes:
        h: Hough, onde as opções são pares no seguinte formato:
            -d distância mínima(padrão 30)
            -n raio mínimo(padrão 35)
            -m raio máximo(padrão 100)
            -c limiar canny(padrão 100)
            -u limiar do acumulador do círculo(padrão 100)
        c: Crescimento, onde as opções são pares no seguinte formato:
            -p porcentagem inicial(padrão 10)
            -l limiar(padrão 15)
        k: K - médias, onde as opções são pares no seguinte formato:
            -i máximo de iterações(padrão 1000)
            -e epsilon(padrão 0, 001)
            -t tentativas(padrão 5)
");
        }
    }
}
