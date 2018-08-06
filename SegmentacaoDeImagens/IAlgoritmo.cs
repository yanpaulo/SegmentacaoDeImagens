namespace SegmentacaoDeImagens
{
    interface IAlgoritmo
    {
        string Sufixo { get; }

        Imagem Executa(Imagem entrada);
    }
}