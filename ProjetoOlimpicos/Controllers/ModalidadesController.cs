using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using ProjetoOlimpicos.Data;
using ProjetoOlimpicos.Filters;
using ProjetoOlimpicos.Models;

namespace ProjetoOlimpicos.Controllers
{
    public class ModalidadesController : Controller
    {
        private readonly Database db = new Database();
        public IActionResult Index()
        {
            List<Modalidades> modalidades = new List<Modalidades>();
            using (MySqlConnection conn = db.GetConnection())
            {
                string sql = "SELECT * FROM modalidades";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        modalidades.Add(new Modalidades
                        {
                            codModalidade = reader.GetInt32("codModalidade"), // oq esta em aspas pega do banco
                            nomeModalidade = reader.GetString("nomeModalidade")
                            

                        });
                    }
                }
            }
            return View(modalidades);
           
        }


        [SessionAuthorize(RoleAnyOf = "Admin,Gerente")]
        public IActionResult Criar()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Criar(Modalidades modalidade)
        {
            using (var conn = db.GetConnection())
            {
                var sql = @"INSERT INTO modalidades (nomeModalidade)
                     VALUES (@nomeModalidade)";
                var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@nomeModalidade", modalidade.nomeModalidade);
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("Index");
        }
    }
}
