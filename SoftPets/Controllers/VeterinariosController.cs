using SoftPets.Models;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace SoftPets.Controllers
{
    public class VeterinariosController : Controller
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["ConexionLocal"].ConnectionString;

        // GET: Veterinarios/MisDatos
        public ActionResult MisDatos()
        {
            int usuarioId = (int)Session["UsuarioId"];
            Veterinario model = null;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("VeterinariosSelect", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", DBNull.Value);
                    con.Open();
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        if ((int)reader["UsuarioId"] == usuarioId)
                        {
                            model = new Veterinario
                            {
                                Id = (int)reader["Id"],
                                UsuarioId = usuarioId,
                                Nombre = reader["Nombre"].ToString(),
                                Colegio = reader["Colegio"].ToString(),
                                CMP = reader["CMP"].ToString(),
                                Estado = Convert.ToChar(reader["Estado"]),
                                FechaCreacion = Convert.ToDateTime(reader["FechaCreacion"]),
                                FechaActualizacion = reader["FechaActualizacion"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["FechaActualizacion"])
                            };
                            break;
                        }
                    }
                }
            }

            if (model == null)
            {
                model = new Veterinario { UsuarioId = usuarioId, Estado = '1', FechaCreacion = DateTime.Now };
                ViewBag.EsNuevo = true;
            }
            else
            {
                ViewBag.EsNuevo = false;
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MisDatos(Veterinario model)
        {
            if (ModelState.IsValid)
            {
                if (model.Id == 0)
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand("VeterinariosInsert", con))
                        {
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@UsuarioId", model.UsuarioId);
                            cmd.Parameters.AddWithValue("@Nombre", model.Nombre ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@Colegio", model.Colegio ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@CMP", model.CMP ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@Estado", model.Estado);
                            con.Open();
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                else
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand("VeterinariosUpdate", con))
                        {
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@Id", model.Id);
                            cmd.Parameters.AddWithValue("@UsuarioId", model.UsuarioId);
                            cmd.Parameters.AddWithValue("@Nombre", model.Nombre ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@Colegio", model.Colegio ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@CMP", model.CMP ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@Estado", model.Estado);
                            con.Open();
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }

        public ActionResult CompletarDatos()
        {
            int usuarioId = (int)Session["UsuarioId"];
            Veterinario model = null;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("VeterinariosSelect", con))
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
            model = new Veterinario { UsuarioId = usuarioId, Estado = '1', FechaCreacion = DateTime.Now };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CompletarDatos(Veterinario model)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("VeterinariosInsert", con))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@UsuarioId", model.UsuarioId);
                        cmd.Parameters.AddWithValue("@Nombre", model.Nombre ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Colegio", model.Colegio ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@CMP", model.CMP ?? (object)DBNull.Value);
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