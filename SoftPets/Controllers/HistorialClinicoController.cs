using SoftPets.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace SoftPets.Controllers
{
    public class HistorialClinicoController : Controller
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["ConexionLocal"].ConnectionString;

        public ActionResult IndexEmergenciaPorDuenio(string estado = "")
        {
            int duenioId = (int)Session["DuenioId"];
            var lista = new List<HistorialClinico>();
            string query = @"SELECT h.*, m.Nombre AS NombreMascota FROM HistorialesClinicos h
                     INNER JOIN Mascotas m ON h.MascotaId = m.Id
                     WHERE h.DuenioId=@DuenioId AND h.Tipo='Emergencia'";

            if (estado == "proceso")
                query += " AND h.FechaFinTratamiento IS NULL";
            else if (estado == "finalizado")
                query += " AND h.FechaFinTratamiento IS NOT NULL";

            using (var con = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@DuenioId", duenioId);
                con.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new HistorialClinico
                        {
                            Id = (int)dr["Id"],
                            FechaInicioTratamiento = (DateTime)dr["FechaInicioTratamiento"],
                            FechaFinTratamiento = dr["FechaFinTratamiento"] != DBNull.Value ? (DateTime?)dr["FechaFinTratamiento"] : null,
                            FechaPosibleFinTratamiento = (DateTime)dr["FechaPosibleFinTratamiento"],
                            MascotaId = (int)dr["MascotaId"],
                            NombreMascota = dr["NombreMascota"].ToString(),
                            Motivo = dr["Motivo"].ToString(),
                            Tipo = dr["Tipo"].ToString(),
                            DuenioId = (int)dr["DuenioId"]
                        });
                    }
                }
            }
            if (Request.IsAjaxRequest())
                return PartialView("IndexEmergenciaPorDuenio", lista);
            ViewBag.DuenioId = duenioId;
            return View(lista);
        }

        public ActionResult IndexConsultaPorDuenio(string estado = "")
        {
            int duenioId = (int)Session["DuenioId"];
            var lista = new List<HistorialClinico>();
            string query = @"SELECT h.*, m.Nombre AS NombreMascota FROM HistorialesClinicos h
                     INNER JOIN Mascotas m ON h.MascotaId = m.Id
                     WHERE h.DuenioId=@DuenioId AND h.Tipo='Consulta'";

            if (estado == "proceso")
                query += " AND h.FechaFinTratamiento IS NULL";
            else if (estado == "finalizado")
                query += " AND h.FechaFinTratamiento IS NOT NULL";

            using (var con = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@DuenioId", duenioId);
                con.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new HistorialClinico
                        {
                            Id = (int)dr["Id"],
                            FechaInicioTratamiento = (DateTime)dr["FechaInicioTratamiento"],
                            FechaFinTratamiento = dr["FechaFinTratamiento"] != DBNull.Value ? (DateTime?)dr["FechaFinTratamiento"] : null,
                            FechaPosibleFinTratamiento = (DateTime)dr["FechaPosibleFinTratamiento"],
                            MascotaId = (int)dr["MascotaId"],
                            NombreMascota = dr["NombreMascota"].ToString(),
                            Motivo = dr["Motivo"].ToString(),
                            Tipo = dr["Tipo"].ToString(),
                            DuenioId = (int)dr["DuenioId"]
                        });
                    }
                }
            }
            if (Request.IsAjaxRequest())
                return PartialView("IndexConsultaPorDuenio", lista);
            ViewBag.DuenioId = duenioId;
            return View(lista);
        }



        public ActionResult IndexEmergenciaPorMascota(int mascotaId, string estado = "")
        {
            var lista = new List<HistorialClinico>();
            string query = @"SELECT * FROM HistorialesClinicos WHERE MascotaId=@MascotaId AND Tipo='Emergencia'";

            if (estado == "proceso")
                query += " AND FechaFinTratamiento IS NULL";
            else if (estado == "finalizado")
                query += " AND FechaFinTratamiento IS NOT NULL";

            using (var con = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@MascotaId", mascotaId);
                con.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new HistorialClinico
                        {
                            Id = (int)dr["Id"],
                            FechaInicioTratamiento = (DateTime)dr["FechaInicioTratamiento"],
                            FechaFinTratamiento = dr["FechaFinTratamiento"] != DBNull.Value ? (DateTime?)dr["FechaFinTratamiento"] : null,
                            FechaPosibleFinTratamiento = (DateTime)dr["FechaPosibleFinTratamiento"],
                            MascotaId = (int)dr["MascotaId"],
                            Motivo = dr["Motivo"].ToString(),
                            Tipo = dr["Tipo"].ToString(),
                            DuenioId = (int)dr["DuenioId"]
                        });
                    }
                }
            }
            if (Request.IsAjaxRequest())
                return PartialView("IndexEmergenciaPorMascota", lista);
            ViewBag.MascotaId = mascotaId;
            ViewBag.NombreMascota = ObtenerNombreMascota(mascotaId);
            return View(lista);
        }


        public ActionResult IndexConsultaPorMascota(int mascotaId, string estado = "")
        {
            var lista = new List<HistorialClinico>();
            string query = @"SELECT * FROM HistorialesClinicos WHERE MascotaId=@MascotaId AND Tipo='Consulta'";

            if (estado == "proceso")
                query += " AND FechaFinTratamiento IS NULL";
            else if (estado == "finalizado")
                query += " AND FechaFinTratamiento IS NOT NULL";

            using (var con = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@MascotaId", mascotaId);
                con.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new HistorialClinico
                        {
                            Id = (int)dr["Id"],
                            FechaInicioTratamiento = (DateTime)dr["FechaInicioTratamiento"],
                            FechaFinTratamiento = dr["FechaFinTratamiento"] != DBNull.Value ? (DateTime?)dr["FechaFinTratamiento"] : null,
                            FechaPosibleFinTratamiento = (DateTime)dr["FechaPosibleFinTratamiento"],
                            MascotaId = (int)dr["MascotaId"],
                            Motivo = dr["Motivo"].ToString(),
                            Tipo = dr["Tipo"].ToString(),
                            DuenioId = (int)dr["DuenioId"]
                        });
                    }
                }
            }
            if (Request.IsAjaxRequest())
                return PartialView("IndexConsultaPorMascota", lista);
            ViewBag.MascotaId = mascotaId;
            ViewBag.NombreMascota = ObtenerNombreMascota(mascotaId);
            return View(lista);
        }

        private List<SelectListItem> ObtenerMascotasPorDuenio(int duenioId)
        {
            var mascotas = new List<SelectListItem>();
            using (var con = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand("SELECT Id, Nombre FROM Mascotas WHERE DuenioId=@DuenioId", con))
            {
                cmd.Parameters.AddWithValue("@DuenioId", duenioId);
                con.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        mascotas.Add(new SelectListItem
                        {
                            Value = dr["Id"].ToString(),
                            Text = dr["Nombre"].ToString()
                        });
                    }
                }
            }
            return mascotas;
        }
        public ActionResult Create(int? mascotaId, string tipo)
        {
            int? duenioId = null;
            if (mascotaId != null)
            {
                // Buscar el DuenioId de la mascota seleccionada
                using (var con = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand("SELECT DuenioId, Nombre FROM Mascotas WHERE Id=@Id", con))
                {
                    cmd.Parameters.AddWithValue("@Id", mascotaId.Value);
                    con.Open();
                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            duenioId = (int)dr["DuenioId"];
                            ViewBag.NombreMascota = dr["Nombre"].ToString();
                        }
                    }
                }
                ViewBag.MascotaId = mascotaId;
            }
            else
            {
                // Si no hay mascota seleccionada, obtener el duenioId de la sesión
                duenioId = Session["DuenioId"] as int?;
                if (duenioId != null)
                    ViewBag.Mascotas = ObtenerMascotasPorDuenio(duenioId.Value);
            }
            ViewBag.DuenioId = duenioId;
            ViewBag.Tipo = tipo ?? "Consulta";
            var model = new HistorialClinico
            {
                FechaInicioTratamiento = DateTime.Now.Date,
                FechaPosibleFinTratamiento = DateTime.Now.Date.AddDays(7),
                DuenioId = duenioId ?? 0,
                MascotaId = mascotaId ?? 0,
                Tipo = tipo ?? "Consulta"
            };
            return View(model);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(HistorialClinico model)
        {
            // Si no viene DuenioId, buscarlo por MascotaId
            if ((model.DuenioId == 0 || model.DuenioId == null) && model.MascotaId != 0)
            {
                using (var con = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand("SELECT DuenioId FROM Mascotas WHERE Id=@Id", con))
                {
                    cmd.Parameters.AddWithValue("@Id", model.MascotaId);
                    con.Open();
                    var result = cmd.ExecuteScalar();
                    if (result != null)
                        model.DuenioId = Convert.ToInt32(result);
                }
            }
            if (ModelState.IsValid)
            {
                try
                {
                    using (var con = new SqlConnection(connectionString))
                    using (var cmd = new SqlCommand(@"INSERT INTO HistorialesClinicos 
                (FechaInicioTratamiento, FechaFinTratamiento, FechaPosibleFinTratamiento, MascotaId, Motivo, Tipo, DuenioId)
                VALUES (@FechaInicioTratamiento, @FechaFinTratamiento, @FechaPosibleFinTratamiento, @MascotaId, @Motivo, @Tipo, @DuenioId)", con))
                    {
                        cmd.Parameters.AddWithValue("@FechaInicioTratamiento", model.FechaInicioTratamiento);
                        cmd.Parameters.AddWithValue("@FechaFinTratamiento", (object)model.FechaFinTratamiento ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@FechaPosibleFinTratamiento", model.FechaPosibleFinTratamiento);
                        cmd.Parameters.AddWithValue("@MascotaId", model.MascotaId);
                        cmd.Parameters.AddWithValue("@Motivo", model.Motivo);
                        cmd.Parameters.AddWithValue("@Tipo", model.Tipo);
                        cmd.Parameters.AddWithValue("@DuenioId", model.DuenioId);
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                    TempData["SwalMascotaEditada"] = "Historial creado correctamente";
                    if (Request["mascotaId"] != null)
                    {
                        if (model.Tipo == "Emergencia")
                            return RedirectToAction("IndexEmergenciaPorMascota", new { mascotaId = model.MascotaId });
                        else
                            return RedirectToAction("IndexConsultaPorMascota", new { mascotaId = model.MascotaId });
                    }
                    else
                    {
                        if (model.Tipo == "Emergencia")
                            return RedirectToAction("IndexEmergenciaPorDuenio", new { duenioId = model.DuenioId });
                        else
                            return RedirectToAction("IndexConsultaPorDuenio", new { duenioId = model.DuenioId });
                    }
                }
                catch (Exception ex)
                {
                    TempData["SwalError"] = "Error al crear historial: " + ex.Message;
                }
            }
            // Si hay error, recargar combos
            ViewBag.DuenioId = model.DuenioId;
            ViewBag.Tipo = model.Tipo;
            if (model.MascotaId == 0 && model.DuenioId != 0)
                ViewBag.Mascotas = ObtenerMascotasPorDuenio(model.DuenioId);
            else
                ViewBag.MascotaId = model.MascotaId;
            return View(model);
        }


        public ActionResult Edit(int id)
        {
            HistorialClinico model = null;
            using (var con = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand("SELECT * FROM HistorialesClinicos WHERE Id=@Id", con))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                con.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        model = new HistorialClinico
                        {
                            Id = (int)dr["Id"],
                            FechaInicioTratamiento = (DateTime)dr["FechaInicioTratamiento"],
                            FechaFinTratamiento = dr["FechaFinTratamiento"] != DBNull.Value ? (DateTime?)dr["FechaFinTratamiento"] : null,
                            FechaPosibleFinTratamiento = (DateTime)dr["FechaPosibleFinTratamiento"],
                            MascotaId = (int)dr["MascotaId"],
                            Motivo = dr["Motivo"].ToString(),
                            Tipo = dr["Tipo"].ToString(),
                            DuenioId = (int)dr["DuenioId"]
                        };
                    }
                }
            }
            if (model == null)
                return HttpNotFound();
            ViewBag.MascotaId = model.MascotaId;
            ViewBag.NombreMascota = ObtenerNombreMascota(model.MascotaId);
            ViewBag.Tipo = model.Tipo;
            return View(model);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(HistorialClinico model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (var con = new SqlConnection(connectionString))
                    using (var cmd = new SqlCommand(@"UPDATE HistorialesClinicos SET 
                FechaInicioTratamiento=@FechaInicioTratamiento, 
                FechaFinTratamiento=@FechaFinTratamiento, 
                FechaPosibleFinTratamiento=@FechaPosibleFinTratamiento, 
                Motivo=@Motivo
                WHERE Id=@Id", con))
                    {
                        cmd.Parameters.AddWithValue("@FechaInicioTratamiento", model.FechaInicioTratamiento);
                        cmd.Parameters.AddWithValue("@FechaFinTratamiento", (object)model.FechaFinTratamiento ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@FechaPosibleFinTratamiento", model.FechaPosibleFinTratamiento);
                        cmd.Parameters.AddWithValue("@Motivo", model.Motivo);
                        cmd.Parameters.AddWithValue("@Id", model.Id);
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                    TempData["SwalMascotaEditada"] = "Historial editado correctamente";
                    // Redirección según origen
                    if (model.Tipo == "Emergencia")
                        return RedirectToAction("IndexEmergenciaPorMascota", new { mascotaId = model.MascotaId });
                    else
                        return RedirectToAction("IndexConsultaPorMascota", new { mascotaId = model.MascotaId });
                }
                catch (Exception ex)
                {
                    TempData["SwalError"] = "Error al editar historial: " + ex.Message;
                }
            }
            ViewBag.MascotaId = model.MascotaId;
            ViewBag.NombreMascota = ObtenerNombreMascota(model.MascotaId);
            ViewBag.Tipo = model.Tipo;
            return View(model);
        }
        public ActionResult Details(int id)
        {
            HistorialClinico model = null;
            using (var con = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(@"
                SELECT h.*, m.Nombre AS NombreMascota, d.Nombres + ' ' + d.Apellidos AS NombreDuenio
                FROM HistorialesClinicos h
                INNER JOIN Mascotas m ON h.MascotaId = m.Id
                INNER JOIN Duenios d ON h.DuenioId = d.Id
                WHERE Id=@Id", con))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                con.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        model = new HistorialClinico
                        {
                            Id = (int)dr["Id"],
                            FechaInicioTratamiento = (DateTime)dr["FechaInicioTratamiento"],
                            FechaFinTratamiento = dr["FechaFinTratamiento"] != DBNull.Value ? (DateTime?)dr["FechaFinTratamiento"] : null,
                            FechaPosibleFinTratamiento = (DateTime)dr["FechaPosibleFinTratamiento"],
                            MascotaId = (int)dr["MascotaId"],
                            Motivo = dr["Motivo"].ToString(),
                            Tipo = dr["Tipo"].ToString(),
                            DuenioId = (int)dr["DuenioId"],
                            NombreMascota = dr["NombreMascota"].ToString(),
                            NombreDuenio = dr["NombreDuenio"].ToString()
                        };
                    }
                }
            }
            if (model == null)
                return HttpNotFound();
            return View(model);
        }
        [HttpPost]
        public ActionResult CerrarHistorial(int id)
        {
            try
            {
                using (var con = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand(@"UPDATE HistorialesClinicos SET FechaFinTratamiento=@FechaFin WHERE Id=@Id", con))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@FechaFin", DateTime.Now);
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

        // Helper para obtener nombre de la mascota
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