using System.Data;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using ProjetoOlimpicos.Data;
using ProjetoOlimpicos.Models;
using ProjetoOlimpicos.Filters;

namespace ProjetoOlimpicos.Controllers
{
    
    public class AdminController : Controller
    {
        private readonly Database db = new Database();
        private static readonly string[] Roles = new[] { "Admin", "Gerente", "Leitor" };

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string username, string password, string? returnUrl = null)
        {
            int userId = 0;
            string? hash = null;
            string? role = null;
            bool ativo = false;

            using (var conn = db.GetConnection())
            using (var cmd = new MySqlCommand(@"
            SELECT id, password_hash, role, ativo
            FROM usuarios
            WHERE username = @u
            LIMIT 1;", conn))
            {
                cmd.Parameters.AddWithValue("@u", username);
                using var r = cmd.ExecuteReader();
                if (r.Read())
                {
                    userId = r.GetInt32("id");
                    hash = r["password_hash"] as string;
                    role = r["role"] as string;
                    ativo = r.GetBoolean("ativo");
                }
            }

            if (userId == 0 || !ativo || string.IsNullOrEmpty(hash) || !BCrypt.Net.BCrypt.Verify(password, hash))
            {
                ModelState.AddModelError("", "Usuário ou senha inválidos.");
                return View();
            }

            // Grava MÍNIMO necessário na sessão
            HttpContext.Session.SetInt32("UserId", userId);
            HttpContext.Session.SetString("Username", username);
            HttpContext.Session.SetString("Role", role ?? "Leitor");

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        public IActionResult AcessoNegado() { 
             return View();
        }



        
        // GET: /Admin/NovoUsuario
        [HttpGet]
        [SessionAuthorize(RoleAnyOf = "Admin,Gerente")]
        public IActionResult NovoUsuario()
        {
            return View(new Usuario());
        }

        // POST: /Admin/NovoUsuario
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionAuthorize(RoleAnyOf = "Admin")]
        public IActionResult NovoUsuario(Usuario vm)
        {
            if (string.IsNullOrWhiteSpace(vm.Username) || string.IsNullOrWhiteSpace(vm.Password))
            {
                ViewBag.Erro = "Preencha usuário e senha.";
                return View(vm);
            }

            var hash = BCrypt.Net.BCrypt.HashPassword(vm.Password);

            using (var conn = db.GetConnection())
            using (var cmd = new MySqlCommand(@"
            INSERT INTO usuarios (username, password_hash, role, ativo)
            VALUES (@u, @h, @r, 1);", conn))
            {
                cmd.Parameters.AddWithValue("@u", vm.Username);
                cmd.Parameters.AddWithValue("@h", hash);
                cmd.Parameters.AddWithValue("@r", vm.Role);

                cmd.ExecuteNonQuery();
            }

            ViewBag.Sucesso = "Usuário cadastrado com sucesso!";
            return View(new Usuario());
        }
        [SessionAuthorize(RoleAnyOf = "Admin")]
        public IActionResult Index()
        {
            List<Usuario> usuarios = new List<Usuario>();
            using (var conn = db.GetConnection())
            {
                var sql = "SELECT id, username, role, ativo FROM usuarios";
                var cmd = new MySqlCommand(sql, conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        usuarios.Add(new Usuario
                        {
                            Id = reader.GetInt32("id"),
                            Username = reader.GetString("username"),
                            Role = reader.GetString("role"),
                            Ativo = reader.GetBoolean("ativo")
                        });
                    }
                }
            }
            return View(usuarios);
        }

        [SessionAuthorize(RoleAnyOf = "Admin")]
        public IActionResult EditarUsuario(int id)
        {
            Usuario user = null;
            using (var conn = db.GetConnection())
            {
                var sql = "SELECT id, username, role, ativo FROM usuarios WHERE id = @id";
                var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", id);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        user = new Usuario
                        {
                            Id = reader.GetInt32("id"),
                            Username = reader.GetString("username"),
                            Role = reader.GetString("role"),
                            Ativo = reader.GetBoolean("ativo")
                        };
                    }
                }
            }
            if (user == null) return RedirectToAction("Index");
            ViewBag.Roles = Roles;
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionAuthorize(RoleAnyOf = "Admin")]
        public IActionResult EditarUsuario(Usuario user, string? newPassword)
        {
            using (var conn = db.GetConnection())
            {
                string sql;
                if (!string.IsNullOrWhiteSpace(newPassword))
                {
                    var hash = BCrypt.Net.BCrypt.HashPassword(newPassword);
                    sql = "UPDATE usuarios SET username = @u, role = @r, ativo = @a, password_hash = @h WHERE id = @id";
                    var cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@u", user.Username);
                    cmd.Parameters.AddWithValue("@r", user.Role);
                    cmd.Parameters.AddWithValue("@a", user.Ativo);
                    cmd.Parameters.AddWithValue("@h", hash);
                    cmd.Parameters.AddWithValue("@id", user.Id);
                    cmd.ExecuteNonQuery();
                }
                else
                {
                    sql = "UPDATE usuarios SET username = @u, role = @r, ativo = @a WHERE id = @id";
                    var cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@u", user.Username);
                    cmd.Parameters.AddWithValue("@r", user.Role);
                    cmd.Parameters.AddWithValue("@a", user.Ativo);
                    cmd.Parameters.AddWithValue("@id", user.Id);
                    cmd.ExecuteNonQuery();
                }
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionAuthorize(RoleAnyOf = "Admin")]
        public IActionResult ExcluirUsuario(int id)
        {
            // Evitar que o admin se exclua a si mesmo
            if (id == HttpContext.Session.GetInt32("UserId"))
            {
                return RedirectToAction("Index");
            }

            using (var conn = db.GetConnection())
            {
                var sql = "DELETE FROM usuarios WHERE id = @id";
                var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("Index");
        }

        // GET: /Admin/Registrar
        [HttpGet]
        public IActionResult Registrar()
        {
            return View();
        }

        // POST: /Admin/Registrar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Registrar(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Erro = "Preencha todos os campos.";
                return View();
            }

            try
            {
                using (var conn = db.GetConnection())
                {
                    // Verificar se usuário já existe
                    var checkSql = "SELECT COUNT(*) FROM usuarios WHERE username = @u";
                    var checkCmd = new MySqlCommand(checkSql, conn);
                    checkCmd.Parameters.AddWithValue("@u", username);
                    if (Convert.ToInt32(checkCmd.ExecuteScalar()) > 0)
                    {
                        ViewBag.Erro = "Este nome de usuário já está em uso.";
                        return View();
                    }

                    var hash = BCrypt.Net.BCrypt.HashPassword(password);
                    var sql = @"INSERT INTO usuarios (username, password_hash, role, ativo)
                                VALUES (@u, @h, 'Leitor', 1);";
                    var cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@u", username);
                    cmd.Parameters.AddWithValue("@h", hash);
                    cmd.ExecuteNonQuery();
                }

                return RedirectToAction("Login");
            }
            catch (System.Exception)
            {
                ViewBag.Erro = "Ocorreu um erro ao criar a conta. Por favor, tente novamente.";
                return View();
            }
        }
    }
}

    



