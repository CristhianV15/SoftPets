using SoftPets.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace SoftPets.Controllers
{
    public class DosisIndicacionHistorialClinicoController : Controller
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["ConexionLocal"].ConnectionString;

        public ActionResult FiltrarAjax(string estado = "", string motivo = "", string mascota = "", string fechaInicio = "", string fechaFin = "")
        {
            int duenioId = (int)Session["DuenioId"];
            var lista = new List<DosisIndicacionHistorialClinicoGeneralVM>();

            string query = @"
        SELECT d.Id, d.Dosis, d.IntervaloCantidad, d.IntervaloTipo, d.CantidadTotalDosis, d.FechaInicioDosis, d.FechaFinDosis, d.EstadoAlerta,
               i.Medicamento, i.Indicacion,
               m.Nombre AS NombreMascota,
               h.Motivo
        FROM DosisIndicacionesHistorialesClinicos d
        INNER JOIN IndicacionesHistorialesClinicos i ON d.IndicacionesHistorialesClinicosId = i.Id
        INNER JOIN DetallesHistorialesClinicos dt ON i.DetallesHistorialesClinicosId = dt.Id
        INNER JOIN HistorialesClinicos h ON dt.HistorialClinicoId = h.Id
        INNER JOIN Mascotas m ON h.MascotaId = m.Id
        WHERE h.DuenioId = @DuenioId
    ";

            if (!string.IsNullOrEmpty(estado))
                query += " AND d.EstadoAlerta = @Estado";
            if (!string.IsNullOrEmpty(motivo))
                query += " AND h.Motivo LIKE @Motivo";
            if (!string.IsNullOrEmpty(mascota))
                query += " AND m.Nombre LIKE @Mascota";
            if (!string.IsNullOrEmpty(fechaInicio))
                query += " AND d.FechaInicioDosis >= @FechaInicio";
            if (!string.IsNullOrEmpty(fechaFin))
                query += " AND d.FechaFinDosis <= @FechaFin";

            using (var con = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@DuenioId", duenioId);
                if (!string.IsNullOrEmpty(estado))
                    cmd.Parameters.AddWithValue("@Estado", estado);
                if (!string.IsNullOrEmpty(motivo))
                    cmd.Parameters.AddWithValue("@Motivo", "%" + motivo + "%");
                if (!string.IsNullOrEmpty(mascota))
                    cmd.Parameters.AddWithValue("@Mascota", "%" + mascota + "%");
                if (!string.IsNullOrEmpty(fechaInicio))
                    cmd.Parameters.AddWithValue("@FechaInicio", DateTime.Parse(fechaInicio));
                if (!string.IsNullOrEmpty(fechaFin))
                    cmd.Parameters.AddWithValue("@FechaFin", DateTime.Parse(fechaFin));

                con.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new DosisIndicacionHistorialClinicoGeneralVM
                        {
                            Id = (int)dr["Id"],
                            Dosis = dr["Dosis"].ToString(),
                            IntervaloCantidad = (int)dr["IntervaloCantidad"],
                            IntervaloTipo = dr["IntervaloTipo"].ToString(),
                            CantidadTotalDosis = dr["CantidadTotalDosis"] != DBNull.Value ? (int?)dr["CantidadTotalDosis"] : null,
                            FechaInicioDosis = (DateTime)dr["FechaInicioDosis"],
                            FechaFinDosis = dr["FechaFinDosis"] != DBNull.Value ? (DateTime?)dr["FechaFinDosis"] : null,
                            EstadoAlerta = dr["EstadoAlerta"].ToString(),
                            Medicamento = dr["Medicamento"].ToString(),
                            Indicacion = dr["Indicacion"].ToString(),
                            NombreMascota = dr["NombreMascota"].ToString(),
                            Motivo = dr["Motivo"].ToString()
                        });
                    }
                }
            }
            return PartialView("_TablaDosis", lista);
        }
        public ActionResult IndexGeneral(string estado = "", string motivo = "", string mascota = "", string fechaInicio = "", string fechaFin = "")
        {
            int duenioId = (int)Session["DuenioId"];
            var lista = new List<DosisIndicacionHistorialClinicoGeneralVM>();

            string query = @"
SELECT d.Id, d.Dosis, d.IntervaloCantidad, d.IntervaloTipo, d.CantidadTotalDosis, d.FechaInicioDosis, d.FechaFinDosis, d.EstadoAlerta,
       i.Medicamento, i.Indicacion,
       m.Nombre AS NombreMascota,
       h.Motivo
FROM DosisIndicacionesHistorialesClinicos d
INNER JOIN IndicacionesHistorialesClinicos i ON d.IndicacionesHistorialesClinicosId = i.Id
INNER JOIN DetallesHistorialesClinicos dt ON i.DetallesHistorialesClinicosId = dt.Id
INNER JOIN HistorialesClinicos h ON dt.HistorialClinicoId = h.Id
INNER JOIN Mascotas m ON h.MascotaId = m.Id
WHERE h.DuenioId = @DuenioId
    ";

            // Filtros dinámicos
            if (!string.IsNullOrEmpty(estado))
                query += " AND d.EstadoAlerta = @Estado";
            if (!string.IsNullOrEmpty(motivo))
                query += " AND h.Motivo LIKE @Motivo";
            if (!string.IsNullOrEmpty(mascota))
                query += " AND m.Nombre LIKE @Mascota";
            if (!string.IsNullOrEmpty(fechaInicio))
                query += " AND d.FechaInicioDosis >= @FechaInicio";
            if (!string.IsNullOrEmpty(fechaFin))
                query += " AND d.FechaFinDosis <= @FechaFin";

            using (var con = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@DuenioId", duenioId);
                if (!string.IsNullOrEmpty(estado))
                    cmd.Parameters.AddWithValue("@Estado", estado);
                if (!string.IsNullOrEmpty(motivo))
                    cmd.Parameters.AddWithValue("@Motivo", "%" + motivo + "%");
                if (!string.IsNullOrEmpty(mascota))
                    cmd.Parameters.AddWithValue("@Mascota", "%" + mascota + "%");
                if (!string.IsNullOrEmpty(fechaInicio))
                    cmd.Parameters.AddWithValue("@FechaInicio", DateTime.Parse(fechaInicio));
                if (!string.IsNullOrEmpty(fechaFin))
                    cmd.Parameters.AddWithValue("@FechaFin", DateTime.Parse(fechaFin));

                con.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new DosisIndicacionHistorialClinicoGeneralVM
                        {
                            Id = (int)dr["Id"],
                            Dosis = dr["Dosis"].ToString(),
                            IntervaloCantidad = (int)dr["IntervaloCantidad"],
                            IntervaloTipo = dr["IntervaloTipo"].ToString(),
                            CantidadTotalDosis = dr["CantidadTotalDosis"] != DBNull.Value ? (int?)dr["CantidadTotalDosis"] : null,
                            FechaInicioDosis = (DateTime)dr["FechaInicioDosis"],
                            FechaFinDosis = dr["FechaFinDosis"] != DBNull.Value ? (DateTime?)dr["FechaFinDosis"] : null,
                            EstadoAlerta = dr["EstadoAlerta"].ToString(),
                            Medicamento = dr["Medicamento"].ToString(),
                            Indicacion = dr["Indicacion"].ToString(),
                            NombreMascota = dr["NombreMascota"].ToString(),
                            Motivo = dr["Motivo"].ToString()
                        });
                    }
                }
            }
            ViewBag.Estado = estado;
            ViewBag.Motivo = motivo;
            ViewBag.Mascota = mascota;
            ViewBag.FechaInicio = fechaInicio;
            ViewBag.FechaFin = fechaFin;
            return View("IndexGeneral", lista);
        }



        // Listar dosis de una indicación
       public ActionResult Index(int IndicacionesHistorialesClinicosId)
{
    var lista = new List<DosisIndicacionHistorialClinico>();
    string medicamento = "";

    try
    {
        // Obtener el medicamento de la indicación seleccionada
        using (var con = new SqlConnection(connectionString))
        using (var cmd = new SqlCommand(@"SELECT Medicamento FROM IndicacionesHistorialesClinicos WHERE Id = @Id", con))
        {
            cmd.Parameters.AddWithValue("@Id", IndicacionesHistorialesClinicosId);
            con.Open();
            var result = cmd.ExecuteScalar();
            if (result != null)
                medicamento = result.ToString();
        }

        // Obtener las dosis
        using (var con = new SqlConnection(connectionString))
        using (var cmd = new SqlCommand(@"SELECT * FROM DosisIndicacionesHistorialesClinicos WHERE IndicacionesHistorialesClinicosId=@IndicacionesHistorialesClinicosId", con))
        {
            cmd.Parameters.AddWithValue("@IndicacionesHistorialesClinicosId", IndicacionesHistorialesClinicosId);
            con.Open();
            using (var dr = cmd.ExecuteReader())
            {
                while (dr.Read())
                {
                    lista.Add(new DosisIndicacionHistorialClinico
                    {
                        Id = (int)dr["Id"],
                        IndicacionesHistorialesClinicosId = (int)dr["IndicacionesHistorialesClinicosId"],
                        Dosis = dr["Dosis"].ToString(),
                        IntervaloCantidad = (int)dr["IntervaloCantidad"],
                        IntervaloTipo = dr["IntervaloTipo"].ToString(),
                        CantidadTotalDosis = dr["CantidadTotalDosis"] != DBNull.Value ? (int?)dr["CantidadTotalDosis"] : null,
                        FechaInicioDosis = (DateTime)dr["FechaInicioDosis"],
                        FechaFinDosis = dr["FechaFinDosis"] != DBNull.Value ? (DateTime?)dr["FechaFinDosis"] : null,
                        EstadoAlerta = dr["EstadoAlerta"].ToString()
                    });
                }
            }
        }
        ViewBag.IndicacionesHistorialesClinicosId = IndicacionesHistorialesClinicosId;
        ViewBag.Medicamento = medicamento;
    }
    catch (Exception ex)
    {
        TempData["SwalError"] = "Error al cargar dosis: " + ex.Message;
    }
    return View(lista);
}
        public ActionResult Create(int IndicacionesHistorialesClinicosId)
        {
            ViewBag.IndicacionesHistorialesClinicosId = IndicacionesHistorialesClinicosId;
            var model = new DosisIndicacionHistorialClinico
            {
                IndicacionesHistorialesClinicosId = IndicacionesHistorialesClinicosId,
                FechaInicioDosis = DateTime.Now,
                FechaFinDosis = DateTime.Now // El JS lo actualizará automáticamente
            };
            return View(model);
        }



        public ActionResult Grafico()
        {
            int duenioId = (int)Session["DuenioId"];
            var lista = new List<DosisIndicacionHistorialClinicoGeneralVM>();

            string query = @"
        SELECT d.Id, d.Dosis, d.IntervaloCantidad, d.IntervaloTipo, d.CantidadTotalDosis, d.FechaInicioDosis, d.FechaFinDosis, d.EstadoAlerta,
               i.Medicamento, i.Indicacion,
               m.Nombre AS NombreMascota,
               h.Motivo
        FROM DosisIndicacionesHistorialesClinicos d
        INNER JOIN IndicacionesHistorialesClinicos i ON d.IndicacionesHistorialesClinicosId = i.Id
        INNER JOIN DetallesHistorialesClinicos dt ON i.DetallesHistorialesClinicosId = dt.Id
        INNER JOIN HistorialesClinicos h ON dt.HistorialClinicoId = h.Id
        INNER JOIN Mascotas m ON h.MascotaId = m.Id
        WHERE h.DuenioId = @DuenioId
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
                        lista.Add(new DosisIndicacionHistorialClinicoGeneralVM
                        {
                            Id = (int)dr["Id"],
                            Dosis = dr["Dosis"].ToString(),
                            IntervaloCantidad = (int)dr["IntervaloCantidad"],
                            IntervaloTipo = dr["IntervaloTipo"].ToString(),
                            CantidadTotalDosis = dr["CantidadTotalDosis"] != DBNull.Value ? (int?)dr["CantidadTotalDosis"] : null,
                            FechaInicioDosis = (DateTime)dr["FechaInicioDosis"],
                            FechaFinDosis = dr["FechaFinDosis"] != DBNull.Value ? (DateTime?)dr["FechaFinDosis"] : null,
                            EstadoAlerta = dr["EstadoAlerta"].ToString(),
                            Medicamento = dr["Medicamento"].ToString(),
                            Indicacion = dr["Indicacion"].ToString(),
                            NombreMascota = dr["NombreMascota"].ToString(),
                            Motivo = dr["Motivo"].ToString()
                        });
                    }
                }
            }
            return View("Grafico", lista);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DosisIndicacionHistorialClinico model)
        {
            int dosisId = 0;
            if (ModelState.IsValid)
            {
                try
                {
                    using (var con = new SqlConnection(connectionString))
                    using (var cmd = new SqlCommand(@"INSERT INTO DosisIndicacionesHistorialesClinicos 
                (IndicacionesHistorialesClinicosId, Dosis, IntervaloCantidad, IntervaloTipo, CantidadTotalDosis, FechaInicioDosis, FechaFinDosis, EstadoAlerta)
                OUTPUT INSERTED.Id
                VALUES (@IndicacionesHistorialesClinicosId, @Dosis, @IntervaloCantidad, @IntervaloTipo, @CantidadTotalDosis, @FechaInicioDosis, @FechaFinDosis, @EstadoAlerta)", con))
                    {
                        cmd.Parameters.AddWithValue("@IndicacionesHistorialesClinicosId", model.IndicacionesHistorialesClinicosId);
                        cmd.Parameters.AddWithValue("@Dosis", model.Dosis);
                        cmd.Parameters.AddWithValue("@IntervaloCantidad", model.IntervaloCantidad);
                        cmd.Parameters.AddWithValue("@IntervaloTipo", model.IntervaloTipo);
                        cmd.Parameters.AddWithValue("@CantidadTotalDosis", (object)model.CantidadTotalDosis ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@FechaInicioDosis", model.FechaInicioDosis);
                        cmd.Parameters.AddWithValue("@FechaFinDosis", (object)model.FechaFinDosis ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@EstadoAlerta", model.EstadoAlerta);
                        con.Open();
                        dosisId = (int)cmd.ExecuteScalar(); // Obtener el ID insertado
                    }

                    // LLAMADA CRÍTICA: Generar las tomas automáticamente
                    int cantidadTotal = model.CantidadTotalDosis ?? 1;
                    new TomasDosisIndicacionHistorialClinicoController().GenerarTomas(
                        dosisId,
                        model.FechaInicioDosis,
                        model.IntervaloCantidad,
                        model.IntervaloTipo,
                        cantidadTotal
                    );

                    TempData["SwalMascotaEditada"] = "Dosis y tomas creadas correctamente";
                    return RedirectToAction("Index", new { IndicacionesHistorialesClinicosId = model.IndicacionesHistorialesClinicosId });
                }
                catch (Exception ex)
                {
                    TempData["SwalError"] = "Error al crear dosis: " + ex.Message;
                }
            }
            ViewBag.IndicacionesHistorialesClinicosId = model.IndicacionesHistorialesClinicosId;
            return View(model);
        }

        // Ver dosis
        public ActionResult Details(int id)
        {
            DosisIndicacionHistorialClinico model = null;
            try
            {
                using (var con = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand(@"SELECT * FROM DosisIndicacionesHistorialesClinicos WHERE Id=@Id", con))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    con.Open();
                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            model = new DosisIndicacionHistorialClinico
                            {
                                Id = (int)dr["Id"],
                                IndicacionesHistorialesClinicosId = (int)dr["IndicacionesHistorialesClinicosId"],
                                Dosis = dr["Dosis"].ToString(),
                                IntervaloCantidad = (int)dr["IntervaloCantidad"],
                                IntervaloTipo = dr["IntervaloTipo"].ToString(),
                                CantidadTotalDosis = dr["CantidadTotalDosis"] != DBNull.Value ? (int?)dr["CantidadTotalDosis"] : null,
                                FechaInicioDosis = (DateTime)dr["FechaInicioDosis"],
                                FechaFinDosis = dr["FechaFinDosis"] != DBNull.Value ? (DateTime?)dr["FechaFinDosis"] : null,
                                EstadoAlerta = dr["EstadoAlerta"].ToString()
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["SwalError"] = "Error al cargar dosis: " + ex.Message;
            }
            return View(model);
        }

        // Editar dosis
        public ActionResult Edit(int id)
        {
            return Details(id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DosisIndicacionHistorialClinico model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (var con = new SqlConnection(connectionString))
                    using (var cmd = new SqlCommand(@"UPDATE DosisIndicacionesHistorialesClinicos SET 
                        Dosis=@Dosis, 
                        IntervaloCantidad=@IntervaloCantidad, 
                        IntervaloTipo=@IntervaloTipo, 
                        CantidadTotalDosis=@CantidadTotalDosis, 
                        FechaInicioDosis=@FechaInicioDosis, 
                        FechaFinDosis=@FechaFinDosis, 
                        EstadoAlerta=@EstadoAlerta
                        WHERE Id=@Id", con))
                    {
                        cmd.Parameters.AddWithValue("@Dosis", model.Dosis);
                        cmd.Parameters.AddWithValue("@IntervaloCantidad", model.IntervaloCantidad);
                        cmd.Parameters.AddWithValue("@IntervaloTipo", model.IntervaloTipo);
                        cmd.Parameters.AddWithValue("@CantidadTotalDosis", (object)model.CantidadTotalDosis ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@FechaInicioDosis", model.FechaInicioDosis);
                        cmd.Parameters.AddWithValue("@FechaFinDosis", (object)model.FechaFinDosis ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@EstadoAlerta", model.EstadoAlerta);
                        cmd.Parameters.AddWithValue("@Id", model.Id);
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                    TempData["SwalMascotaEditada"] = "Dosis editada correctamente";
                    return RedirectToAction("Index", new { IndicacionesHistorialesClinicosId = model.IndicacionesHistorialesClinicosId });
                }
                catch (Exception ex)
                {
                    TempData["SwalError"] = "Error al editar dosis: " + ex.Message;
                }
            }
            ViewBag.IndicacionesHistorialesClinicosId = model.IndicacionesHistorialesClinicosId;
            return View(model);
        }

        // Eliminar dosis
        public ActionResult Delete(int id, int IndicacionesHistorialesClinicosId)
        {
            try
            {
                using (var con = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand("DELETE FROM DosisIndicacionesHistorialesClinicos WHERE Id=@Id", con))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                TempData["SwalMascotaEditada"] = "Dosis eliminada correctamente";
            }
            catch (Exception ex)
            {
                TempData["SwalError"] = "Error al eliminar dosis: " + ex.Message;
            }
            return RedirectToAction("Index", new { IndicacionesHistorialesClinicosId = IndicacionesHistorialesClinicosId });
        }
    }
}