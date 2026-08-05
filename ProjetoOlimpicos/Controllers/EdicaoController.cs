using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using ProjetoOlimpicos.Data;
using ProjetoOlimpicos.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using ProjetoOlimpicos.Filters;

namespace ProjetoOlimpicos.Controllers
{

    [SessionAuthorize]  
    public class EdicaoController : Controller
    {
        private readonly Database db = new Database();
        public IActionResult Index()
        {
            List<Edicao> edicoes = new List<Edicao>();
            using (MySqlConnection conn = db.GetConnection())
            {   
                string sql = "SELECT * FROM edicao";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        edicoes.Add(new Edicao
                        {
                            Codedicao = reader.GetInt32("codedicao"), // oq esta em aspas pega do banco
                            Ano = reader.GetInt32("ano"),
                            Sede = reader.GetString("sede")
                        });
                    }
                }
            }
            return View(edicoes);

        }


        [SessionAuthorize(RoleAnyOf = "Admin,Gerente")]
        public IActionResult Criar()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Criar(Edicao edicao)
        {
            using (var conn = db.GetConnection())
            {
                var sql = @"INSERT INTO edicao (ano,sede)
                     VALUES (@ano, @sede)";
                var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ano", edicao.Ano);
                cmd.Parameters.AddWithValue("@sede", edicao.Sede);
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("Index");
        }

        private List<Cidades> GetCidades()
        {
            List<Cidades> cidades = new List<Cidades>();
            using (var conn = db.GetConnection())
            {
                var sql = "SELECT Distinct * FROM cidades order by nomeCidade";
                var cmd = new MySqlCommand(sql, conn);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    cidades.Add(new Cidades
                    {
                        CodCidade = reader.GetInt32("codCidade"),
                        NomeCidade = reader.GetString("nomeCidade"),
                        CodEstado = reader.GetInt32("codEstado")
                    });
                }
            }
            return cidades;
        }

        public IActionResult Atletas(int id)
        {
            List<Atletas> atletas = new List<Atletas>();
            int totalAtletas = 0;

            using (MySqlConnection conn = db.GetConnection())
            {
                MySqlCommand cmd = new MySqlCommand("sp_GetAtletasByEdicao", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("p_edicao", id);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        atletas.Add(new Atletas
                        {
                            CodAtleta = reader.GetInt32(reader.GetOrdinal("codAtleta")),
                            NomeAtleta = reader.IsDBNull(reader.GetOrdinal("nomeAtleta"))
                                ? null
                                : reader.GetString(reader.GetOrdinal("nomeAtleta")),
                            DataNascimento = reader.IsDBNull(reader.GetOrdinal("dataNascimento"))
                                ? null
                                : reader.GetString(reader.GetOrdinal("dataNascimento")),
                            Sexo = reader.IsDBNull(reader.GetOrdinal("sexo"))
                                ? '\0'
                                : reader.GetChar(reader.GetOrdinal("sexo")),
                            CodCidade = reader.IsDBNull(reader.GetOrdinal("codCidade"))
                                ? 0
                                : reader.GetInt32(reader.GetOrdinal("codCidade")),
                            CodModalidade = reader.IsDBNull(reader.GetOrdinal("codModalidade"))
                                ? 0
                                : reader.GetInt32(reader.GetOrdinal("codModalidade")),
                            Modalidade = reader.IsDBNull(reader.GetOrdinal("nomeModalidade"))
                                ? null
                                : reader.GetString(reader.GetOrdinal("nomeModalidade"))
                        });
                    }
                }

                totalAtletas = atletas.Count;
            }

            ViewBag.EdicaoId = id;
            ViewBag.TotalAtletas = totalAtletas;
            return View(atletas);
        }


        public IActionResult Detalhes(int id)
        {
            Atletas atleta = null;
            List<(string Prova, string Edicao, string Resultado, string Medalha)> participacoes = new();

            using (var conn = db.GetConnection())
            {
                string query = @"
         SELECT 
             a.codAtleta,a.nomeAtleta,a.dataNascimento,a.sexo,c.codCidade, c.nomeCidade,e.nomeEstado,
             m.codModalidade, m.nomeModalidade,p.Prova,r.resultado,r.medalha 
                 FROM atletas a
                 JOIN cidades c ON c.codCidade = a.codCidade
                 JOIN estados e ON e.codEstado = c.codEstado
                 JOIN resultadosatletas r ON r.codAtleta = a.codAtleta
                 JOIN provas p ON p.codProva = r.codProva
                 JOIN modalidades m ON m.codModalidade = p.codModalidade
                 WHERE a.codAtleta = @id";

                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        atleta = new Atletas
                        {
                            CodAtleta = reader.GetInt32("codAtleta"),
                            NomeAtleta = reader.GetString("nomeAtleta"),
                            DataNascimento = reader.GetString("dataNascimento"),
                            Sexo = reader.GetChar("sexo"),
                            CidadeNascimento = reader.GetString("nomeCidade"),
                            CodModalidade = reader.GetInt32("codModalidade"),
                            Modalidade = reader.GetString("nomeModalidade"),
                            EstadoNascimento = reader.GetString("nomeEstado"),
                            CodCidade = reader.GetInt32("codCidade")
                        };
                    }
                }

                // Buscar participações
                string participacaoQuery = @"
     SELECT p.Prova, e.ano, e.sede, r.resultado, r.medalha
     FROM resultadosatletas r
     JOIN provas p ON p.codProva = r.codProva
     JOIN edicao e ON e.codedicao = r.edicao
     WHERE r.codAtleta = @id";

                var cmd2 = new MySqlCommand(participacaoQuery, conn);
                cmd2.Parameters.AddWithValue("@id", id);
                using (var reader = cmd2.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        participacoes.Add((
                            reader.IsDBNull(reader.GetOrdinal("Prova"))
                                ? null
                                : reader.GetString(reader.GetOrdinal("Prova")),

                            $"{(reader.IsDBNull(reader.GetOrdinal("ano"))
                                ? "?"
                                : reader.GetInt32(reader.GetOrdinal("ano")).ToString())} - {(reader.IsDBNull(reader.GetOrdinal("sede"))
                                ? "?"
                                : reader.GetString(reader.GetOrdinal("sede")))}",

                            reader.IsDBNull(reader.GetOrdinal("resultado"))
                                ? null
                                : reader.GetString(reader.GetOrdinal("resultado")),

                            reader.IsDBNull(reader.GetOrdinal("medalha"))
                                ? null
                                : reader.GetString(reader.GetOrdinal("medalha"))
                        ));
                    }

                }
            }

            ViewBag.Participacoes = participacoes;
            return View(atleta);
        }


    }
}
 

    
