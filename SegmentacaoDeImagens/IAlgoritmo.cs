namespace SegmentacaoDeImagens
{
    interface IAlgoritmo
    {
        string Sufixo { get; }

        ResultadoAlgoritmo Executa(Imagem entrada);
    }
}