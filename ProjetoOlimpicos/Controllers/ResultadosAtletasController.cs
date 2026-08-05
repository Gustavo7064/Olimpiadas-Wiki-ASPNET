using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using ProjetoOlimpicos.Data;
using ProjetoOlimpicos.Models;
using ProjetoOlimpicos.Filters;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering; // Adicionado para SelectListItem

namespace ProjetoOlimpicos.Controllers
{
    public class ResultadosAtletasController : Controller
    {
        private readonly Database db = new Database();

        public IActionResult Index()
        {
            return View();
        }

        [SessionAuthorize(RoleAnyOf = "Admin,Gerente")]

        public IActionResult Criar()
        {
            ViewBag.Atletas = GetAtletas(); // Para dropdown
            ViewBag.Provas = GetProva(); // Para dropdown
            ViewBag.Edicoes = GetEdicao(); // Para dropdown
            return View();
        }

        [HttpPost]
        public IActionResult Criar(ResultadosAtletas resultadosAtletas)
        {
            using (var conn = db.GetConnection())
            {
                var sql = @"INSERT INTO resultadosatletas (codAtleta,codProva,edicao,resultado,medalha )
                             VALUES (@atleta,@prova,@edicao, @resultado, @medalha)";
                var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@atleta", resultadosAtletas.codAtleta);
                cmd.Parameters.AddWithValue("@prova", resultadosAtletas.codProva); // Corrigido para @prova
                cmd.Parameters.AddWithValue("@edicao", resultadosAtletas.edicao);
                cmd.Parameters.AddWithValue("@resultado", resultadosAtletas.resultado);
                cmd.Parameters.AddWithValue("@medalha", resultadosAtletas.medalha);
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("Index");
        }

        private List<Atletas> GetAtletas()
        {
            List<Atletas> atletas = new List<Atletas>();
            using (var conn = db.GetConnection())
            {
                var sql = "SELECT Distinct * FROM atletas order by nomeAtleta";
                var cmd = new MySqlCommand(sql, conn);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    atletas.Add(new Atletas
                    {
                        CodAtleta = reader.GetInt32("codAtleta"),
                        NomeAtleta = reader.GetString("nomeAtleta"),
                    });
                }
            }
            return atletas;
        }

        private List<Provas> GetProva()
        {
            List<Provas> provas = new List<Provas>();
            using (var conn = db.GetConnection())
            {
                var sql = "SELECT Distinct * FROM provas order by prova";
                var cmd = new MySqlCommand(sql, conn);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    provas.Add(new Provas
                    {
                        codProva = reader.GetInt32("codProva"),
                        prova = reader.GetString("prova")
                    });
                }
            }
            return provas;
        }

        private List<Edicao> GetEdicao()
        {
            List<Edicao> edicoes = new List<Edicao>();
            using (var conn = db.GetConnection())
            {
                var sql = "SELECT Distinct * FROM edicao order by ano";
                var cmd = new MySqlCommand(sql, conn);
                var reader = cmd.ExecuteReader();
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
            return edicoes;
        }
    }
}
