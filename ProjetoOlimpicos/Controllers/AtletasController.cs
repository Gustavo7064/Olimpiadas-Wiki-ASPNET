using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using ProjetoOlimpicos.Data;
using ProjetoOlimpicos.Models;
using ProjetoOlimpicos.Filters;

namespace ProjetoOlimpicos.Controllers
{
    [SessionAuthorize]
    public class AtletasController : Controller
    {
        private readonly Database db = new Database();
       
        
        public IActionResult Index()
        {
            List<Atletas> at = new List<Atletas>();
            using (MySqlConnection conn = db.GetConnection())
            {
                // A tabela atletas não possui codModalidade, então buscamos os dados básicos
                // e a cidade para a listagem.
                string sql = @"SELECT a.*, c.nomeCidade 
                             FROM atletas a 
                             LEFT JOIN cidades c ON a.codCidade = c.codCidade";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        at.Add(new Atletas
                        {
                            CodAtleta = reader.IsDBNull(reader.GetOrdinal("CodAtleta")) ? 0 : reader.GetInt32(reader.GetOrdinal("CodAtleta")),
                            NomeAtleta = reader.IsDBNull(reader.GetOrdinal("nomeAtleta")) ? null : reader.GetString(reader.GetOrdinal("nomeAtleta")),
                            DataNascimento = reader.IsDBNull(reader.GetOrdinal("dataNascimento")) ? null : reader.GetString(reader.GetOrdinal("dataNascimento")),
                            Sexo = reader.IsDBNull(reader.GetOrdinal("sexo")) ? ' ' : reader.GetChar(reader.GetOrdinal("sexo")),
                            Altura = reader.IsDBNull(reader.GetOrdinal("altura")) ? null : reader.GetDecimal(reader.GetOrdinal("altura")),
                            Peso = reader.IsDBNull(reader.GetOrdinal("peso")) ? null : reader.GetDecimal(reader.GetOrdinal("peso")),
                            CodCidade = reader.IsDBNull(reader.GetOrdinal("codCidade")) ? 0 : reader.GetInt32(reader.GetOrdinal("codCidade")),
                            CidadeNascimento = reader.IsDBNull(reader.GetOrdinal("nomeCidade")) ? "" : reader.GetString("nomeCidade")
                        });
                    }
                }
                return View(at);
            }
        }

        public IActionResult Detalhes(int id)
        {
            Atletas atleta = null;
            List<(string Prova, string Edicao, string Resultado, string Medalha)> participacoes = new();
            using (var conn = db.GetConnection())
            {
                // LEFT JOIN em tudo para garantir que o atleta apareça mesmo sem resultados
                string query = @"
                    SELECT a.*, c.nomeCidade, e.nomeEstado
                    FROM atletas a
                    LEFT JOIN cidades c ON a.codCidade = c.codCidade
                    LEFT JOIN estados e ON c.codEstado = e.codEstado
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
                            DataNascimento = reader.IsDBNull(reader.GetOrdinal("dataNascimento")) ? null : reader.GetString("dataNascimento"),
                            Sexo = reader.GetChar("sexo"),
                            Altura = reader.IsDBNull(reader.GetOrdinal("altura")) ? null : reader.GetDecimal("altura"),
                            Peso = reader.IsDBNull(reader.GetOrdinal("peso")) ? null : reader.GetDecimal("peso"),
                            CidadeNascimento = reader.IsDBNull(reader.GetOrdinal("nomeCidade")) ? "" : reader.GetString("nomeCidade"),
                            EstadoNascimento = reader.IsDBNull(reader.GetOrdinal("nomeEstado")) ? "" : reader.GetString("nomeEstado")
                        };
                    }
                }
                if (atleta == null) return NotFound();
                string participacaoQuery = @"
                    SELECT p.Prova, ed.ano, ed.sede, r.resultado, r.medalha
                    FROM resultadosatletas r
                    JOIN provas p ON r.codProva = p.codProva
                    JOIN edicao ed ON r.edicao = ed.codedicao
                    WHERE r.codAtleta = @id";
                var cmd2 = new MySqlCommand(participacaoQuery, conn);
                cmd2.Parameters.AddWithValue("@id", id);
                using (var reader = cmd2.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        participacoes.Add((
                            reader.GetString("Prova"),
                            $"{reader.GetInt32("ano")} - {reader.GetString("sede")}",
                            reader.IsDBNull(reader.GetOrdinal("resultado")) ? "-" : reader.GetString("resultado"),
                            reader.IsDBNull(reader.GetOrdinal("medalha")) ? "-" : reader.GetString("medalha")
                        ));
                    }
                }
            }
            ViewBag.Participacoes = participacoes;
            return View(atleta);
        }




        [SessionAuthorize(RoleAnyOf ="Admin,Gerente")]
        public IActionResult Criar()
        {
            ViewBag.Cidades = GetCidades(); // Para dropdown
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionAuthorize(RoleAnyOf = "Admin,Gerente")]
        public IActionResult Criar(Atletas atleta)
        {
            using (var conn = db.GetConnection())
            {
                var sql = @"INSERT INTO atletas (nomeAtleta, dataNascimento, sexo, altura, peso, codCidade)
                     VALUES (@nome, @data, @sexo, @altura, @peso, @cidade)";
                var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@nome", atleta.NomeAtleta);
                cmd.Parameters.AddWithValue("@data", atleta.DataNascimento);
                cmd.Parameters.AddWithValue("@sexo", atleta.Sexo);
                cmd.Parameters.AddWithValue("@altura", atleta.Altura);
                cmd.Parameters.AddWithValue("@peso", atleta.Peso);
                cmd.Parameters.AddWithValue("@cidade", atleta.CodCidade);
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("Index");
        }

        [SessionAuthorize(RoleAnyOf = "Admin,Gerente")]
        public IActionResult Editar(int id)
        {
            Atletas atleta = null;
            using (var conn = db.GetConnection())
            {
                var sql = "SELECT * FROM atletas WHERE codAtleta = @id";
                var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", id);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        atleta = new Atletas
                        {
                            CodAtleta = reader.GetInt32("codAtleta"),
                            NomeAtleta = reader.GetString("nomeAtleta"),
                            DataNascimento = reader.IsDBNull(reader.GetOrdinal("dataNascimento")) ? null : reader.GetString("dataNascimento"),
                            Sexo = reader.GetChar("sexo"),
                            Altura = reader.IsDBNull(reader.GetOrdinal("altura")) ? null : reader.GetDecimal("altura"),
                            Peso = reader.IsDBNull(reader.GetOrdinal("peso")) ? null : reader.GetDecimal("peso"),
                            CodCidade = reader.GetInt32("codCidade")
                        };
                    }
                }
            }
            if (atleta == null) return NotFound();
            ViewBag.Cidades = GetCidades();
            return View(atleta);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionAuthorize(RoleAnyOf = "Admin,Gerente")]
        public IActionResult Editar(Atletas atleta)
        {
            using (var conn = db.GetConnection())
            {
                var sql = @"UPDATE atletas SET nomeAtleta=@nome, dataNascimento=@data, sexo=@sexo, 
                            altura=@altura, peso=@peso, codCidade=@cidade WHERE codAtleta=@id";
                var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@nome", atleta.NomeAtleta);
                cmd.Parameters.AddWithValue("@data", atleta.DataNascimento);
                cmd.Parameters.AddWithValue("@sexo", atleta.Sexo);
                cmd.Parameters.AddWithValue("@altura", atleta.Altura);
                cmd.Parameters.AddWithValue("@peso", atleta.Peso);
                cmd.Parameters.AddWithValue("@cidade", atleta.CodCidade);
                cmd.Parameters.AddWithValue("@id", atleta.CodAtleta);
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
                    var sql = "DELETE FROM atletas WHERE codAtleta = @id";
                    var cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (MySqlException ex) when (ex.Number == 1451)
            {
                TempData["Erro"] = "Não é possível excluir este atleta pois ele possui resultados vinculados.";
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
    }
}
