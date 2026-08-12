using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using ProjetoOlimpicos.Data;
using ProjetoOlimpicos.Filters;
using ProjetoOlimpicos.Models;

namespace ProjetoOlimpicos.Controllers
{
    [SessionAuthorize]
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
        [ValidateAntiForgeryToken]
        [SessionAuthorize(RoleAnyOf = "Admin,Gerente")]
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

        [SessionAuthorize(RoleAnyOf = "Admin,Gerente")]
        public IActionResult Editar(int id)
        {
            Estados est = null;
            using (var conn = db.GetConnection())
            {
                var sql = "SELECT * FROM estados WHERE codEstado = @id";
                var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", id);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        est = new Estados
                        {
                            CodEstado = reader.GetInt32("codEstado"),
                            NomeEstado = reader.GetString("nomeEstado")
                        };
                    }
                }
            }
            if (est == null) return NotFound();
            return View(est);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionAuthorize(RoleAnyOf = "Admin,Gerente")]
        public IActionResult Editar(Estados est)
        {
            using (var conn = db.GetConnection())
            {
                var sql = "UPDATE estados SET nomeEstado=@nome WHERE codEstado=@id";
                var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@nome", est.NomeEstado);
                cmd.Parameters.AddWithValue("@id", est.CodEstado);
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionAuthorize(RoleAnyOf = "Admin")]
        public IActionResult Excluir(int id)
        {
            try
            {
                using (var conn = db.GetConnection())
                {
                    var sql = "DELETE FROM estados WHERE codEstado = @id";
                    var cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (MySqlException ex) when (ex.Number == 1451)
            {
                TempData["Erro"] = "Não é possível excluir este estado pois existem cidades vinculadas a ele.";
            }
            return RedirectToAction("Index");
        }
    }
}
