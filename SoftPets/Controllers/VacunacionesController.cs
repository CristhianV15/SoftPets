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

    public class VacunacionesController : Controller
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["ConexionLocal"].ConnectionString;

        // INDEX: Historial y pendientes por mascota
        public ActionResult Index(int? mascotaId, string tipo = "", string estado = "", string mascotaNombre = "", string vacunaNombre = "")
        {
            int rolId = (int)Session["RolId"];
            int duenioId = rolId == 2 ? (int)Session["DuenioId"] : 0; // Solo filtra por dueño si es dueño
            var lista = new List<Vacunacion>();
            var vacunas = new List<Vacuna>();
            var mascotas = new List<Mascota>();

            // Obtener todas las vacunas activas para el diccionario
            using (var con = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand("SELECT Id, Nombre, Lote FROM Vacunas WHERE Estado='Activo'", con))
            {
                con.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        vacunas.Add(new Vacuna
                        {
                            Id = (int)dr["Id"],
                            Nombre = dr["Nombre"].ToString(),
                            Lote = dr["Lote"].ToString()
                        });
                    }
                }
            }

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

            string query = @"
        SELECT v.*, va.Nombre AS NombreVacuna, va.Tipo, m.Nombre AS NombreMascota
        FROM Vacunaciones v
        INNER JOIN Vacunas va ON v.VacunaId = va.Id
        INNER JOIN Mascotas m ON v.MascotaId = m.Id
        WHERE 1=1
    ";
            if (rolId == 2)
                query += " AND m.DuenioId = @DuenioId";
            if (mascotaId.HasValue)
                query += " AND v.MascotaId = @MascotaId";
            if (!string.IsNullOrEmpty(tipo))
                query += " AND va.Tipo = @Tipo";
            if (!string.IsNullOrEmpty(estado))
                query += " AND v.Estado = @Estado";
            if (!string.IsNullOrEmpty(mascotaNombre))
                query += " AND m.Nombre LIKE @MascotaNombre";
            if (!string.IsNullOrEmpty(vacunaNombre))
                query += " AND va.Nombre LIKE @VacunaNombre";

            using (var con = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(query, con))
            {
                if (rolId == 2)
                    cmd.Parameters.AddWithValue("@DuenioId", duenioId);
                if (mascotaId.HasValue)
                    cmd.Parameters.AddWithValue("@MascotaId", mascotaId.Value);
                if (!string.IsNullOrEmpty(tipo))
                    cmd.Parameters.AddWithValue("@Tipo", tipo);
                if (!string.IsNullOrEmpty(estado))
                    cmd.Parameters.AddWithValue("@Estado", estado);
                if (!string.IsNullOrEmpty(mascotaNombre))
                    cmd.Parameters.AddWithValue("@MascotaNombre", "%" + mascotaNombre + "%");
                if (!string.IsNullOrEmpty(vacunaNombre))
                    cmd.Parameters.AddWithValue("@VacunaNombre", "%" + vacunaNombre + "%");

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
                            FechaActualizacion = dr["FechaActualizacion"] != DBNull.Value ? (DateTime?)dr["FechaActualizacion"] : null,
                        });
                    }
                }
            }

            ViewBag.VacunasNombres = vacunas.ToDictionary(v => v.Id, v => v.Nombre);
            ViewBag.Mascotas = mascotas;
            ViewBag.Tipo = tipo;
            ViewBag.Estado = estado;
            ViewBag.MascotaId = mascotaId;
            ViewBag.MascotaNombre = mascotaNombre;
            ViewBag.VacunaNombre = vacunaNombre;
            return View(lista);
        }

        // CREATE: GET
        public ActionResult Create(int? mascotaId)
        {
            ViewBag.MascotaId = mascotaId;
            ViewBag.Vacunas = GetVacunasSelectList();
            if (mascotaId == null)
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
            var model = new Vacunacion
            {
                MascotaId = mascotaId ?? 0,
                FechaAplicada = DateTime.Now,
                Estado = "Aplicada",
                FechaCreacion = DateTime.Now
            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Vacunacion model, int? DosisYaAplicadas, DateTime? FechaUltimaDosis)
        {
            if (string.IsNullOrEmpty(model.Estado))
                model.Estado = "Aplicada";
            if (model.FechaCreacion == default(DateTime))
                model.FechaCreacion = DateTime.Now;

            if (ModelState.IsValid)
            {
                try
                {
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

                    int totalDosis = 0;
                    if (frecuencia.HasValue && duracion.HasValue && !string.IsNullOrEmpty(unidadFrecuencia) && !string.IsNullOrEmpty(unidadDuracion))
                    {
                        if (unidadFrecuencia == "Mes" && unidadDuracion == "Año")
                            totalDosis = duracion.Value * 12 / frecuencia.Value;
                        else if (unidadFrecuencia == "Mes" && unidadDuracion == "Mes")
                            totalDosis = duracion.Value / frecuencia.Value;
                        else if (unidadFrecuencia == "Año" && unidadDuracion == "Año")
                            totalDosis = duracion.Value / frecuencia.Value;
                        else if (unidadFrecuencia == "Semana" && unidadDuracion == "Mes")
                            totalDosis = (duracion.Value * 4) / frecuencia.Value;
                        else if (unidadFrecuencia == "Día" && unidadDuracion == "Mes")
                            totalDosis = (duracion.Value * 30) / frecuencia.Value;
                        else
                            totalDosis = duracion.Value;
                        if (totalDosis < 1) totalDosis = 1;
                    }

                    int dosisAplicadasPrevias = DosisYaAplicadas ?? 0;
                    int dosisActual = dosisAplicadasPrevias + 1;

                    // Insertar los registros históricos si corresponde
                    if (dosisAplicadasPrevias > 0 && frecuencia.HasValue && !string.IsNullOrEmpty(unidadFrecuencia))
                    {
                        DateTime fechaBase = FechaUltimaDosis ?? model.FechaAplicada ?? DateTime.Now;
                        for (int i = dosisAplicadasPrevias; i >= 1; i--)
                        {
                            DateTime fechaHistorica = fechaBase.AddMonths(-frecuencia.Value * i); // Ajusta según unidadFrecuencia
                            using (var con = new SqlConnection(connectionString))
                            using (var cmd = new SqlCommand(@"
                        INSERT INTO Vacunaciones 
                        (MascotaId, VacunaId, Estado, FechaAplicada, FechaCreacion, FechaActualizacion)
                        VALUES (@MascotaId, @VacunaId, @Estado, @FechaAplicada, @FechaCreacion, @FechaActualizacion)", con))
                            {
                                cmd.Parameters.AddWithValue("@MascotaId", model.MascotaId);
                                cmd.Parameters.AddWithValue("@VacunaId", model.VacunaId);
                                cmd.Parameters.AddWithValue("@Estado", "Aplicada");
                                cmd.Parameters.AddWithValue("@FechaAplicada", fechaHistorica);
                                cmd.Parameters.AddWithValue("@FechaCreacion", DateTime.Now);
                                cmd.Parameters.AddWithValue("@FechaActualizacion", DateTime.Now);
                                con.Open();
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }

                    // Insertar la vacunación aplicada (sin FechaProgramada)
                    using (var con = new SqlConnection(connectionString))
                    using (var cmd = new SqlCommand(@"
                INSERT INTO Vacunaciones 
                (MascotaId, VacunaId, DosisAplicada, FechaAplicada, Lote, Observaciones, Estado, FechaCreacion, FechaActualizacion)
                VALUES (@MascotaId, @VacunaId, @DosisAplicada, @FechaAplicada, @Lote, @Observaciones, @Estado, @FechaCreacion, @FechaActualizacion)", con))
                    {
                        cmd.Parameters.AddWithValue("@MascotaId", model.MascotaId);
                        cmd.Parameters.AddWithValue("@VacunaId", model.VacunaId);
                        cmd.Parameters.AddWithValue("@DosisAplicada", (object)model.DosisAplicada ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@FechaAplicada", (object)model.FechaAplicada ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Lote", (object)model.Lote ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Observaciones", (object)model.Observaciones ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Estado", model.Estado);
                        cmd.Parameters.AddWithValue("@FechaCreacion", model.FechaCreacion);
                        cmd.Parameters.AddWithValue("@FechaActualizacion", DateTime.Now);
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }

                    // Obtener la última fecha aplicada (la actual)
                    DateTime? ultimaFechaAplicada = model.FechaAplicada;

                    // Si aún faltan dosis, crea solo la próxima pendiente
                    if (frecuencia.HasValue && !string.IsNullOrEmpty(unidadFrecuencia) && (totalDosis == 0 || dosisActual < totalDosis))
                    {
                        DateTime fechaProx = ultimaFechaAplicada.HasValue
                            ? CalcularProximaFecha(ultimaFechaAplicada.Value, frecuencia.Value, unidadFrecuencia).Value
                            : DateTime.Now;

                        using (var con = new SqlConnection(connectionString))
                        using (var cmd = new SqlCommand(@"
                    INSERT INTO Vacunaciones 
                    (MascotaId, VacunaId, Estado, FechaProgramada, FechaCreacion, FechaActualizacion)
                    VALUES (@MascotaId, @VacunaId, @Estado, @FechaProgramada, @FechaCreacion, @FechaActualizacion)", con))
                        {
                            cmd.Parameters.AddWithValue("@MascotaId", model.MascotaId);
                            cmd.Parameters.AddWithValue("@VacunaId", model.VacunaId);
                            cmd.Parameters.AddWithValue("@Estado", "Pendiente");
                            cmd.Parameters.AddWithValue("@FechaProgramada", fechaProx);
                            cmd.Parameters.AddWithValue("@FechaCreacion", DateTime.Now);
                            cmd.Parameters.AddWithValue("@FechaActualizacion", DateTime.Now);
                            con.Open();
                            cmd.ExecuteNonQuery();
                        }
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
            if (string.IsNullOrEmpty(model.Estado))
                model.Estado = "Aplicada";
            if (model.FechaCreacion == default(DateTime))
                model.FechaCreacion = DateTime.Now;
            if (model.Estado == "Pendiente" && model.FechaAplicada.HasValue)
                model.Estado = "Aplicada";
            if (ModelState.IsValid)
            {
                try
                {
                    // Si el registro estaba pendiente y ahora se marca como aplicada, crea la próxima pendiente si corresponde
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

                    int totalDosis = 0;
                    if (frecuencia.HasValue && duracion.HasValue && !string.IsNullOrEmpty(unidadFrecuencia) && !string.IsNullOrEmpty(unidadDuracion))
                    {
                        if (unidadFrecuencia == "Mes" && unidadDuracion == "Año")
                            totalDosis = duracion.Value * 12 / frecuencia.Value;
                        else if (unidadFrecuencia == "Mes" && unidadDuracion == "Mes")
                            totalDosis = duracion.Value / frecuencia.Value;
                        else if (unidadFrecuencia == "Año" && unidadDuracion == "Año")
                            totalDosis = duracion.Value / frecuencia.Value;
                        else if (unidadFrecuencia == "Semana" && unidadDuracion == "Mes")
                            totalDosis = (duracion.Value * 4) / frecuencia.Value;
                        else if (unidadFrecuencia == "Día" && unidadDuracion == "Mes")
                            totalDosis = (duracion.Value * 30) / frecuencia.Value;
                        else
                            totalDosis = duracion.Value;
                        if (totalDosis < 1) totalDosis = 1;
                    }

                    // Contar cuántas dosis aplicadas hay para esta mascota y vacuna (después de editar)
                    int aplicadas = 0;
                    DateTime? ultimaFechaAplicada = model.FechaAplicada;
                    using (var con = new SqlConnection(connectionString))
                    using (var cmd = new SqlCommand("SELECT COUNT(*), MAX(FechaAplicada) FROM Vacunaciones WHERE MascotaId=@MascotaId AND VacunaId=@VacunaId AND Estado='Aplicada'", con))
                    {
                        cmd.Parameters.AddWithValue("@MascotaId", model.MascotaId);
                        cmd.Parameters.AddWithValue("@VacunaId", model.VacunaId);
                        con.Open();
                        using (var dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                aplicadas = (dr.IsDBNull(0) ? 0 : dr.GetInt32(0)) + 1; // + 1 de la actual 
                                if (!dr.IsDBNull(1))
                                    ultimaFechaAplicada = dr.GetDateTime(1);
                            }
                        }
                    }

                    // Actualizar el registro editado
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

                    // Si el registro estaba pendiente y ahora se marca como aplicada, crea la próxima pendiente si corresponde
                    if (model.Estado == "Aplicada" && frecuencia.HasValue && !string.IsNullOrEmpty(unidadFrecuencia) && (totalDosis == 0 || aplicadas < totalDosis))
                    {
                        DateTime fechaProx = ultimaFechaAplicada.HasValue
                            ? CalcularProximaFecha(ultimaFechaAplicada.Value, frecuencia.Value, unidadFrecuencia).Value
                            : DateTime.Now;

                        using (var con = new SqlConnection(connectionString))
                        using (var cmd = new SqlCommand(@"
                    INSERT INTO Vacunaciones 
                    (MascotaId, VacunaId, Estado, FechaProgramada, FechaCreacion, FechaActualizacion)
                    VALUES (@MascotaId, @VacunaId, @Estado, @FechaProgramada, @FechaCreacion, @FechaActualizacion)", con))
                        {
                            cmd.Parameters.AddWithValue("@MascotaId", model.MascotaId);
                            cmd.Parameters.AddWithValue("@VacunaId", model.VacunaId);
                            cmd.Parameters.AddWithValue("@Estado", "Pendiente");
                            cmd.Parameters.AddWithValue("@FechaProgramada", fechaProx);
                            cmd.Parameters.AddWithValue("@FechaCreacion", DateTime.Now);
                            cmd.Parameters.AddWithValue("@FechaActualizacion", DateTime.Now);
                            con.Open();
                            cmd.ExecuteNonQuery();
                        }
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

        [HttpGet]
        public JsonResult ObtenerFrecuenciaUnidad(int vacunaId)
        {
            int frecuencia = 0;
            string unidad = "";
            using (var con = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand("SELECT Frecuencia, UnidadFrecuencia FROM Vacunas WHERE Id=@Id", con))
            {
                cmd.Parameters.AddWithValue("@Id", vacunaId);
                con.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        frecuencia = dr["Frecuencia"] != DBNull.Value ? Convert.ToInt32(dr["Frecuencia"]) : 0;
                        unidad = dr["UnidadFrecuencia"].ToString();
                    }
                }
            }
            return Json(new { frecuencia = frecuencia, unidad = unidad }, JsonRequestBehavior.AllowGet);
        }

    }
}