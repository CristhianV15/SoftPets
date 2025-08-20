using SoftPets.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace SoftPets.Controllers
{
    public class DetalleHistorialClinicoController : Controller
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["ConexionLocal"].ConnectionString;

        // Listar detalles de un historial clínico
        public ActionResult Index(int historialClinicoId)
        {
            var lista = new List<DetalleHistorialClinico>();
            string motivo = "";

            try
            {
                // Obtener el motivo del historial clínico
                using (var con = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand(@"SELECT Motivo FROM HistorialesClinicos WHERE Id = @HistorialClinicoId", con))
                {
                    cmd.Parameters.AddWithValue("@HistorialClinicoId", historialClinicoId);
                    con.Open();
                    var result = cmd.ExecuteScalar();
                    if (result != null)
                        motivo = result.ToString();
                }

                // Obtener los detalles
                using (var con = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand(@"SELECT * FROM DetallesHistorialesClinicos WHERE HistorialClinicoId=@HistorialClinicoId", con))
                {
                    cmd.Parameters.AddWithValue("@HistorialClinicoId", historialClinicoId);
                    con.Open();
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new DetalleHistorialClinico
                            {
                                Id = (int)dr["Id"],
                                HistorialClinicoId = (int)dr["HistorialClinicoId"],
                                FechaConsulta = (DateTime)dr["FechaConsulta"],
                                Recomendaciones = dr["Recomendaciones"].ToString(),
                                FechaFuturaConsulta = dr["FechaFuturaConsulta"] != DBNull.Value ? (DateTime?)dr["FechaFuturaConsulta"] : null
                            });
                        }
                    }
                }
                ViewBag.HistorialClinicoId = historialClinicoId;
                ViewBag.Motivo = motivo;
            }
            catch (Exception ex)
            {
                TempData["SwalError"] = "Error al cargar detalles: " + ex.Message;
            }
            return View(lista);
        }
        // Crear detalle
        public ActionResult Create(int historialClinicoId)
        {
            ViewBag.HistorialClinicoId = historialClinicoId;
            var model = new DetalleHistorialClinico
            {
                HistorialClinicoId = historialClinicoId,
                FechaConsulta = DateTime.Now.Date,
                FechaFuturaConsulta = DateTime.Now.Date.AddDays(7)
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DetalleHistorialClinico model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (var con = new SqlConnection(connectionString))
                    using (var cmd = new SqlCommand(@"INSERT INTO DetallesHistorialesClinicos 
                        (HistorialClinicoId, FechaConsulta, Recomendaciones, FechaFuturaConsulta)
                        VALUES (@HistorialClinicoId, @FechaConsulta, @Recomendaciones, @FechaFuturaConsulta)", con))
                    {
                        cmd.Parameters.AddWithValue("@HistorialClinicoId", model.HistorialClinicoId);
                        cmd.Parameters.AddWithValue("@FechaConsulta", model.FechaConsulta);
                        cmd.Parameters.AddWithValue("@Recomendaciones", model.Recomendaciones);
                        cmd.Parameters.AddWithValue("@FechaFuturaConsulta", (object)model.FechaFuturaConsulta ?? DBNull.Value);
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                    TempData["SwalMascotaEditada"] = "Detalle creado correctamente";
                    return RedirectToAction("Index", new { historialClinicoId = model.HistorialClinicoId });
                }
                catch (Exception ex)
                {
                    TempData["SwalError"] = "Error al crear detalle: " + ex.Message;
                }
            }
            ViewBag.HistorialClinicoId = model.HistorialClinicoId;
            return View(model);
        }

        // Ver detalle
        public ActionResult Details(int id)
        {
            DetalleHistorialClinico model = null;
            try
            {
                using (var con = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand(@"SELECT * FROM DetallesHistorialesClinicos WHERE Id=@Id", con))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    con.Open();
                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            model = new DetalleHistorialClinico
                            {
                                Id = (int)dr["Id"],
                                HistorialClinicoId = (int)dr["HistorialClinicoId"],
                                FechaConsulta = (DateTime)dr["FechaConsulta"],
                                Recomendaciones = dr["Recomendaciones"].ToString(),
                                FechaFuturaConsulta = dr["FechaFuturaConsulta"] != DBNull.Value ? (DateTime?)dr["FechaFuturaConsulta"] : null
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["SwalError"] = "Error al cargar detalle: " + ex.Message;
            }
            return View(model);
        }

        // Editar detalle
        public ActionResult Edit(int id)
        {
            return Details(id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DetalleHistorialClinico model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (var con = new SqlConnection(connectionString))
                    using (var cmd = new SqlCommand(@"UPDATE DetallesHistorialesClinicos SET 
                        FechaConsulta=@FechaConsulta, 
                        Recomendaciones=@Recomendaciones, 
                        FechaFuturaConsulta=@FechaFuturaConsulta
                        WHERE Id=@Id", con))
                    {
                        cmd.Parameters.AddWithValue("@FechaConsulta", model.FechaConsulta);
                        cmd.Parameters.AddWithValue("@Recomendaciones", model.Recomendaciones);
                        cmd.Parameters.AddWithValue("@FechaFuturaConsulta", (object)model.FechaFuturaConsulta ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Id", model.Id);
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                    TempData["SwalMascotaEditada"] = "Detalle editado correctamente";
                    return RedirectToAction("Index", new { historialClinicoId = model.HistorialClinicoId });
                }
                catch (Exception ex)
                {
                    TempData["SwalError"] = "Error al editar detalle: " + ex.Message;
                }
            }
            ViewBag.HistorialClinicoId = model.HistorialClinicoId;
            return View(model);
        }


        public ActionResult ProximasConsultas()
        {
            int duenioId = (int)Session["DuenioId"];
            var lista = new List<ProximaConsultaVM>();

            string query = @"
        SELECT d.Id, d.HistorialClinicoId, d.FechaFuturaConsulta, d.Estado, h.Tipo, h.Motivo, m.Nombre AS NombreMascota
        FROM DetallesHistorialesClinicos d
        INNER JOIN HistorialesClinicos h ON d.HistorialClinicoId = h.Id
        INNER JOIN Mascotas m ON h.MascotaId = m.Id
        WHERE h.DuenioId = @DuenioId
          AND d.FechaFuturaConsulta IS NOT NULL
          AND d.FechaFuturaConsulta >= GETDATE()
          AND (d.Estado IS NULL OR d.Estado = 'Pendiente')
        ORDER BY d.FechaFuturaConsulta
    ";

            using (var con = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@DuenioId", duenioId);
                con.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new ProximaConsultaVM
                        {
                            Id = (int)dr["Id"],
                            HistorialClinicoId = (int)dr["HistorialClinicoId"],
                            FechaFuturaConsulta = (DateTime)dr["FechaFuturaConsulta"],
                            Estado = dr["Estado"] != DBNull.Value ? dr["Estado"].ToString() : "Pendiente",
                            Tipo = dr["Tipo"].ToString(),
                            Motivo = dr["Motivo"].ToString(),
                            NombreMascota = dr["NombreMascota"].ToString()
                        });
                    }
                }
            }
            return View(lista);
        }


        // Eliminar detalle
        public ActionResult Delete(int id, int historialClinicoId)
        {
            try
            {
                using (var con = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand("DELETE FROM DetallesHistorialesClinicos WHERE Id=@Id", con))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                TempData["SwalMascotaEditada"] = "Detalle eliminado correctamente";
            }
            catch (Exception ex)
            {
                TempData["SwalError"] = "Error al eliminar detalle: " + ex.Message;
            }
            return RedirectToAction("Index", new { historialClinicoId = historialClinicoId });
        }
    }
}