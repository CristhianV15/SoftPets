using SoftPets.Filters;
using SoftPets.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftPets.Controllers
{
    [RequiereDatosCompletos]

    public class VacunasController : Controller
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["ConexionLocal"].ConnectionString;

        public ActionResult Index(string tipo = "", string estado = "")
        {
            var lista = new List<Vacuna>();
            string query = "SELECT * FROM Vacunas WHERE 1=1";
            if (!string.IsNullOrEmpty(tipo))
                query += " AND Tipo = @Tipo";
            if (!string.IsNullOrEmpty(estado))
                query += " AND Estado = @Estado";

            using (var con = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(query, con))
            {
                if (!string.IsNullOrEmpty(tipo))
                    cmd.Parameters.AddWithValue("@Tipo", tipo);
                if (!string.IsNullOrEmpty(estado))
                    cmd.Parameters.AddWithValue("@Estado", estado);

                con.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new Vacuna
                        {
                            Id = (int)dr["Id"],
                            Nombre = dr["Nombre"].ToString(),
                            Descripcion = dr["Descripcion"].ToString(),
                            Lote = dr["Lote"].ToString(),
                            Estado = dr["Estado"].ToString(),
                            FechaCreacion = (DateTime)dr["FechaCreacion"],
                            FechaActualizacion = dr["FechaActualizacion"] != DBNull.Value ? (DateTime?)dr["FechaActualizacion"] : null,
                            Tipo = dr["Tipo"].ToString(),
                            Frecuencia = dr["Frecuencia"] != DBNull.Value ? (int?)dr["Frecuencia"] : null,
                            UnidadFrecuencia = dr["UnidadFrecuencia"].ToString(),
                            Duracion = dr["Duracion"] != DBNull.Value ? (int?)dr["Duracion"] : null,
                            UnidadDuracion = dr["UnidadDuracion"].ToString(),
                            RangoDosis = dr["RangoDosis"].ToString()
                        });
                    }
                }
            }
            ViewBag.Tipo = tipo;
            ViewBag.Estado = estado;
            return View(lista);
        }

        public ActionResult FiltrarAjax(string tipo = "", string estado = "")
        {
            var lista = new List<Vacuna>();
            string query = "SELECT * FROM Vacunas WHERE 1=1";
            if (!string.IsNullOrEmpty(tipo))
                query += " AND Tipo = @Tipo";
            if (!string.IsNullOrEmpty(estado))
                query += " AND Estado = @Estado";

            using (var con = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(query, con))
            {
                if (!string.IsNullOrEmpty(tipo))
                    cmd.Parameters.AddWithValue("@Tipo", tipo);
                if (!string.IsNullOrEmpty(estado))
                    cmd.Parameters.AddWithValue("@Estado", estado);

                con.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new Vacuna
                        {
                            Id = (int)dr["Id"],
                            Nombre = dr["Nombre"].ToString(),
                            Descripcion = dr["Descripcion"].ToString(),
                            Lote = dr["Lote"].ToString(),
                            Estado = dr["Estado"].ToString(),
                            FechaCreacion = (DateTime)dr["FechaCreacion"],
                            FechaActualizacion = dr["FechaActualizacion"] != DBNull.Value ? (DateTime?)dr["FechaActualizacion"] : null,
                            Tipo = dr["Tipo"].ToString(),
                            Frecuencia = dr["Frecuencia"] != DBNull.Value ? (int?)dr["Frecuencia"] : null,
                            UnidadFrecuencia = dr["UnidadFrecuencia"].ToString(),
                            Duracion = dr["Duracion"] != DBNull.Value ? (int?)dr["Duracion"] : null,
                            UnidadDuracion = dr["UnidadDuracion"].ToString(),
                            RangoDosis = dr["RangoDosis"].ToString()
                        });
                    }
                }
            }
            return PartialView("_TablaVacunas", lista);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Vacuna model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (var con = new SqlConnection(connectionString))
                    using (var cmd = new SqlCommand(@"
                        INSERT INTO Vacunas 
                        (Nombre, Descripcion, Estado, FechaCreacion, FechaActualizacion, Tipo, Frecuencia, UnidadFrecuencia, Duracion, UnidadDuracion, RangoDosis)
                        VALUES (@Nombre, @Descripcion, @Estado, @FechaCreacion, @FechaActualizacion, @Tipo, @Frecuencia, @UnidadFrecuencia, @Duracion, @UnidadDuracion, @RangoDosis)", con))
                    {
                        cmd.Parameters.AddWithValue("@Nombre", model.Nombre);
                        cmd.Parameters.AddWithValue("@Descripcion", (object)model.Descripcion ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Estado", model.Estado);
                        cmd.Parameters.AddWithValue("@FechaCreacion", DateTime.Now);
                        cmd.Parameters.AddWithValue("@FechaActualizacion", DateTime.Now);
                        cmd.Parameters.AddWithValue("@Tipo", model.Tipo);
                        cmd.Parameters.AddWithValue("@Frecuencia", (object)model.Frecuencia ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@UnidadFrecuencia", (object)model.UnidadFrecuencia ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Duracion", (object)model.Duracion ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@UnidadDuracion", (object)model.UnidadDuracion ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@RangoDosis", (object)model.RangoDosis ?? DBNull.Value);
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                    TempData["SwalMascotaEditada"] = "Vacuna/Pastilla registrada correctamente";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    TempData["SwalError"] = "Error al registrar: " + ex.Message;
                }
            }
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            Vacuna model = null;
            using (var con = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand("SELECT * FROM Vacunas WHERE Id=@Id", con))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                con.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        model = new Vacuna
                        {
                            Id = (int)dr["Id"],
                            Nombre = dr["Nombre"].ToString(),
                            Descripcion = dr["Descripcion"].ToString(),
                            Estado = dr["Estado"].ToString(),
                            FechaCreacion = (DateTime)dr["FechaCreacion"],
                            FechaActualizacion = dr["FechaActualizacion"] != DBNull.Value ? (DateTime?)dr["FechaActualizacion"] : null,
                            Tipo = dr["Tipo"].ToString(),
                            Frecuencia = dr["Frecuencia"] != DBNull.Value ? (int?)dr["Frecuencia"] : null,
                            UnidadFrecuencia = dr["UnidadFrecuencia"].ToString(),
                            Duracion = dr["Duracion"] != DBNull.Value ? (int?)dr["Duracion"] : null,
                            UnidadDuracion = dr["UnidadDuracion"].ToString(),
                            RangoDosis = dr["RangoDosis"].ToString()
                        };
                    }
                }
            }
            if (model == null)
                return HttpNotFound();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Vacuna model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (var con = new SqlConnection(connectionString))
                    using (var cmd = new SqlCommand(@"
                        UPDATE Vacunas SET 
                            Nombre=@Nombre, Descripcion=@Descripcion, Estado=@Estado, 
                            FechaActualizacion=@FechaActualizacion, Tipo=@Tipo, 
                            Frecuencia=@Frecuencia, UnidadFrecuencia=@UnidadFrecuencia, 
                            Duracion=@Duracion, UnidadDuracion=@UnidadDuracion, RangoDosis=@RangoDosis
                        WHERE Id=@Id", con))
                    {
                        cmd.Parameters.AddWithValue("@Nombre", model.Nombre);
                        cmd.Parameters.AddWithValue("@Descripcion", (object)model.Descripcion ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Estado", model.Estado);
                        cmd.Parameters.AddWithValue("@FechaActualizacion", DateTime.Now);
                        cmd.Parameters.AddWithValue("@Tipo", model.Tipo);
                        cmd.Parameters.AddWithValue("@Frecuencia", (object)model.Frecuencia ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@UnidadFrecuencia", (object)model.UnidadFrecuencia ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Duracion", (object)model.Duracion ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@UnidadDuracion", (object)model.UnidadDuracion ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@RangoDosis", (object)model.RangoDosis ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Id", model.Id);
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                    TempData["SwalMascotaEditada"] = "Vacuna/Pastilla editada correctamente";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    TempData["SwalError"] = "Error al editar: " + ex.Message;
                }
            }
            return View(model);
        }

        public ActionResult Details(int id)
        {
            Vacuna model = null;
            using (var con = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand("SELECT * FROM Vacunas WHERE Id=@Id", con))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                con.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        model = new Vacuna
                        {
                            Id = (int)dr["Id"],
                            Nombre = dr["Nombre"].ToString(),
                            Descripcion = dr["Descripcion"].ToString(),
                            Estado = dr["Estado"].ToString(),
                            FechaCreacion = (DateTime)dr["FechaCreacion"],
                            FechaActualizacion = dr["FechaActualizacion"] != DBNull.Value ? (DateTime?)dr["FechaActualizacion"] : null,
                            Tipo = dr["Tipo"].ToString(),
                            Frecuencia = dr["Frecuencia"] != DBNull.Value ? (int?)dr["Frecuencia"] : null,
                            UnidadFrecuencia = dr["UnidadFrecuencia"].ToString(),
                            Duracion = dr["Duracion"] != DBNull.Value ? (int?)dr["Duracion"] : null,
                            UnidadDuracion = dr["UnidadDuracion"].ToString(),
                            RangoDosis = dr["RangoDosis"].ToString()
                        };
                    }
                }
            }
            if (model == null)
                return HttpNotFound();
            return View(model);
        }

        // ACTIVAR/DESACTIVAR (en vez de delete)
        [HttpPost]
        public ActionResult CambiarEstado(int id, string nuevoEstado)
        {
            try
            {
                using (var con = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand("UPDATE Vacunas SET Estado=@Estado, FechaActualizacion=@FechaActualizacion WHERE Id=@Id", con))
                {
                    cmd.Parameters.AddWithValue("@Estado", nuevoEstado);
                    cmd.Parameters.AddWithValue("@FechaActualizacion", DateTime.Now);
                    cmd.Parameters.AddWithValue("@Id", id);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                TempData["SwalMascotaEditada"] = "Estado actualizado correctamente";
            }
            catch (Exception ex)
            {
                TempData["SwalError"] = "Error al actualizar estado: " + ex.Message;
            }
            return RedirectToAction("Index");
        }
    }
}
