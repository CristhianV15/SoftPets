using SoftPets.Filters;
using SoftPets.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace SoftPets.Controllers
{
    [RequiereDatosCompletos]

    public class VacunacionesController : Controller
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["ConexionLocal"].ConnectionString;

        // Listar historial de vacunas de una mascota
        public ActionResult Index(int mascotaId)
        {
            var lista = new List<Vacunacion>();
            using (var con = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand("VacunacionesSelectByMascota", con))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MascotaId", mascotaId);
                con.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new Vacunacion
                        {
                            Id = (int)dr["Id"],
                            MascotaId = (int)dr["MascotaId"],
                            VacunaId = (int)dr["VacunaId"],
                            Fecha = dr["Fecha"] != DBNull.Value ? (DateTime?)dr["Fecha"] : null,
                            VeterinarioId = (int)dr["VeterinarioId"],
                            Observaciones = dr["Observaciones"].ToString(),
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

        // Crear nueva vacuna
        public ActionResult Create(int mascotaId)
        {
            ViewBag.MascotaId = mascotaId;
            ViewBag.NombreMascota = ObtenerNombreMascota(mascotaId);
            ViewBag.Vacunas = ObtenerVacunas();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Vacunacion model)
        {
            if (ModelState.IsValid)
            {
                using (var con = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand("VacunacionesInsert", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MascotaId", model.MascotaId);
                    cmd.Parameters.AddWithValue("@VacunaId", model.VacunaId);
                    cmd.Parameters.AddWithValue("@Fecha", model.Fecha ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@VeterinarioId", model.VeterinarioId);
                    cmd.Parameters.AddWithValue("@Observaciones", model.Observaciones ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Estado", '1');
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                return RedirectToAction("Index", new { mascotaId = model.MascotaId });
            }
            ViewBag.MascotaId = model.MascotaId;
            ViewBag.Vacunas = ObtenerVacunas();
            return View(model);
        }

        // Helpers
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

        private List<SelectListItem> ObtenerVacunas()
        {
            var vacunas = new List<SelectListItem>();
            using (var con = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand("SELECT Id, Nombre FROM Vacunas WHERE Estado='1'", con))
            {
                con.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        vacunas.Add(new SelectListItem
                        {
                            Value = dr["Id"].ToString(),
                            Text = dr["Nombre"].ToString()
                        });
                    }
                }
            }
            return vacunas;
        }
    }
}