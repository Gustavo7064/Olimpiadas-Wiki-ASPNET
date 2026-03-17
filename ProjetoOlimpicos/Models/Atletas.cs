namespace ProjetoOlimpicos.Models
{
    public class Atletas
    {
        public int CodAtleta { get; set; }
        public string NomeAtleta { get; set; }
        public string DataNascimento { get; set; }
        public char Sexo { get; set; }
        public decimal? Altura { get; set; }
        public decimal? Peso { get; set; }
        public int CodCidade { get; set; }
        public int CodModalidade { get; set; }
        public string Modalidade { get; set; }
        public string CidadeNascimento { get; set; }
        public string EstadoNascimento { get; set; }
    }
}
