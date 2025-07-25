using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Mvc;
using SoftPets.Models;

namespace SoftPets.Controllers
{
    public class RolesController : Controller
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["ConexionLocal"].ConnectionString;

        // Index
        public ActionResult Index()
        {
            var lista = new List<Rol>();
            using (var con = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand("RolesSelect", con))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                con.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new Rol
                        {
                            Id = (int)dr["Id"],
                            Nombre = dr["Nombre"].ToString(),
                            Estado = dr["Estado"] != DBNull.Value ? dr["Estado"].ToString()[0] : '0'
                        });
                    }
                }
            }
            return View(lista);
        }

        // Crear
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Rol model)
        {
            if (ModelState.IsValid)
            {
                using (var con = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand("RolesInsert", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Nombre", model.Nombre);
                    cmd.Parameters.AddWithValue("@Estado", '1'); // Por defecto activo
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                return RedirectToAction("Index");
            }
            return View(model);
        }

        // Editar
        public ActionResult Edit(int id)
        {
            Rol rol = null;
            using (var con = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand("SELECT Id, Nombre, Estado FROM Roles WHERE Id=@id", con))
            {
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                using (var dr = cmd.ExecuteReader())
                    if (dr.Read())
                        rol = new Rol
                        {
                            Id = (int)dr["Id"],
                            Nombre = dr["Nombre"].ToString(),
                            Estado = dr["Estado"] != DBNull.Value ? dr["Estado"].ToString()[0] : '0'
                        };
            }
            return View(rol);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Rol model)
        {
            if (ModelState.IsValid)
            {
                using (var con = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand("RolesUpdate", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", model.Id);
                    cmd.Parameters.AddWithValue("@Nombre", model.Nombre);
                    cmd.Parameters.AddWithValue("@Estado", model.Estado);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult CambiarEstado(int id, string nuevoEstado)
        {
            try
            {
                if (string.IsNullOrEmpty(nuevoEstado) || (nuevoEstado != "1" && nuevoEstado != "0"))
                    return new HttpStatusCodeResult(400, "Parámetro nuevoEstado inválido.");

                using (var con = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand("RolesDeactivate", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@NuevoEstado", nuevoEstado);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                return new HttpStatusCodeResult(200);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(500, ex.Message);
            }
        }
    }
    }