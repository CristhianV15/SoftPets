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

        // INDEX: Historial y pendientes por mascota
        public ActionResult Index(int mascotaId, string tipo = "", string estado = "")
        {
            var lista = new List<Vacunacion>();
            string query = @"
                SELECT v.*, va.Nombre AS NombreVacuna, va.Tipo
                FROM Vacunaciones v
                INNER JOIN Vacunas va ON v.VacunaId = va.Id
                WHERE v.MascotaId = @MascotaId
            ";
            if (!string.IsNullOrEmpty(tipo))
                query += " AND va.Tipo = @Tipo";
            if (!string.IsNullOrEmpty(estado))
                query += " AND v.Estado = @Estado";

            using (var con = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@MascotaId", mascotaId);
                if (!string.IsNullOrEmpty(tipo))
                    cmd.Parameters.AddWithValue("@Tipo", tipo);
                if (!string.IsNullOrEmpty(estado))
                    cmd.Parameters.AddWithValue("@Estado", estado);

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
                            DosisAplicada = dr["DosisAplicada"].ToString(),
                            FechaAplicada = dr["FechaAplicada"] != DBNull.Value ? (DateTime?)dr["FechaAplicada"] : null,
                            FechaProgramada = dr["FechaProgramada"] != DBNull.Value ? (DateTime?)dr["FechaProgramada"] : null,
                            Lote = dr["Lote"].ToString(),
                            Observaciones = dr["Observaciones"].ToString(),
                            Estado = dr["Estado"].ToString(),
                            FechaCreacion = (DateTime)dr["FechaCreacion"],
                            FechaActualizacion = dr["FechaActualizacion"] != DBNull.Value ? (DateTime?)dr["FechaActualizacion"] : null
                        });
                    }
                }
            }
            ViewBag.Tipo = tipo;
            ViewBag.Estado = estado;
            ViewBag.MascotaId = mascotaId;
            return View(lista);
        }

        // CREATE: GET
        public ActionResult Create(int mascotaId)
        {
            ViewBag.MascotaId = mascotaId;
            ViewBag.Vacunas = GetVacunasSelectList();
            return View();
        }

        // CREATE: POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Vacunacion model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Obtener frecuencia y unidad de la vacuna seleccionada
                    int? frecuencia = null, duracion = null;
                    string unidadFrecuencia = null, unidadDuracion = null;
                    using (var con = new SqlConnection(connectionString))
                    using (var cmd = new SqlCommand("SELECT Frecuencia, UnidadFrecuencia, Duracion, UnidadDuracion FROM Vacunas WHERE Id=@Id", con))
                    {
                        cmd.Parameters.AddWithValue("@Id", model.VacunaId);
                        con.Open();
                        using (var dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                frecuencia = dr["Frecuencia"] != DBNull.Value ? (int?)dr["Frecuencia"] : null;
                                unidadFrecuencia = dr["UnidadFrecuencia"].ToString();
                                duracion = dr["Duracion"] != DBNull.Value ? (int?)dr["Duracion"] : null;
                                unidadDuracion = dr["UnidadDuracion"].ToString();
                            }
                        }
                    }

                    // Insertar la vacunación aplicada
                    using (var con = new SqlConnection(connectionString))
                    using (var cmd = new SqlCommand(@"
                        INSERT INTO Vacunaciones 
                        (MascotaId, VacunaId, DosisAplicada, FechaAplicada, FechaProgramada, Lote, Observaciones, Estado, FechaCreacion, FechaActualizacion)
                        VALUES (@MascotaId, @VacunaId, @DosisAplicada, @FechaAplicada, @FechaProgramada, @Lote, @Observaciones, @Estado, @FechaCreacion, @FechaActualizacion)", con))
                    {
                        cmd.Parameters.AddWithValue("@MascotaId", model.MascotaId);
                        cmd.Parameters.AddWithValue("@VacunaId", model.VacunaId);
                        cmd.Parameters.AddWithValue("@DosisAplicada", (object)model.DosisAplicada ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@FechaAplicada", (object)model.FechaAplicada ?? DBNull.Value);
                        // Calcular próxima fecha programada
                        DateTime? fechaProx = null;
                        if (model.FechaAplicada.HasValue && frecuencia.HasValue && !string.IsNullOrEmpty(unidadFrecuencia))
                        {
                            fechaProx = CalcularProximaFecha(model.FechaAplicada.Value, frecuencia.Value, unidadFrecuencia);
                        }
                        cmd.Parameters.AddWithValue("@FechaProgramada", (object)fechaProx ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Lote", (object)model.Lote ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Observaciones", (object)model.Observaciones ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Estado", "Aplicada");
                        cmd.Parameters.AddWithValue("@FechaCreacion", DateTime.Now);
                        cmd.Parameters.AddWithValue("@FechaActualizacion", DateTime.Now);
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                    TempData["SwalMascotaEditada"] = "Vacunación registrada correctamente";
                    return RedirectToAction("Index", new { mascotaId = model.MascotaId });
                }
                catch (Exception ex)
                {
                    TempData["SwalError"] = "Error al registrar: " + ex.Message;
                }
            }
            ViewBag.MascotaId = model.MascotaId;
            ViewBag.Vacunas = GetVacunasSelectList();
            return View(model);
        }

        // EDIT: GET
        public ActionResult Edit(int id)
        {
            Vacunacion model = null;
            using (var con = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand("SELECT * FROM Vacunaciones WHERE Id=@Id", con))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                con.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        model = new Vacunacion
                        {
                            Id = (int)dr["Id"],
                            MascotaId = (int)dr["MascotaId"],
                            VacunaId = (int)dr["VacunaId"],
                            DosisAplicada = dr["DosisAplicada"].ToString(),
                            FechaAplicada = dr["FechaAplicada"] != DBNull.Value ? (DateTime?)dr["FechaAplicada"] : null,
                            FechaProgramada = dr["FechaProgramada"] != DBNull.Value ? (DateTime?)dr["FechaProgramada"] : null,
                            Lote = dr["Lote"].ToString(),
                            Observaciones = dr["Observaciones"].ToString(),
                            Estado = dr["Estado"].ToString(),
                            FechaCreacion = (DateTime)dr["FechaCreacion"],
                            FechaActualizacion = dr["FechaActualizacion"] != DBNull.Value ? (DateTime?)dr["FechaActualizacion"] : null
                        };
                    }
                }
            }
            if (model == null)
                return HttpNotFound();
            ViewBag.MascotaId = model.MascotaId;
            ViewBag.Vacunas = GetVacunasSelectList();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Vacunacion model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (var con = new SqlConnection(connectionString))
                    using (var cmd = new SqlCommand(@"
                        UPDATE Vacunaciones SET 
                            VacunaId=@VacunaId, DosisAplicada=@DosisAplicada, FechaAplicada=@FechaAplicada, FechaProgramada=@FechaProgramada, 
                            Lote=@Lote, Observaciones=@Observaciones, Estado=@Estado, FechaActualizacion=@FechaActualizacion
                        WHERE Id=@Id", con))
                    {
                        cmd.Parameters.AddWithValue("@VacunaId", model.VacunaId);
                        cmd.Parameters.AddWithValue("@DosisAplicada", (object)model.DosisAplicada ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@FechaAplicada", (object)model.FechaAplicada ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@FechaProgramada", (object)model.FechaProgramada ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Lote", (object)model.Lote ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Observaciones", (object)model.Observaciones ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Estado", model.Estado);
                        cmd.Parameters.AddWithValue("@FechaActualizacion", DateTime.Now);
                        cmd.Parameters.AddWithValue("@Id", model.Id);
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                    TempData["SwalMascotaEditada"] = "Vacunación editada correctamente";
                    return RedirectToAction("Index", new { mascotaId = model.MascotaId });
                }
                catch (Exception ex)
                {
                    TempData["SwalError"] = "Error al editar: " + ex.Message;
                }
            }
            ViewBag.MascotaId = model.MascotaId;
            ViewBag.Vacunas = GetVacunasSelectList();
            return View(model);
        }

        // Helper: Obtener vacunas activas para el select
        private List<SelectListItem> GetVacunasSelectList()
        {
            var vacunas = new List<SelectListItem>();
            using (var con = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand("SELECT Id, Nombre, Tipo FROM Vacunas WHERE Estado='Activo'", con))
            {
                con.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        vacunas.Add(new SelectListItem
                        {
                            Value = dr["Id"].ToString(),
                            Text = $"{dr["Nombre"]} ({dr["Tipo"]})"
                        });
                    }
                }
            }
            return vacunas;
        }

        // Helper: Calcular próxima fecha programada
        private DateTime? CalcularProximaFecha(DateTime fechaAplicada, int frecuencia, string unidadFrecuencia)
        {
            switch (unidadFrecuencia)
            {
                case "Día": return fechaAplicada.AddDays(frecuencia);
                case "Semana": return fechaAplicada.AddDays(frecuencia * 7);
                case "Mes": return fechaAplicada.AddMonths(frecuencia);
                case "Año": return fechaAplicada.AddYears(frecuencia);
                default: return null;
            }
        }
    }
}