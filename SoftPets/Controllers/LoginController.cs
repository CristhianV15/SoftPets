using SoftPets.Models;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Mvc;
using System.Web.Security;

namespace SoftPets.Controllers
{
    public class LoginController : Controller
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["ConexionLocal"].ConnectionString;

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("UsuariosLogin", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Nombre", model.Nombre);
                    cmd.Parameters.AddWithValue("@Contrasenia", model.Contrasenia);

                    con.Open();
                    var reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        int usuarioId = (int)reader["UsuarioId"];
                        int rolId = (int)reader["RolId"];
                        int? duenioId = reader["DuenioId"] != DBNull.Value ? (int?)reader["DuenioId"] : null;
                        int? veterinarioId = reader["VeterinarioId"] != DBNull.Value ? (int?)reader["VeterinarioId"] : null;
                        char estado = Convert.ToChar(reader["Estado"]);

                        // Validar si la cuenta está inhabilitada
                        if (estado == '0')
                        {
                            ModelState.AddModelError("", "La cuenta está inhabilitada. Contacte al administrador.");
                            return View(model);
                        }

                        // Guardar en sesión
                        Session["UsuarioId"] = usuarioId;
                        Session["RolId"] = rolId;
                        Session["DuenioId"] = duenioId;
                        Session["VeterinarioId"] = veterinarioId;
                        Session["NombreUsuario"] = reader["Nombre"].ToString();

                        // Redirección según tipo de usuario
                        if (rolId == 1) // Admin
                            return RedirectToAction("Index", "Home");
                        else if (rolId == 2) // Dueño
                        {
                            if (duenioId == null)
                                return RedirectToAction("CompletarDatos", "Duenios");
                            else
                            {
                                Session["DuenioId"] = duenioId;
                                return RedirectToAction("Index", "Home");
                            }
                        }
                        else if (rolId == 3) // Veterinario
                        {
                            if (veterinarioId == null)
                                return RedirectToAction("CompletarDatos", "Veterinarios");
                            else
                                return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            ModelState.AddModelError("", "Rol de usuario no soportado.");
                            return View(model);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Usuario o contraseña inválidos.");
                        return View(model);
                    }
                }
            }
        }

        public ActionResult Logout()
        {
            Session.Clear();
            FormsAuthentication.SignOut();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult MiCuenta()
        {
            int usuarioId = (int)Session["UsuarioId"];
            Usuario model = null;
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("UsuariosSelect", con))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", usuarioId);
                con.Open();
                var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    model = new Usuario
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Nombre = reader["Nombre"].ToString(),
                        Email = reader["Email"].ToString(),
                        Contrasenia = reader["Contrasenia"].ToString(),
                        Estado = Convert.ToChar(reader["Estado"]),
                        FechaCreacion = Convert.ToDateTime(reader["FechaCreacion"]),
                        FechaActualizacion = reader["FechaActualizacion"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["FechaActualizacion"])
                    };
                }
            }
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MiCuenta(Usuario model)
        {
            // Validación de nombre único
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Usuarios WHERE Nombre=@Nombre AND Id<>@Id", con))
            {
                cmd.Parameters.AddWithValue("@Nombre", model.Nombre);
                cmd.Parameters.AddWithValue("@Id", model.Id);
                con.Open();
                int count = (int)cmd.ExecuteScalar();
                if (count > 0)
                {
                    ModelState.AddModelError("Nombre", "El nombre de usuario ya existe. Utilice otro.");
                }
            }

            if (ModelState.IsValid)
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand("UsuariosUpdateCuenta", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", model.Id);
                    cmd.Parameters.AddWithValue("@Nombre", model.Nombre);
                    cmd.Parameters.AddWithValue("@Email", model.Email);
                    cmd.Parameters.AddWithValue("@Contrasenia", model.Contrasenia);
                    cmd.Parameters.AddWithValue("@FechaActualizacion", DateTime.Now);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                TempData["CuentaActualizada"] = true;
                return RedirectToAction("MiCuenta");
            }
            return View(model);
        }
    }
}