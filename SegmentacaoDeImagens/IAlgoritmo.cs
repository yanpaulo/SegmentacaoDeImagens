namespace SegmentacaoDeImagens
{
    interface IAlgoritmo
    {
        Imagem Entrada { get; set; }

        Imagem Executa();
    }
}