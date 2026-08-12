using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using ProjetoOlimpicos.Data;
using ProjetoOlimpicos.Filters;
using ProjetoOlimpicos.Models;

namespace ProjetoOlimpicos.Controllers
{
    [SessionAuthorize]
    public class CidadesController : Controller
    {
        private readonly Database db = new Database();
        public IActionResult Index()
        {
            List<Cidades> cidades = new List<Cidades>();
            using (MySqlConnection conn = db.GetConnection())
            {
                string sql = "SELECT * FROM cidades";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        cidades.Add(new Cidades
                        {
                            CodCidade = reader.GetInt32("codCidade"), // oq esta em aspas pega do banco
                            NomeCidade = reader.GetString("nomeCidade"),
                            CodEstado = reader.GetInt32("codEstado"),
                            
                        });
                    }
                }
            }
            return View(cidades);
           
        }


        [SessionAuthorize(RoleAnyOf = "Admin,Gerente")]
        public IActionResult Criar()
        {
            ViewBag.Estados = GetEstados(); // Para dropdown
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionAuthorize(RoleAnyOf = "Admin,Gerente")]
        public IActionResult Criar(Cidades cidade)
        {
            using (var conn = db.GetConnection())
            {
                var sql = @"INSERT INTO cidades (nomeCidade,codEstado)
                     VALUES (@nome, @estado)";
                var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@nome", cidade.NomeCidade);
                cmd.Parameters.AddWithValue("@estado", cidade.CodEstado);
               
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("Index");
        }

        [SessionAuthorize(RoleAnyOf = "Admin,Gerente")]
        public IActionResult Editar(int id)
        {
            Cidades cid = null;
            using (var conn = db.GetConnection())
            {
                var sql = "SELECT * FROM cidades WHERE codCidade = @id";
                var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", id);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        cid = new Cidades
                        {
                            CodCidade = reader.GetInt32("codCidade"),
                            NomeCidade = reader.GetString("nomeCidade"),
                            CodEstado = reader.GetInt32("codEstado")
                        };
                    }
                }
            }
            if (cid == null) return NotFound();
            ViewBag.Estados = GetEstados();
            return View(cid);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionAuthorize(RoleAnyOf = "Admin,Gerente")]
        public IActionResult Editar(Cidades cid)
        {
            using (var conn = db.GetConnection())
            {
                var sql = "UPDATE cidades SET nomeCidade=@nome, codEstado=@estado WHERE codCidade=@id";
                var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@nome", cid.NomeCidade);
                cmd.Parameters.AddWithValue("@estado", cid.CodEstado);
                cmd.Parameters.AddWithValue("@id", cid.CodCidade);
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
                    var sql = "DELETE FROM cidades WHERE codCidade = @id";
                    var cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (MySqlException ex) when (ex.Number == 1451)
            {
                TempData["Erro"] = "Não é possível excluir esta cidade pois existem atletas vinculados a ela.";
            }
            return RedirectToAction("Index");
        }

        private List<Estados> GetEstados()
        {
            List<Estados> estados = new List<Estados>();
            using (var conn = db.GetConnection())
            {
                var sql = "SELECT Distinct * FROM estados order by nomeEstado";
                var cmd = new MySqlCommand(sql, conn);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    estados.Add(new Estados
                    {
                        CodEstado = reader.GetInt32("codEstado"),
                        NomeEstado = reader.GetString("nomeEstado"),
                    });
                }
            }
            return estados;
        }
    }
}
