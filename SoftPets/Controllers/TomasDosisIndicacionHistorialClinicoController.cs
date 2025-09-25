using SoftPets.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftPets.Controllers
{
    public class TomasDosisIndicacionHistorialClinicoController : Controller
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["ConexionLocal"].ConnectionString;

        // INDEX GENERAL: Todas las tomas del dueño, con filtros por mascota, medicamento, fecha, estado
        public ActionResult IndexGeneral(string estado = "", string mascota = "", string medicamento = "", string motivo = "", string fecha = "")
        {
            int duenioId = (int)Session["DuenioId"];
            var lista = new List<TomasDosisIndicacionHistorialClinicoVM>();

            string query = @"
        SELECT t.Id, t.DosisIndicacionHistorialClinicoId, t.FechaHoraProgramada, t.Estado, t.Observaciones,
               m.Nombre AS NombreMascota, i.Medicamento, h.Motivo
        FROM TomasDosisIndicacionHistorialClinico t
        INNER JOIN DosisIndicacionesHistorialesClinicos d ON t.DosisIndicacionHistorialClinicoId = d.Id
        INNER JOIN IndicacionesHistorialesClinicos i ON d.IndicacionesHistorialesClinicosId = i.Id
        INNER JOIN DetallesHistorialesClinicos dt ON i.DetallesHistorialesClinicosId = dt.Id
        INNER JOIN HistorialesClinicos h ON dt.HistorialClinicoId = h.Id
        INNER JOIN Mascotas m ON h.MascotaId = m.Id
        WHERE h.DuenioId = @DuenioId
    ";

            if (!string.IsNullOrEmpty(estado))
                query += " AND t.Estado = @Estado";
            if (!string.IsNullOrEmpty(mascota))
                query += " AND m.Nombre LIKE @Mascota";
            if (!string.IsNullOrEmpty(medicamento))
                query += " AND i.Medicamento LIKE @Medicamento";
            if (!string.IsNullOrEmpty(motivo))
                query += " AND h.Motivo LIKE @Motivo";
            if (!string.IsNullOrEmpty(fecha))
                query += " AND CONVERT(date, t.FechaHoraProgramada) = @Fecha";

            using (var con = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@DuenioId", duenioId);
                if (!string.IsNullOrEmpty(estado))
                    cmd.Parameters.AddWithValue("@Estado", estado);
                if (!string.IsNullOrEmpty(mascota))
                    cmd.Parameters.AddWithValue("@Mascota", "%" + mascota + "%");
                if (!string.IsNullOrEmpty(medicamento))
                    cmd.Parameters.AddWithValue("@Medicamento", "%" + medicamento + "%");
                if (!string.IsNullOrEmpty(motivo))
                    cmd.Parameters.AddWithValue("@Motivo", "%" + motivo + "%");
                if (!string.IsNullOrEmpty(fecha))
                    cmd.Parameters.AddWithValue("@Fecha", DateTime.Parse(fecha).Date);

                con.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new TomasDosisIndicacionHistorialClinicoVM
                        {
                            Id = (int)dr["Id"],
                            DosisIndicacionHistorialClinicoId = (int)dr["DosisIndicacionHistorialClinicoId"],
                            FechaHoraProgramada = (DateTime)dr["FechaHoraProgramada"],
                            Estado = dr["Estado"].ToString(),
                            Observaciones = dr["Observaciones"] != DBNull.Value ? dr["Observaciones"].ToString() : "",
                            NombreMascota = dr["NombreMascota"].ToString(),
                            Medicamento = dr["Medicamento"].ToString(),
                            Motivo = dr["Motivo"].ToString()
                        });
                    }
                }
            }
            ViewBag.Estado = estado;
            ViewBag.Mascota = mascota;
            ViewBag.Medicamento = medicamento;
            ViewBag.Motivo = motivo;
            ViewBag.Fecha = fecha;

            if (Request.IsAjaxRequest())
                return PartialView("_TablaTomasGeneral", lista);
            return View(lista);
        }

        public ActionResult FiltrarAjax(string estado = "", string motivo = "", string mascota = "", string medicamento = "", string fecha = "")
        {
            int duenioId = (int)Session["DuenioId"];
            var lista = new List<TomasDosisIndicacionHistorialClinicoVM>();

            string query = @"
        SELECT t.Id, t.DosisIndicacionHistorialClinicoId, t.FechaHoraProgramada, t.Estado, t.Observaciones,
               m.Nombre AS NombreMascota, i.Medicamento, h.Motivo
        FROM TomasDosisIndicacionHistorialClinico t
        INNER JOIN DosisIndicacionesHistorialesClinicos d ON t.DosisIndicacionHistorialClinicoId = d.Id
        INNER JOIN IndicacionesHistorialesClinicos i ON d.IndicacionesHistorialesClinicosId = i.Id
        INNER JOIN DetallesHistorialesClinicos dt ON i.DetallesHistorialesClinicosId = dt.Id
        INNER JOIN HistorialesClinicos h ON dt.HistorialClinicoId = h.Id
        INNER JOIN Mascotas m ON h.MascotaId = m.Id
        WHERE h.DuenioId = @DuenioId
    ";

            if (!string.IsNullOrEmpty(estado))
                query += " AND t.Estado = @Estado";
            if (!string.IsNullOrEmpty(mascota))
                query += " AND m.Nombre LIKE @Mascota";
            if (!string.IsNullOrEmpty(medicamento))
                query += " AND i.Medicamento LIKE @Medicamento";
            if (!string.IsNullOrEmpty(motivo))
                query += " AND h.Motivo LIKE @Motivo";
            if (!string.IsNullOrEmpty(fecha))
                query += " AND CONVERT(date, t.FechaHoraProgramada) = @Fecha";

            using (var con = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@DuenioId", duenioId);
                if (!string.IsNullOrEmpty(estado))
                    cmd.Parameters.AddWithValue("@Estado", estado);
                if (!string.IsNullOrEmpty(mascota))
                    cmd.Parameters.AddWithValue("@Mascota", "%" + mascota + "%");
                if (!string.IsNullOrEmpty(medicamento))
                    cmd.Parameters.AddWithValue("@Medicamento", "%" + medicamento + "%");
                if (!string.IsNullOrEmpty(motivo))
                    cmd.Parameters.AddWithValue("@Motivo", "%" + motivo + "%");
                if (!string.IsNullOrEmpty(fecha))
                    cmd.Parameters.AddWithValue("@Fecha", DateTime.Parse(fecha).Date);

                con.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new TomasDosisIndicacionHistorialClinicoVM
                        {
                            Id = (int)dr["Id"],
                            DosisIndicacionHistorialClinicoId = (int)dr["DosisIndicacionHistorialClinicoId"],
                            FechaHoraProgramada = (DateTime)dr["FechaHoraProgramada"],
                            Estado = dr["Estado"].ToString(),
                            Observaciones = dr["Observaciones"] != DBNull.Value ? dr["Observaciones"].ToString() : "",
                            NombreMascota = dr["NombreMascota"].ToString(),
                            Medicamento = dr["Medicamento"].ToString(),
                            Motivo = dr["Motivo"].ToString()
                        });
                    }
                }
            }
            return PartialView("_TablaTomasGeneral", lista);
        }

        // INDEX POR DOSIS (ver todas las tomas de una receta específica)
        public ActionResult Index(int dosisIndicacionHistorialClinicoId)
        {
            var lista = new List<TomasDosisIndicacionHistorialClinico>();
            string medicamento = "";
            try
            {
                // Obtener el medicamento de la dosis
                using (var con = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand(@"SELECT Medicamento FROM IndicacionesHistorialesClinicos 
            WHERE Id = (SELECT IndicacionesHistorialesClinicosId FROM DosisIndicacionesHistorialesClinicos WHERE Id = @DosisId)", con))
                {
                    cmd.Parameters.AddWithValue("@DosisId", dosisIndicacionHistorialClinicoId);
                    con.Open();
                    var result = cmd.ExecuteScalar();
                    if (result != null)
                        medicamento = result.ToString();
                }

                // Obtener las tomas de la dosis
                using (var con = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand(@"SELECT * FROM TomasDosisIndicacionHistorialClinico WHERE DosisIndicacionHistorialClinicoId=@DosisId ORDER BY FechaHoraProgramada", con))
                {
                    cmd.Parameters.AddWithValue("@DosisId", dosisIndicacionHistorialClinicoId);
                    con.Open();
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new TomasDosisIndicacionHistorialClinico
                            {
                                Id = (int)dr["Id"],
                                DosisIndicacionHistorialClinicoId = (int)dr["DosisIndicacionHistorialClinicoId"],
                                FechaHoraProgramada = (DateTime)dr["FechaHoraProgramada"],
                                Estado = dr["Estado"].ToString(),
                                Observaciones = dr["Observaciones"] != DBNull.Value ? dr["Observaciones"].ToString() : ""
                            });
                        }
                    }
                }
                ViewBag.DosisIndicacionHistorialClinicoId = dosisIndicacionHistorialClinicoId;
                ViewBag.Medicamento = medicamento;
            }
            catch (Exception ex)
            {
                TempData["SwalError"] = "Error al cargar tomas: " + ex.Message;
            }
            return View(lista);
        }

        // Marcar toma como realizada
        [HttpPost]
        public ActionResult MarcarRealizada(int id, string observacion = "")
        {
            try
            {
                int dosisId = 0, indicacionId = 0, detalleId = 0, historialId = 0;
                using (var con = new SqlConnection(connectionString))
                {
                    // 1. Marcar la toma como realizada
                    using (var cmd = new SqlCommand(@"
                UPDATE TomasDosisIndicacionHistorialClinico 
                SET Estado='Realizada', Observaciones=@Obs 
                OUTPUT INSERTED.DosisIndicacionHistorialClinicoId 
                WHERE Id=@Id", con))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        cmd.Parameters.AddWithValue("@Obs", (object)observacion ?? DBNull.Value);
                        con.Open();
                        var result = cmd.ExecuteScalar();
                        if (result != null)
                            dosisId = Convert.ToInt32(result);
                    }

                    // 2. Verificar si todas las tomas de la dosis están realizadas
                    if (dosisId > 0)
                    {
                        using (var cmd = new SqlCommand(@"
                    SELECT COUNT(*) 
                    FROM TomasDosisIndicacionHistorialClinico 
                    WHERE DosisIndicacionHistorialClinicoId=@DosisId AND Estado<>'Realizada'", con))
                        {
                            cmd.Parameters.AddWithValue("@DosisId", dosisId);
                            int pendientes = (int)cmd.ExecuteScalar();
                            if (pendientes == 0)
                            {
                                using (var cmd2 = new SqlCommand(@"
                            UPDATE DosisIndicacionesHistorialesClinicos 
                            SET EstadoAlerta='Realizada' 
                            OUTPUT INSERTED.IndicacionesHistorialesClinicosId
                            WHERE Id=@DosisId", con))
                                {
                                    cmd2.Parameters.AddWithValue("@DosisId", dosisId);
                                    var result = cmd2.ExecuteScalar();
                                    if (result != null)
                                        indicacionId = Convert.ToInt32(result);
                                }
                            }
                        }
                    }

                    // 3. Si todas las dosis de la indicación están realizadas
                    if (indicacionId > 0)
                    {
                        using (var cmd = new SqlCommand(@"
                    SELECT COUNT(*) 
                    FROM DosisIndicacionesHistorialesClinicos 
                    WHERE IndicacionesHistorialesClinicosId=@IndicacionId AND EstadoAlerta<>'Realizada'", con))
                        {
                            cmd.Parameters.AddWithValue("@IndicacionId", indicacionId);
                            int pendientes = (int)cmd.ExecuteScalar();
                            if (pendientes == 0)
                            {
                                using (var cmd2 = new SqlCommand(@"
                            UPDATE IndicacionesHistorialesClinicos 
                            SET Estado='Realizada' 
                            OUTPUT INSERTED.DetallesHistorialesClinicosId
                            WHERE Id=@IndicacionId", con))
                                {
                                    cmd2.Parameters.AddWithValue("@IndicacionId", indicacionId);
                                    var result = cmd2.ExecuteScalar();
                                    if (result != null)
                                        detalleId = Convert.ToInt32(result);
                                }
                            }
                        }
                    }

                    // 4. Si todas las indicaciones del detalle están realizadas
                    if (detalleId > 0)
                    {
                        using (var cmd = new SqlCommand(@"
                    SELECT COUNT(*) 
                    FROM IndicacionesHistorialesClinicos 
                    WHERE DetallesHistorialesClinicosId=@DetalleId AND Estado<>'Realizada'", con))
                        {
                            cmd.Parameters.AddWithValue("@DetalleId", detalleId);
                            int pendientes = (int)cmd.ExecuteScalar();
                            if (pendientes == 0)
                            {
                                using (var cmd2 = new SqlCommand(@"
                            UPDATE DetallesHistorialesClinicos 
                            SET Estado='Realizada' 
                            OUTPUT INSERTED.HistorialClinicoId
                            WHERE Id=@DetalleId", con))
                                {
                                    cmd2.Parameters.AddWithValue("@DetalleId", detalleId);
                                    var result = cmd2.ExecuteScalar();
                                    if (result != null)
                                        historialId = Convert.ToInt32(result);
                                }
                            }
                        }
                    }

                    // 5. Si todos los detalles del historial están realizados
                    if (historialId > 0)
                    {
                        using (var cmd = new SqlCommand(@"
                    SELECT COUNT(*) 
                    FROM DetallesHistorialesClinicos 
                    WHERE HistorialClinicoId=@HistorialId AND Estado<>'Realizada'", con))
                        {
                            cmd.Parameters.AddWithValue("@HistorialId", historialId);
                            int pendientes = (int)cmd.ExecuteScalar();
                            if (pendientes == 0)
                            {
                                using (var cmd2 = new SqlCommand(@"
                            UPDATE HistorialesClinicos 
                            SET Estado='Finalizado' 
                            WHERE Id=@HistorialId", con))
                                {
                                    cmd2.Parameters.AddWithValue("@HistorialId", historialId);
                                    cmd2.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }


        // Generar tomas automáticamente al crear una dosis
        public void GenerarTomas(int dosisId, DateTime fechaInicio, int intervaloCantidad, string intervaloTipo, int cantidadTotal)
        {
            var fechas = new List<DateTime>();
            DateTime actual = fechaInicio;
            for (int i = 0; i < cantidadTotal; i++)
            {
                fechas.Add(actual);
                if (intervaloTipo == "Hora")
                    actual = actual.AddHours(intervaloCantidad);
                else if (intervaloTipo == "Día")
                    actual = actual.AddDays(intervaloCantidad);
                else if (intervaloTipo == "Semana")
                    actual = actual.AddDays(intervaloCantidad * 7);
            }

            using (var con = new SqlConnection(connectionString))
            {
                con.Open();
                foreach (var fecha in fechas)
                {
                    using (var cmd = new SqlCommand(@"INSERT INTO TomasDosisIndicacionHistorialClinico 
                        (DosisIndicacionHistorialClinicoId, FechaHoraProgramada, Estado) VALUES 
                        (@DosisId, @Fecha, 'Pendiente')", con))
                    {
                        cmd.Parameters.AddWithValue("@DosisId", dosisId);
                        cmd.Parameters.AddWithValue("@Fecha", fecha);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
    }

}