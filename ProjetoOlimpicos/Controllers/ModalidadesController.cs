using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using ProjetoOlimpicos.Data;
using ProjetoOlimpicos.Filters;
using ProjetoOlimpicos.Models;

namespace ProjetoOlimpicos.Controllers
{
    [SessionAuthorize]
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
        [ValidateAntiForgeryToken]
        [SessionAuthorize(RoleAnyOf = "Admin,Gerente")]
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

        [SessionAuthorize(RoleAnyOf = "Admin,Gerente")]
        public IActionResult Editar(int id)
        {
            Modalidades mod = null;
            using (var conn = db.GetConnection())
            {
                var sql = "SELECT * FROM modalidades WHERE codModalidade = @id";
                var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", id);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        mod = new Modalidades
                        {
                            codModalidade = reader.GetInt32("codModalidade"),
                            nomeModalidade = reader.GetString("nomeModalidade")
                        };
                    }
                }
            }
            if (mod == null) return NotFound();
            return View(mod);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionAuthorize(RoleAnyOf = "Admin,Gerente")]
        public IActionResult Editar(Modalidades mod)
        {
            using (var conn = db.GetConnection())
            {
                var sql = "UPDATE modalidades SET nomeModalidade=@nome WHERE codModalidade=@id";
                var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@nome", mod.nomeModalidade);
                cmd.Parameters.AddWithValue("@id", mod.codModalidade);
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
                    var sql = "DELETE FROM modalidades WHERE codModalidade = @id";
                    var cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (MySqlException ex) when (ex.Number == 1451)
            {
                TempData["Erro"] = "Não é possível excluir esta modalidade pois existem atletas ou provas vinculadas a ela.";
            }
            return RedirectToAction("Index");
        }
    }
}
