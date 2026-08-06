using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using ProjetoOlimpicos.Data;
using ProjetoOlimpicos.Models;

namespace ProjetoOlimpicos.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Database db = new Database();

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            // --- Carregar edições ---
            List<Edicao> edicoes = new List<Edicao>();
            using (var conn = db.GetConnection())
            {
                var sql = "SELECT * FROM edicao ORDER BY ano DESC";
                var cmd = new MySqlCommand(sql, conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        edicoes.Add(new Edicao
                        {
                            Codedicao = reader.GetInt32("codedicao"),
                            Ano = reader.GetInt32("ano"),
                            Sede = reader.GetString("sede")
                        });
                    }
                }
            }
            ViewBag.Edicoes = edicoes;

            // --- Carregar atletas em destaque ---
            List<Atletas> atletasDestaque = new List<Atletas>();
            using (var conn = db.GetConnection())
            {
                var sql = @"SELECT a.codAtleta, a.nomeAtleta, a.dataNascimento, a.sexo, a.altura, a.peso, a.codCidade,
                                   (SELECT m.nomeModalidade 
                                    FROM resultadosatletas r 
                                    JOIN provas p ON p.codProva = r.codProva 
                                    JOIN modalidades m ON m.codModalidade = p.codModalidade 
                                    WHERE r.codAtleta = a.codAtleta 
                                    LIMIT 1) AS modalidade
                            FROM atletas a";
                var cmd = new MySqlCommand(sql, conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        atletasDestaque.Add(new Atletas
                        {
                            CodAtleta = reader.IsDBNull(reader.GetOrdinal("codAtleta")) ? 0 : reader.GetInt32(reader.GetOrdinal("codAtleta")),
                            NomeAtleta = reader.IsDBNull(reader.GetOrdinal("nomeAtleta")) ? null : reader.GetString(reader.GetOrdinal("nomeAtleta")),
                            DataNascimento = reader.IsDBNull(reader.GetOrdinal("dataNascimento")) ? null : reader.GetString(reader.GetOrdinal("dataNascimento")),
                            Sexo = reader.IsDBNull(reader.GetOrdinal("sexo")) ? '\0' : reader.GetChar(reader.GetOrdinal("sexo")),
                            Altura = reader.IsDBNull(reader.GetOrdinal("altura")) ? null : reader.GetDecimal(reader.GetOrdinal("altura")),
                            Peso = reader.IsDBNull(reader.GetOrdinal("peso")) ? null : reader.GetDecimal(reader.GetOrdinal("peso")),
                            CodCidade = reader.IsDBNull(reader.GetOrdinal("codCidade")) ? 0 : reader.GetInt32(reader.GetOrdinal("codCidade")),
                            Modalidade = reader.IsDBNull(reader.GetOrdinal("modalidade")) ? null : reader.GetString(reader.GetOrdinal("modalidade"))
                        });
                    }
                }
            }
            ViewBag.AtletasDestaque = atletasDestaque;

            // --- Carregar estatísticas globais ---
            var estatisticas = new Dictionary<string, int>();
            using (var conn = db.GetConnection())
            {
                // Total de edições
                var cmd1 = new MySqlCommand("SELECT COUNT(*) FROM edicao", conn);
                estatisticas["edicoes"] = Convert.ToInt32(cmd1.ExecuteScalar());

                // Total de atletas
                var cmd2 = new MySqlCommand("SELECT COUNT(*) FROM atletas", conn);
                estatisticas["atletas"] = Convert.ToInt32(cmd2.ExecuteScalar());

                // Total de provas
                var cmd3 = new MySqlCommand("SELECT COUNT(*) FROM provas", conn);
                estatisticas["provas"] = Convert.ToInt32(cmd3.ExecuteScalar());
            }
            ViewBag.Estatisticas = estatisticas;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
