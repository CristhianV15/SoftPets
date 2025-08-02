using SoftPets.Filters;
using SoftPets.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftPets.Controllers
{

    public class UsuariosController : Controller
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["ConexionLocal"].ConnectionString;

        // Index
        public ActionResult Index()
        {
            List<Usuario> usuarios = new List<Usuario>();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("UsuariosSelect", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", DBNull.Value);

                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        usuarios.Add(new Usuario
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Nombre = reader["Nombre"].ToString(),
                            Email = reader["Email"].ToString(),
                            Contrasenia = reader["Contrasenia"].ToString(),
                            RolNombre = reader["RolNombre"].ToString(),
                            Estado = Convert.ToChar(reader["Estado"]),
                            FechaCreacion = Convert.ToDateTime(reader["FechaCreacion"]),
                            FechaActualizacion = reader["FechaActualizacion"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["FechaActualizacion"])
                        });
                    }
                }
            }
            return View(usuarios);
        }

        // Details
        public ActionResult Details(int id)
        {
            Usuario usuario = null;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("UsuariosSelect", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", id);

                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        usuario = new Usuario
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Nombre = reader["Nombre"].ToString(),
                            Email = reader["Email"].ToString(),
                            Contrasenia = reader["Contrasenia"].ToString(),
                            RolNombre = reader["RolNombre"].ToString(),
                            Estado = Convert.ToChar(reader["Estado"]),
                            FechaCreacion = Convert.ToDateTime(reader["FechaCreacion"]),
                            FechaActualizacion = reader["FechaActualizacion"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["FechaActualizacion"])
                        };
                    }
                }
            }
            if (usuario == null) return HttpNotFound();
            return View(usuario);
        }

        // Create
        public ActionResult Create()
        {
            //Enviar Roles a la vista
            ViewBag.Roles = ObtenerRolesActivos();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Usuario model)
        {
            if (ModelState.IsValid)
            {
                // Validar si el nombre de usuario ya existe
                bool existe = false;
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("UsuariosExisteNombre", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Nombre", model.Nombre);
                        con.Open();
                        var result = cmd.ExecuteScalar();
                        existe = Convert.ToInt32(result) > 0;
                    }
                }
                if (existe)
                {
                    ModelState.AddModelError("Nombre", "El nombre de usuario ya existe. Elija otro.");
                    ViewBag.Roles = ObtenerRolesActivos();
                    return View(model);
                }

                // Insertar usuario si no existe
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("UsuariosInsert", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Nombre", model.Nombre);
                        cmd.Parameters.AddWithValue("@Email", model.Email);
                        cmd.Parameters.AddWithValue("@Contrasenia", model.Contrasenia);
                        cmd.Parameters.AddWithValue("@RolId", model.RolId);
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                return RedirectToAction("Index");
            }
            ViewBag.Roles = ObtenerRolesActivos();
            return View(model);
        }

        // Edit
        public ActionResult Edit(int id)
        {
            Usuario usuario = null;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("UsuariosSelect", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", id);
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        usuario = new Usuario
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Nombre = reader["Nombre"].ToString(),
                            Email = reader["Email"].ToString(),
                            Contrasenia = reader["Contrasenia"].ToString(),
                            RolNombre = reader["RolNombre"].ToString(),
                            RolId = Convert.ToInt32(reader["Rol"]), // Asegúrate de convertir a int
                            Estado = Convert.ToChar(reader["Estado"]),
                            FechaCreacion = Convert.ToDateTime(reader["FechaCreacion"]),
                            FechaActualizacion = reader["FechaActualizacion"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["FechaActualizacion"])
                        };
                    }
                }
            }
            if (usuario == null) return HttpNotFound();
            ViewBag.Roles = ObtenerRolesActivos();

            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Usuario model)
        {
            if (ModelState.IsValid)
            {
                // Obtener el nombre de usuario real desde la base de datos
                string nombreReal = "";
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("UsuariosSelect", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Id", model.Id);
                        con.Open();
                        var reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            nombreReal = reader["Nombre"].ToString();
                        }
                    }
                }

                // Validar si el nombre de usuario ya existe en otro usuario (opcional, pero seguro)
                bool existe = false;
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("UsuariosExisteNombre", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Nombre", nombreReal);
                        cmd.Parameters.AddWithValue("@Id", model.Id); // Excluir el usuario actual
                        con.Open();
                        var result = cmd.ExecuteScalar();
                        existe = Convert.ToInt32(result) > 0;
                    }
                }
                if (existe)
                {
                    ModelState.AddModelError("Nombre", "El nombre de usuario ya existe. Por favor, elija otro.");
                    ViewBag.Roles = ObtenerRolesActivos();
                    model.Nombre = nombreReal;
                    return View(model);
                }

                // Actualizar usuario si el nombre es único
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("UsuariosUpdate", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Id", model.Id);
                        cmd.Parameters.AddWithValue("@Nombre", nombreReal); // Siempre usa el nombre real
                        cmd.Parameters.AddWithValue("@Email", model.Email);
                        cmd.Parameters.AddWithValue("@Contrasenia", model.Contrasenia);
                        cmd.Parameters.AddWithValue("@RolId", model.RolId);
                        cmd.Parameters.AddWithValue("@Estado", model.Estado);
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                TempData["SwalUsuarioEditado"] = nombreReal;
                return RedirectToAction("Index");
            }
            ViewBag.Roles = ObtenerRolesActivos();
            return View(model);
        }
        // Alternar activar/desactivar (Delete)
        public ActionResult Delete(int id)
        {
            Usuario usuario = null;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("UsuariosSelect", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", id);
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        usuario = new Usuario
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Nombre = reader["Nombre"].ToString(),
                            Email = reader["Email"].ToString(),
                            Contrasenia = reader["Contrasenia"].ToString(),
                            RolNombre = reader["RolNombre"].ToString(),
                            Estado = Convert.ToChar(reader["Estado"]),
                            FechaCreacion = Convert.ToDateTime(reader["FechaCreacion"]),
                            FechaActualizacion = reader["FechaActualizacion"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["FechaActualizacion"])
                        };
                    }
                }
            }
            if (usuario == null) return HttpNotFound();
            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            char estadoActual = '1';
            Usuario usuarioActual = null;

            // Obtener estado actual y datos del usuario
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("UsuariosSelect", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", id);
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        estadoActual = Convert.ToChar(reader["Estado"]);
                        usuarioActual = new Usuario
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Nombre = reader["Nombre"].ToString(),
                            Email = reader["Email"].ToString(),
                            Contrasenia = reader["Contrasenia"].ToString(),
                            RolNombre = reader["Rol"].ToString(),
                            Estado = Convert.ToChar(reader["Estado"]),
                            FechaCreacion = Convert.ToDateTime(reader["FechaCreacion"]),
                            FechaActualizacion = reader["FechaActualizacion"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["FechaActualizacion"])
                        };
                    }
                }
            }

            char nuevoEstado = estadoActual == '1' ? '0' : '1';

            // Actualizar el estado
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("UsuariosDeactivate", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@NuevoEstado", nuevoEstado);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            // TempData para Swal Fire
            TempData["SwalAccion"] = nuevoEstado == '1' ? "activó" : "desactivó";
            TempData["SwalUsuario"] = usuarioActual != null ? usuarioActual.Nombre : "";
            return RedirectToAction("Index");
        }

        //Para Roles
        private List<SelectListItem> ObtenerRolesActivos()
        {
            var roles = new List<SelectListItem>();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("RolesSelectActive", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        roles.Add(new SelectListItem
                        {
                            Value = reader["Id"].ToString(),
                            Text = reader["Nombre"].ToString()
                        });
                    }
                }
            }
            return roles;
        }
    }
}