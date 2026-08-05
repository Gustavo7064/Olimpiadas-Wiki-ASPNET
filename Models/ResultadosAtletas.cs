namespace ProjetoOlimpicos.Models
{
    public class ResultadosAtletas
    {
        public int codAtletaRes { get; set; }
        public int codAtleta { get; set; } // chave estrangeira
        public int codProva { get; set; } //chave estrangeira
        public int edicao { get; set; } // chave estrangeira
        public string resultado { get; set; }
        public string medalha { get; set; }
    }
}
