using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using ProjetoOlimpicos.Data;
using ProjetoOlimpicos.Filters;
using ProjetoOlimpicos.Models;

namespace ProjetoOlimpicos.Controllers
{
    public class AdminController : Controller
    {
        private readonly Database db = new Database();

        private static readonly string[] Roles = new[]
        {
            "Admin",
            "Gerente",
            "Leitor"
        };


        // ============================
        // LOGIN
        // ============================

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
                    hash = r["password_hash"]?.ToString();
                    role = r["role"]?.ToString();
                    ativo = r.GetBoolean("ativo");
                }
            }


            // Validação BCrypt
            if (userId == 0 ||
                !ativo ||
                string.IsNullOrEmpty(hash) ||
                !BCrypt.Net.BCrypt.Verify(password, hash))
            {
                ModelState.AddModelError("", "Usuário ou senha inválidos.");
                return View();
            }


            // Sessão do usuário
            HttpContext.Session.SetInt32("UserId", userId);
            HttpContext.Session.SetString("Username", username);
            HttpContext.Session.SetString("Role", role ?? "Leitor");


            if (!string.IsNullOrEmpty(returnUrl) &&
                Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }


            return RedirectToAction("Index", "Home");
        }



        // ============================
        // LOGOUT
        // ============================

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }



        public IActionResult AcessoNegado()
        {
            return View();
        }



        // ============================
        // CADASTRO DE USUÁRIO
        // ============================

        [HttpGet]
        [SessionAuthorize(RoleAnyOf = "Admin,Gerente")]
        public IActionResult NovoUsuario()
        {
            return View(new Usuario());
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionAuthorize(RoleAnyOf = "Admin,Gerente")]
        public IActionResult NovoUsuario(Usuario vm)
        {

            if (string.IsNullOrWhiteSpace(vm.Username) ||
                string.IsNullOrWhiteSpace(vm.Password))
            {
                ViewBag.Erro = "Preencha usuário e senha.";
                return View(vm);
            }


            // Gera senha criptografada BCrypt
            string hash = BCrypt.Net.BCrypt.HashPassword(vm.Password);



            using (var conn = db.GetConnection())
            using (var cmd = new MySqlCommand(@"
                INSERT INTO usuarios
                (
                    username,
                    password_hash,
                    role,
                    ativo
                )
                VALUES
                (
                    @u,
                    @h,
                    @r,
                    1
                );", conn))
            {

                cmd.Parameters.AddWithValue("@u", vm.Username);
                cmd.Parameters.AddWithValue("@h", hash);
                cmd.Parameters.AddWithValue("@r", vm.Role);

                cmd.ExecuteNonQuery();
            }


            ViewBag.Sucesso = "Usuário cadastrado com sucesso!";

            return View(new Usuario());
        }



        // ============================
        // INDEX ADMIN
        // ============================

        public IActionResult Index()
        {
            return View();
        }
    }
}