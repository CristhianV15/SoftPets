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
                                Session["DuenioId"] = duenioId; // <-- ¡IMPORTANTE!
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
    }
}