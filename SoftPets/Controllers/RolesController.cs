using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SoftPets.Models;

namespace SoftPets.Controllers
{
    public class RolesController : Controller
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["ConexionLocal"].ConnectionString;
        
        //Vista Index
        public ActionResult Index()
        {
            var lista = new List<Rol>();
            using (var con = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand("RolesSelect", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new Rol
                        {
                            Id = (int)dr["Id"],
                            Nombre = dr["Nombre"].ToString(),
                            Estado = dr["Estado"] != DBNull.Value ? dr["Estado"].ToString()[0] : '0' // Asignación corregida

                        });
                    }
                }
            }
            return View(lista);
        }

        //Crear 
        public ActionResult Create()
        {
            return View();
        }
        //Crear
        [HttpPost]
        public ActionResult Create(Rol model)
        {
            if (ModelState.IsValid)
            {
                using (var con = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand("RolesInsert", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Nombre", model.Nombre);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                return RedirectToAction("Index");
            }
            return View(model);
        }

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
                        rol = new Rol { 
                        Id = (int)dr["Id"], 
                        Nombre = dr["Nombre"].ToString(),
                        Estado = dr["Estado"] != DBNull.Value ? dr["Estado"].ToString()[0] : '0'
                        };
            }
            return View(rol);
        }

        //Editar
        [HttpPost]
        public ActionResult Edit(Rol model)
        {
            if (ModelState.IsValid)
            {
                using (var con = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand("RolesUpdate", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
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

        //Detalles
        public ActionResult Details(int id)
        {
            Rol rol = null;
            using (var con = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand("SELECT Id, Nombre, Estado FROM Roles WHERE Id=@id", con))
            {
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        rol = new Rol
                        {
                            Id = (int)dr["Id"],
                            Nombre = dr["Nombre"].ToString(),
                            Estado = dr["Estado"] != DBNull.Value ? dr["Estado"].ToString()[0] : '0'
                        };
                    }
                }
            }

            if (rol == null)
            {
                return HttpNotFound(); // Devuelve un error 404 si no se encuentra el rol
            }

            return View(rol); // Devuelve la vista con el rol encontrado
        }


        //Eliminar
        public ActionResult Delete(int id)
        {
            using (var con = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand("RolesDeactivate", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);
                con.Open();
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("Index");
        }
    }
}