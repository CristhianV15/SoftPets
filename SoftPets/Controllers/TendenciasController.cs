using SoftPets.Filters;
using SoftPets.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;

namespace SoftPets.Controllers
{
    [RequiereDatosCompletos]

    public class TendenciasController : Controller
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["ConexionLocal"].ConnectionString;

        // Listar historial de tendencias de una mascota
        public ActionResult Index(int? mascotaId, string mascotaNombre = "")
        {
            int rolId = (int)Session["RolId"];
            int duenioId = rolId == 2 ? (int)Session["DuenioId"] : 0;
            var lista = new List<Tendencia>();
            var mascotas = new List<Mascota>();

            // Obtener todas las mascotas para el filtro (todas si veterinario, solo propias si dueño)
            string queryMascotas = rolId == 2
                ? "SELECT Id, Nombre FROM Mascotas WHERE DuenioId=@DuenioId"
                : "SELECT Id, Nombre FROM Mascotas";
            using (var con = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(queryMascotas, con))
            {
                if (rolId == 2)
                    cmd.Parameters.AddWithValue("@DuenioId", duenioId);
                con.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        mascotas.Add(new Mascota
                        {
                            Id = (int)dr["Id"],
                            Nombre = dr["Nombre"].ToString()
                        });
                    }
                }
            }

            if (mascotaId.HasValue)
            {
                using (var con = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand("TendenciasSelectByMascota", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MascotaId", mascotaId.Value);
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
                ViewBag.NombreMascota = mascotas.FirstOrDefault(m => m.Id == mascotaId.Value)?.Nombre ?? "";
            }
            else
            {
                string query = @"
            SELECT t.*, m.Nombre AS NombreMascota
            FROM Tendencias t
            INNER JOIN Mascotas m ON t.MascotaId = m.Id
            WHERE 1=1
        ";
                if (rolId == 2)
                    query += " AND m.DuenioId = @DuenioId";
                if (!string.IsNullOrEmpty(mascotaNombre))
                    query += " AND m.Nombre LIKE @MascotaNombre";

                using (var con = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand(query, con))
                {
                    if (rolId == 2)
                        cmd.Parameters.AddWithValue("@DuenioId", duenioId);
                    if (!string.IsNullOrEmpty(mascotaNombre))
                        cmd.Parameters.AddWithValue("@MascotaNombre", "%" + mascotaNombre + "%");
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
                ViewBag.NombreMascota = "";
            }

            ViewBag.MascotaId = mascotaId;
            ViewBag.Mascotas = mascotas;
            ViewBag.MascotaNombre = mascotaNombre;
            return View(lista);
        }

        // Helper para renderizar la tabla como string (igual que en Vacunaciones)
        private string RenderPartialViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new System.IO.StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }
        public ActionResult Create(int? mascotaId)
        {
            ViewBag.MascotaId = mascotaId;
            if (mascotaId != null)
            {
                ViewBag.NombreMascota = ObtenerNombreMascota(mascotaId.Value);
            }
            else
            {
                int duenioId = (int)Session["DuenioId"];
                var mascotas = new List<Mascota>();
                using (var con = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand("SELECT Id, Nombre FROM Mascotas WHERE DuenioId=@DuenioId", con))
                {
                    cmd.Parameters.AddWithValue("@DuenioId", duenioId);
                    con.Open();
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            mascotas.Add(new Mascota
                            {
                                Id = (int)dr["Id"],
                                Nombre = dr["Nombre"].ToString()
                            });
                        }
                    }
                }
                ViewBag.Mascotas = mascotas;
            }
            var model = new Tendencia
            {
                MascotaId = mascotaId ?? 0,
                Fecha = DateTime.Now
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Tendencia model)
        {
            // Validación: MascotaId debe ser obligatorio
            if (model.MascotaId == 0)
                ModelState.AddModelError("MascotaId", "Debe seleccionar una mascota.");

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
            if (model.MascotaId == 0)
            {
                int duenioId = (int)Session["DuenioId"];
                var mascotas = new List<Mascota>();
                using (var con = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand("SELECT Id, Nombre FROM Mascotas WHERE DuenioId=@DuenioId", con))
                {
                    cmd.Parameters.AddWithValue("@DuenioId", duenioId);
                    con.Open();
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            mascotas.Add(new Mascota
                            {
                                Id = (int)dr["Id"],
                                Nombre = dr["Nombre"].ToString()
                            });
                        }
                    }
                }
                ViewBag.Mascotas = mascotas;
            }
            return View(model);
        }

        // Nuevo: Gráfico de tendencias
        public ActionResult Grafico(int mascotaId)
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