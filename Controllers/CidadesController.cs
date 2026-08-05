using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using ProjetoOlimpicos.Data;
using ProjetoOlimpicos.Filters;
using ProjetoOlimpicos.Models;

namespace ProjetoOlimpicos.Controllers
{
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
