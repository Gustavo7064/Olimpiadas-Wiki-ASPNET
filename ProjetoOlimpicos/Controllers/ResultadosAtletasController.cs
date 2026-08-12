using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using ProjetoOlimpicos.Data;
using ProjetoOlimpicos.Models;
using ProjetoOlimpicos.Filters;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ProjetoOlimpicos.Controllers
{
    [SessionAuthorize]
    public class ResultadosAtletasController : Controller
    {
        private readonly Database db = new Database();

        public IActionResult Index()
        {
            List<dynamic> resultados = new List<dynamic>();
            using (var conn = db.GetConnection())
            {
                var sql = @"SELECT r.*, a.nomeAtleta, p.prova, e.ano, e.sede 
                            FROM resultadosatletas r
                            JOIN atletas a ON r.codAtleta = a.codAtleta
                            JOIN provas p ON r.codProva = p.codProva
                            JOIN edicao e ON r.edicao = e.codedicao
                            ORDER BY e.ano DESC, a.nomeAtleta";
                var cmd = new MySqlCommand(sql, conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        resultados.Add(new {
                            CodAtleta = reader.GetInt32("codAtleta"),
                            CodProva = reader.GetInt32("codProva"),
                            Edicao = reader.GetInt32("edicao"),
                            Resultado = reader.IsDBNull(reader.GetOrdinal("resultado")) ? "" : reader.GetString("resultado"),
                            Medalha = reader.IsDBNull(reader.GetOrdinal("medalha")) ? "" : reader.GetString("medalha"),
                            NomeAtleta = reader.GetString("nomeAtleta"),
                            Prova = reader.GetString("prova"),
                            AnoEdicao = reader.GetInt32("ano"),
                            SedeEdicao = reader.GetString("sede")
                        });
                    }
                }
            }
            return View(resultados);
        }

        [SessionAuthorize(RoleAnyOf = "Admin,Gerente")]
        public IActionResult Criar()
        {
            ViewBag.Atletas = GetAtletas();
            ViewBag.Provas = GetProva();
            ViewBag.Edicoes = GetEdicao();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionAuthorize(RoleAnyOf = "Admin,Gerente")]
        public IActionResult Criar(ResultadosAtletas resultadosAtletas)
        {
            using (var conn = db.GetConnection())
            {
                var sql = @"INSERT INTO resultadosatletas (codAtleta,codProva,edicao,resultado,medalha )
                             VALUES (@atleta,@prova,@edicao, @resultado, @medalha)";
                var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@atleta", resultadosAtletas.codAtleta);
                cmd.Parameters.AddWithValue("@prova", resultadosAtletas.codProva);
                cmd.Parameters.AddWithValue("@edicao", resultadosAtletas.edicao);
                cmd.Parameters.AddWithValue("@resultado", resultadosAtletas.resultado);
                cmd.Parameters.AddWithValue("@medalha", resultadosAtletas.medalha);
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("Index");
        }

        [SessionAuthorize(RoleAnyOf = "Admin,Gerente")]
        public IActionResult Editar(int codAtleta, int codProva, int edicao)
        {
            ResultadosAtletas res = null;
            using (var conn = db.GetConnection())
            {
                var sql = "SELECT * FROM resultadosatletas WHERE codAtleta = @atleta AND codProva = @prova AND edicao = @edicao";
                var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@atleta", codAtleta);
                cmd.Parameters.AddWithValue("@prova", codProva);
                cmd.Parameters.AddWithValue("@edicao", edicao);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        res = new ResultadosAtletas
                        {
                            codAtleta = reader.GetInt32("codAtleta"),
                            codProva = reader.GetInt32("codProva"),
                            edicao = reader.GetInt32("edicao"),
                            resultado = reader.IsDBNull(reader.GetOrdinal("resultado")) ? "" : reader.GetString("resultado"),
                            medalha = reader.IsDBNull(reader.GetOrdinal("medalha")) ? "" : reader.GetString("medalha")
                        };
                    }
                }
            }
            if (res == null) return NotFound();
            ViewBag.Atletas = GetAtletas();
            ViewBag.Provas = GetProva();
            ViewBag.Edicoes = GetEdicao();
            return View(res);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionAuthorize(RoleAnyOf = "Admin,Gerente")]
        public IActionResult Editar(ResultadosAtletas res)
        {
            using (var conn = db.GetConnection())
            {
                var sql = @"UPDATE resultadosatletas SET resultado=@resultado, medalha=@medalha 
                            WHERE codAtleta=@atleta AND codProva=@prova AND edicao=@edicao";
                var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@resultado", res.resultado);
                cmd.Parameters.AddWithValue("@medalha", res.medalha);
                cmd.Parameters.AddWithValue("@atleta", res.codAtleta);
                cmd.Parameters.AddWithValue("@prova", res.codProva);
                cmd.Parameters.AddWithValue("@edicao", res.edicao);
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionAuthorize(RoleAnyOf = "Admin")]
        public IActionResult Excluir(int codAtleta, int codProva, int edicao)
        {
            using (var conn = db.GetConnection())
            {
                var sql = "DELETE FROM resultadosatletas WHERE codAtleta = @atleta AND codProva = @prova AND edicao = @edicao";
                var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@atleta", codAtleta);
                cmd.Parameters.AddWithValue("@prova", codProva);
                cmd.Parameters.AddWithValue("@edicao", edicao);
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("Index");
        }

        private List<Atletas> GetAtletas()
        {
            List<Atletas> atletas = new List<Atletas>();
            using (var conn = db.GetConnection())
            {
                var sql = "SELECT codAtleta, nomeAtleta FROM atletas order by nomeAtleta";
                var cmd = new MySqlCommand(sql, conn);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    atletas.Add(new Atletas { CodAtleta = reader.GetInt32("codAtleta"), NomeAtleta = reader.GetString("nomeAtleta") });
                }
            }
            return atletas;
        }

        private List<Provas> GetProva()
        {
            List<Provas> provas = new List<Provas>();
            using (var conn = db.GetConnection())
            {
                var sql = "SELECT codProva, prova FROM provas order by prova";
                var cmd = new MySqlCommand(sql, conn);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    provas.Add(new Provas { codProva = reader.GetInt32("codProva"), prova = reader.GetString("prova") });
                }
            }
            return provas;
        }

        private List<Edicao> GetEdicao()
        {
            List<Edicao> edicoes = new List<Edicao>();
            using (var conn = db.GetConnection())
            {
                var sql = "SELECT codedicao, ano, sede FROM edicao order by ano DESC";
                var cmd = new MySqlCommand(sql, conn);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    edicoes.Add(new Edicao { Codedicao = reader.GetInt32("codedicao"), Ano = reader.GetInt32("ano"), Sede = reader.GetString("sede") });
                }
            }
            return edicoes;
        }
    }
}
