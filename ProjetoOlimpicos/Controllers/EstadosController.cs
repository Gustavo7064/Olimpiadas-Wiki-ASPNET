using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using ProjetoOlimpicos.Data;
using ProjetoOlimpicos.Filters;
using ProjetoOlimpicos.Models;

namespace ProjetoOlimpicos.Controllers
{
    public class EstadosController : Controller
    {

        private readonly Database db = new Database();
        public IActionResult Index()
        {
            List<Estados> estados = new List<Estados>();
            using (MySqlConnection conn = db.GetConnection())
            {
                string sql = "SELECT * FROM estados";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        estados.Add(new Estados
                        {
                            CodEstado = reader.GetInt32("codEstado"), // oq esta em aspas pega do banco
                            NomeEstado= reader.GetString("nomeEstado"),

                        });
                    }
                }
            }
            return View(estados);
            
        }


        [SessionAuthorize(RoleAnyOf = "Admin,Gerente")]
        public IActionResult Criar()
        { 
            return View();
        }


        [HttpPost]
        public IActionResult Criar(Estados estado)
        {
            using (var conn = db.GetConnection())
            {
                var sql = @"INSERT INTO estados (nomeEstado)
                     VALUES (@nomeEstado)";
                var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@nomeEstado", estado.NomeEstado);
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("Index");
        }
    }
}
