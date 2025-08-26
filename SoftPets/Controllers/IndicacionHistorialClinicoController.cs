using SoftPets.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace SoftPets.Controllers
{
    public class IndicacionHistorialClinicoController : Controller
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["ConexionLocal"].ConnectionString;

        // Listar indicaciones de un detalle
        public ActionResult Index(int detalleHistorialClinicoId)
        {
            var lista = new List<IndicacionHistorialClinico>();
            string motivo = "";

            try
            {
                // Obtener el motivo a partir del detalle
                using (var con = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand(@"
            SELECT h.Motivo
            FROM DetallesHistorialesClinicos d
            INNER JOIN HistorialesClinicos h ON d.HistorialClinicoId = h.Id
            WHERE d.Id = @DetalleHistorialClinicoId", con))
                {
                    cmd.Parameters.AddWithValue("@DetalleHistorialClinicoId", detalleHistorialClinicoId);
                    con.Open();
                    var result = cmd.ExecuteScalar();
                    if (result != null)
                        motivo = result.ToString();
                }

                // Obtener las indicaciones
                using (var con = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand(@"SELECT * FROM IndicacionesHistorialesClinicos WHERE DetallesHistorialesClinicosId=@DetalleHistorialClinicoId", con))
                {
                    cmd.Parameters.AddWithValue("@DetalleHistorialClinicoId", detalleHistorialClinicoId);
                    con.Open();
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new IndicacionHistorialClinico
                            {
                                Id = (int)dr["Id"],
                                DetalleHistorialClinicoId = (int)dr["DetallesHistorialesClinicosId"],
                                Medicamento = dr["Medicamento"].ToString(),
                                Indicacion = dr["Indicacion"].ToString(),
                                Estado = dr["Estado"].ToString()
                            });
                        }
                    }
                }
                ViewBag.DetalleHistorialClinicoId = detalleHistorialClinicoId;
                ViewBag.Motivo = motivo;
            }
            catch (Exception ex)
            {
                TempData["SwalError"] = "Error al cargar indicaciones: " + ex.Message;
            }
            return View(lista);
        }

        // Crear indicación
        public ActionResult Create(int detalleHistorialClinicoId)
        {
            ViewBag.DetalleHistorialClinicoId = detalleHistorialClinicoId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IndicacionHistorialClinico model)
        {
            if (string.IsNullOrEmpty(model.Estado))
                model.Estado = "Pendiente";

            if (ModelState.IsValid)
            {
                try
                {
                    using (var con = new SqlConnection(connectionString))
                    using (var cmd = new SqlCommand(@"INSERT INTO IndicacionesHistorialesClinicos 
                (DetallesHistorialesClinicosId, Medicamento, Indicacion, Estado)
                VALUES (@DetalleHistorialClinicoId, @Medicamento, @Indicacion, @Estado)", con))
                    {
                        cmd.Parameters.AddWithValue("@DetalleHistorialClinicoId", model.DetalleHistorialClinicoId);
                        cmd.Parameters.AddWithValue("@Medicamento", (object)model.Medicamento ?? DBNull.Value); 
                       cmd.Parameters.AddWithValue("@Indicacion", model.Indicacion);
                        cmd.Parameters.AddWithValue("@Estado", model.Estado);
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                    TempData["SwalMascotaEditada"] = "Indicación creada correctamente";
                    return RedirectToAction("Index", new { detalleHistorialClinicoId = model.DetalleHistorialClinicoId });
                }
                catch (Exception ex)
                {
                    TempData["SwalError"] = "Error al crear indicación: " + ex.Message;
                }
            }
            ViewBag.DetalleHistorialClinicoId = model.DetalleHistorialClinicoId;
            return View(model);
        }

        // Ver indicación
        public ActionResult Details(int id)
        {
            IndicacionHistorialClinico model = null;
            try
            {
                using (var con = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand(@"SELECT * FROM IndicacionesHistorialesClinicos WHERE Id=@Id", con))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    con.Open();
                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            model = new IndicacionHistorialClinico
                            {
                                Id = (int)dr["Id"],
                                DetalleHistorialClinicoId = (int)dr["DetallesHistorialesClinicosId"],
                                Medicamento = dr["Medicamento"].ToString(),
                                Indicacion = dr["Indicacion"].ToString(),
                                Estado = dr["Estado"].ToString()
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["SwalError"] = "Error al cargar indicación: " + ex.Message;
            }
            return View(model);
        }

        // Editar indicación
        public ActionResult Edit(int id)
        {
            return Details(id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(IndicacionHistorialClinico model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (var con = new SqlConnection(connectionString))
                    using (var cmd = new SqlCommand(@"UPDATE IndicacionesHistorialesClinicos SET 
                        Medicamento=@Medicamento, 
                        Indicacion=@Indicacion, 
                        Estado=@Estado
                        WHERE Id=@Id", con))
                    {
                        cmd.Parameters.AddWithValue("@Medicamento", model.Medicamento);
                        cmd.Parameters.AddWithValue("@Indicacion", model.Indicacion);
                        cmd.Parameters.AddWithValue("@Estado", model.Estado);
                        cmd.Parameters.AddWithValue("@Id", model.Id);
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                    TempData["SwalMascotaEditada"] = "Indicación editada correctamente";
                    return RedirectToAction("Index", new { detalleHistorialClinicoId = model.DetalleHistorialClinicoId });
                }
                catch (Exception ex)
                {
                    TempData["SwalError"] = "Error al editar indicación: " + ex.Message;
                }
            }
            ViewBag.DetalleHistorialClinicoId = model.DetalleHistorialClinicoId;
            return View(model);
        }

        // Eliminar indicación
        public ActionResult Delete(int id, int detalleHistorialClinicoId)
        {
            try
            {
                using (var con = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand("DELETE FROM IndicacionesHistorialesClinicos WHERE Id=@Id", con))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                TempData["SwalMascotaEditada"] = "Indicación eliminada correctamente";
            }
            catch (Exception ex)
            {
                TempData["SwalError"] = "Error al eliminar indicación: " + ex.Message;
            }
            return RedirectToAction("Index", new { detalleHistorialClinicoId = detalleHistorialClinicoId });
        }

        [HttpPost]
        public ActionResult MarcarIndicacionRealizada(int id)
        {
            try
            {
                using (var con = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand(@"UPDATE IndicacionesHistorialesClinicos SET Estado='Realizada' WHERE Id=@Id", con))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
        }

    }
}