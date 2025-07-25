using SoftPets.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace SoftPets.Controllers
{
    public class TendenciasController : Controller
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["ConexionLocal"].ConnectionString;

        // Listar historial de tendencias de una mascota
        public ActionResult Index(int mascotaId)
        {
            var lista = new List<Tendencia>();
            using (var con = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand("TendenciasSelectByMascota", con))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MascotaId", mascotaId);
                con.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new Tendencia
                        {
                            Id = (int)dr["Id"],
                            MascotaId = (int)dr["MascotaId"],
                            Fecha = dr["Fecha"] != DBNull.Value ? (DateTime?)dr["Fecha"] : null,
                            Peso = dr["Peso"] != DBNull.Value ? (decimal?)dr["Peso"] : null,
                            Temperatura = dr["Temperatura"] != DBNull.Value ? (decimal?)dr["Temperatura"] : null,
                            Otros = dr["Otros"].ToString(),
                            Estado = dr["Estado"].ToString()[0],
                            FechaCreacion = Convert.ToDateTime(dr["FechaCreacion"])
                        });
                    }
                }
            }
            ViewBag.MascotaId = mascotaId;
            ViewBag.NombreMascota = ObtenerNombreMascota(mascotaId);
            return View(lista);
        }

        // Crear nueva tendencia
        public ActionResult Create(int mascotaId)
        {
            ViewBag.MascotaId = mascotaId;
            ViewBag.NombreMascota = ObtenerNombreMascota(mascotaId);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Tendencia model)
        {
            if (ModelState.IsValid)
            {
                using (var con = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand("TendenciasInsert", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MascotaId", model.MascotaId);
                    cmd.Parameters.AddWithValue("@Fecha", model.Fecha ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Peso", model.Peso ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Temperatura", model.Temperatura ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Otros", model.Otros ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Estado", '1');
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                return RedirectToAction("Index", new { mascotaId = model.MascotaId });
            }
            ViewBag.MascotaId = model.MascotaId;
            return View(model);
        }

        // Helper
        private string ObtenerNombreMascota(int mascotaId)
        {
            string nombre = "";
            using (var con = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand("SELECT Nombre FROM Mascotas WHERE Id=@id", con))
            {
                cmd.Parameters.AddWithValue("@id", mascotaId);
                con.Open();
                var result = cmd.ExecuteScalar();
                if (result != null)
                    nombre = result.ToString();
            }
            return nombre;
        }
    }
}