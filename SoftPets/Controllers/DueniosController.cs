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
    public class DueniosController : Controller
    {
        // GET: Duenios
        private string connectionString = ConfigurationManager.ConnectionStrings["ConexionLocal"].ConnectionString;

        // Index
        public ActionResult Index()
        {
            List<Duenio> duenios = new List<Duenio>();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("DueniosSelect", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", DBNull.Value);

                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        duenios.Add(new Duenio
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            UsuarioId = Convert.ToInt32(reader["UsuarioId"]),
                            Nombres = reader["Nombres"].ToString(),
                            Apellidos = reader["Apellidos"].ToString(),
                            DNI = reader["DNI"].ToString(),
                            Celular = reader["Celular"].ToString(),
                            Direccion = reader["Direccion"].ToString(),
                            Estado = Convert.ToChar(reader["Estado"]),
                            FechaCreacion = Convert.ToDateTime(reader["FechaCreacion"]),
                            FechaActualizacion = reader["FechaActualizacion"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["FechaActualizacion"]),
                            NombreUser = reader["NombreUser"].ToString(),
                            Email = reader["Email"].ToString(),
                            Contrasenia = reader["Contrasenia"].ToString()
                        });
                    }
                }
            }
            return View(duenios);
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
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("UsuariosUpdate", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Id", model.Id);
                        cmd.Parameters.AddWithValue("@Nombre", model.Nombre);
                        cmd.Parameters.AddWithValue("@Email", model.Email);
                        cmd.Parameters.AddWithValue("@Contrasenia", model.Contrasenia);
                        cmd.Parameters.AddWithValue("@RolId", model.RolId);
                        cmd.Parameters.AddWithValue("@Estado", 1);
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
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

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            char estadoActual = '1';
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
                    }
                }
            }

            char nuevoEstado = estadoActual == '1' ? '0' : '1';
            Usuario usuarioActual = null;
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
            // Actualizamos el estado, pero mantenemos los demás datos


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

        public ActionResult MisDatos()
        {
            int usuarioId = (int)Session["UsuarioId"];
            var viewModel = new DuenioMisDatosViewModel();

            // Obtener datos de Usuario
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("UsuariosSelect", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", usuarioId);
                    con.Open();
                    var reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        viewModel.Usuario = new Usuario
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Nombre = reader["Nombre"].ToString(),
                            Email = reader["Email"].ToString(),
                            RolNombre = reader["RolNombre"].ToString(),
                            Estado = Convert.ToChar(reader["Estado"]),
                            FechaCreacion = Convert.ToDateTime(reader["FechaCreacion"]),
                            FechaActualizacion = reader["FechaActualizacion"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["FechaActualizacion"])
                        };
                    }
                }
            }

            // Obtener datos de Duenio
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("DueniosSelect", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", DBNull.Value);
                    con.Open();
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        if ((int)reader["UsuarioId"] == usuarioId)
                        {
                            viewModel.Duenio = new Duenio
                            {
                                Id = (int)reader["Id"],
                                UsuarioId = usuarioId,
                                Nombres = reader["Nombres"].ToString(),
                                Apellidos = reader["Apellidos"].ToString(),
                                DNI = reader["DNI"].ToString(),
                                Celular = reader["Celular"].ToString(),
                                Direccion = reader["Direccion"].ToString(),
                                Estado = Convert.ToChar(reader["Estado"]),
                                FechaCreacion = Convert.ToDateTime(reader["FechaCreacion"]),
                                FechaActualizacion = reader["FechaActualizacion"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["FechaActualizacion"])
                            };
                            break;
                        }
                    }
                }
            }

            // Si no tiene datos de Duenio, inicializa para crear
            if (viewModel.Duenio == null)
            {
                viewModel.Duenio = new Duenio { UsuarioId = usuarioId, Estado = '1', FechaCreacion = DateTime.Now };
                ViewBag.EsNuevo = true;
            }
            else
            {
                ViewBag.EsNuevo = false;
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MisDatos(DuenioMisDatosViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (viewModel.Duenio.Id == 0)
                {
                    // Insertar
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand("DueniosInsert", con))
                        {
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@UsuarioId", viewModel.Duenio.UsuarioId);
                            cmd.Parameters.AddWithValue("@Nombres", viewModel.Duenio.Nombres ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@Apellidos", viewModel.Duenio.Apellidos ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@DNI", viewModel.Duenio.DNI ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@Celular", viewModel.Duenio.Celular ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@Direccion", viewModel.Duenio.Direccion ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@Estado", viewModel.Duenio.Estado);
                            con.Open();
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                else
                {
                    // Update
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand("DueniosUpdate", con))
                        {
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@Id", viewModel.Duenio.Id);
                            cmd.Parameters.AddWithValue("@Nombres", viewModel.Duenio.Nombres ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@Apellidos", viewModel.Duenio.Apellidos ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@DNI", viewModel.Duenio.DNI ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@Celular", viewModel.Duenio.Celular ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@Direccion", viewModel.Duenio.Direccion ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@Estado", 1);
                            con.Open();
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                TempData["DatosActualizados"] = true;
                return RedirectToAction("MisDatos");
            }
            return View(viewModel);
        }


        public ActionResult CompletarDatos()
        {
            int usuarioId = (int)Session["UsuarioId"];
            Duenio model = null;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("DueniosSelect", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", DBNull.Value);
                    con.Open();
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        if ((int)reader["UsuarioId"] == usuarioId)
                        {
                            // Ya tiene datos, redirige a MisDatos para editar
                            return RedirectToAction("MisDatos");
                        }
                    }
                }
            }
            // No tiene datos, mostrar formulario para completar
            model = new Duenio { UsuarioId = usuarioId, Estado = '1', FechaCreacion = DateTime.Now };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CompletarDatos(Duenio model)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("DueniosInsert", con))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@UsuarioId", model.UsuarioId);
                        cmd.Parameters.AddWithValue("@Nombres", model.Nombres ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Apellidos", model.Apellidos ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@DNI", model.DNI ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Celular", model.Celular ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Direccion", model.Direccion ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Estado", model.Estado);
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }

    }
    }