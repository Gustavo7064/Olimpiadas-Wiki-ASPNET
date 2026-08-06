using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using ProjetoOlimpicos.Data;
using ProjetoOlimpicos.Filters;
using ProjetoOlimpicos.Models;

namespace ProjetoOlimpicos.Controllers
{
    [SessionAuthorize]
    public class ProvasController : Controller
    {
        private readonly Database db = new Database();

        public IActionResult Index()
        {
            List<Provas> pr= new List<Provas>();
            using (MySqlConnection conn = db.GetConnection())
            {
                string sql = "SELECT * FROM provas";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        pr.Add(new Provas
                        {
                            codProva = reader.GetInt32("codProva"), // oq esta em aspas pega do banco
                            prova = reader.GetString("prova"),
                            codModalidade = reader.GetInt32("codModalidade"),

                        });
                    }
                }
            }
            return View(pr);
        }

        [SessionAuthorize(RoleAnyOf = "Admin,Gerente")]
        public IActionResult Criar()
        {
            ViewBag.Modalidades = GetModalidades(); // Para dropdown
            return View();
        }

        [HttpPost]
        [SessionAuthorize(RoleAnyOf = "Admin,Gerente")]
        public IActionResult Criar(Provas pr)
        {
            using (var conn = db.GetConnection())
            {
                var sql = @"INSERT INTO provas (prova,codModalidade)
                     VALUES (@prova, @modalidade)";
                var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@prova", pr.prova);
                cmd.Parameters.AddWithValue("@modalidade", pr.codModalidade);

                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("Index");
        }

                [SessionAuthorize(RoleAnyOf = "Admin,Gerente")]
        public IActionResult Editar(int id)
        {
            Provas prova = null;
            using (var conn = db.GetConnection())
            {
                var sql = "SELECT * FROM provas WHERE codProva = @id";
                var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", id);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        prova = new Provas
                        {
                            codProva = reader.GetInt32("codProva"),
                            prova = reader.GetString("prova"),
                            codModalidade = reader.GetInt32("codModalidade")
                        };
                    }
                }
            }
            if (prova == null) return NotFound();
            ViewBag.Modalidades = GetModalidades();
            return View(prova);
        }

        [HttpPost]
        [SessionAuthorize(RoleAnyOf = "Admin,Gerente")]
        public IActionResult Editar(Provas pr)
        {
            using (var conn = db.GetConnection())
            {
                var sql = "UPDATE provas SET prova=@prova, codModalidade=@modalidade WHERE codProva=@id";
                var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@prova", pr.prova);
                cmd.Parameters.AddWithValue("@modalidade", pr.codModalidade);
                cmd.Parameters.AddWithValue("@id", pr.codProva);
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("Index");
        }

        [SessionAuthorize(RoleAnyOf = "Admin")]
        public IActionResult Excluir(int id)
        {
            using (var conn = db.GetConnection())
            {
                var sql = "DELETE FROM provas WHERE codProva = @id";
                var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("Index");
        }

        private List<Modalidades> GetModalidades()
        {
            List<Modalidades> Modalidades = new List<Modalidades>();
            using (var conn = db.GetConnection())
            {
                var sql = "SELECT Distinct * FROM modalidades order by nomeModalidade";
                var cmd = new MySqlCommand(sql, conn);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Modalidades.Add(new Modalidades
                    {
                        codModalidade = reader.GetInt32("codModalidade"),
                        nomeModalidade = reader.GetString("nomeModalidade"),
                    });
                }
            }
            return Modalidades;
        }
    }
}
