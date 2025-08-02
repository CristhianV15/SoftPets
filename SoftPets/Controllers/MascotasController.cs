using SoftPets.Filters;
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
    [RequiereDatosCompletos]

    public class MascotasController : Controller
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["ConexionLocal"].ConnectionString;

        // Listar mascotas del dueño logueado
        public ActionResult Index(string filtroNombre = null, string filtroSexo = null, string filtroRaza = null, int? filtroDuenio = null)
        {
            int? rolId = Session["RolId"] as int?;
            int? duenioId = Session["DuenioId"] as int?;

            var lista = new List<Mascota>();
            var dueños = new List<Duenio>();

            using (var con = new SqlConnection(connectionString))
            {
                SqlCommand cmd;
                if (rolId == 1 || rolId == 3) // Admin o Veterinario
                {
                    string sql = @"SELECT m.*, d.Nombres, d.Apellidos FROM Mascotas m 
                           LEFT JOIN Duenios d ON m.DuenioId = d.Id
                           WHERE m.Estado='1'";
                    if (!string.IsNullOrEmpty(filtroNombre))
                        sql += " AND m.Nombre LIKE @filtroNombre";
                    if (!string.IsNullOrEmpty(filtroSexo))
                        sql += " AND m.Sexo = @filtroSexo";
                    if (!string.IsNullOrEmpty(filtroRaza))
                        sql += " AND m.Raza = @filtroRaza";
                    if (filtroDuenio.HasValue && filtroDuenio.Value > 0)
                        sql += " AND m.DuenioId = @filtroDuenio";
                    cmd = new SqlCommand(sql, con);
                    if (!string.IsNullOrEmpty(filtroNombre))
                        cmd.Parameters.AddWithValue("@filtroNombre", "%" + filtroNombre + "%");
                    if (!string.IsNullOrEmpty(filtroSexo))
                        cmd.Parameters.AddWithValue("@filtroSexo", filtroSexo);
                    if (!string.IsNullOrEmpty(filtroRaza))
                        cmd.Parameters.AddWithValue("@filtroRaza", filtroRaza);
                    if (filtroDuenio.HasValue && filtroDuenio.Value > 0)
                        cmd.Parameters.AddWithValue("@filtroDuenio", filtroDuenio.Value);
                }
                else if (rolId == 2 && duenioId != null) // Dueño
                {
                    cmd = new SqlCommand("SELECT m.*, d.Nombres, d.Apellidos FROM Mascotas m LEFT JOIN Duenios d ON m.DuenioId = d.Id WHERE m.DuenioId=@duenioId AND m.Estado='1'", con);
                    cmd.Parameters.AddWithValue("@duenioId", duenioId.Value);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }

                con.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new Mascota
                        {
                            Id = (int)dr["Id"],
                            DuenioId = (int)dr["DuenioId"],
                            Nombre = dr["Nombre"].ToString(),
                            Especie = dr["Especie"].ToString(),
                            Raza = dr["Raza"].ToString(),
                            FechaNacimiento = dr["FechaNacimiento"] != DBNull.Value ? (DateTime?)dr["FechaNacimiento"] : null,
                            Sexo = dr["Sexo"].ToString(),
                            Color = dr["Color"].ToString(),
                            Renian = dr["Renian"].ToString(),
                            Foto = dr["Foto"].ToString(),
                            Estado = dr["Estado"].ToString()[0],
                            FechaCreacion = Convert.ToDateTime(dr["FechaCreacion"]),
                            DueñoNombre = dr["Nombres"]?.ToString(),
                            DueñoApellido = dr["Apellidos"]?.ToString()
                        });
                    }
                }
            }

            // Para los combos de filtro
            ViewBag.Razas = lista.Select(m => m.Raza).Distinct().OrderBy(x => x).ToList();
            ViewBag.Sexos = lista.Select(m => m.Sexo).Distinct().OrderBy(x => x).ToList();

            // Para filtro de dueños (solo admin/veterinario)
            if (rolId == 1 || rolId == 3)
            {
                using (var con = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand("SELECT Id, Nombres, Apellidos FROM Duenios", con))
                {
                    con.Open();
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            dueños.Add(new Duenio
                            {
                                Id = (int)dr["Id"],
                                Nombres = dr["Nombres"].ToString(),
                                Apellidos = dr["Apellidos"].ToString()
                            });
                        }
                    }
                }
            }
            ViewBag.Duenios = dueños;

            return View(lista);
        }

        // Detalles
        public ActionResult Details(int id)
        {
            Mascota mascota = null;
            using (var con = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand("SELECT * FROM Mascotas WHERE Id=@id", con))
            {
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        mascota = new Mascota
                        {
                            Id = (int)dr["Id"],
                            DuenioId = (int)dr["DuenioId"],
                            Nombre = dr["Nombre"].ToString(),
                            Especie = dr["Especie"].ToString(),
                            Raza = dr["Raza"].ToString(),
                            FechaNacimiento = dr["FechaNacimiento"] != DBNull.Value ? (DateTime?)dr["FechaNacimiento"] : null,
                            Sexo = dr["Sexo"].ToString(),
                            Color = dr["Color"].ToString(),
                            Renian = dr["Renian"].ToString(),
                            Foto = dr["Foto"].ToString(),
                            Estado = dr["Estado"].ToString()[0],
                            FechaCreacion = Convert.ToDateTime(dr["FechaCreacion"])
                        };
                    }
                }
            }
            if (mascota == null) return HttpNotFound();
            return View(mascota);
        }

        // Crear
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Mascota model, HttpPostedFileBase FotoFile)
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrWhiteSpace(model.Renian))
                {
                    using (var con = new SqlConnection(connectionString))
                    using (var cmd = new SqlCommand("SELECT COUNT(*) FROM Mascotas WHERE Renian = @Renian", con))
                    {
                        cmd.Parameters.AddWithValue("@Renian", model.Renian);
                        con.Open();
                        int count = (int)cmd.ExecuteScalar();
                        if (count > 0)
                        {
                            ModelState.AddModelError("Renian", "El Renian ya está registrado para otra mascota.");
                            return View(model);
                        }
                    }
                }
                int duenioId = (int)Session["DuenioId"];
                string rutaFoto = null;
                if (FotoFile != null && FotoFile.ContentLength > 0)
                {
                    var fileName = Guid.NewGuid() + System.IO.Path.GetExtension(FotoFile.FileName);
                    var path = Server.MapPath("~/FotosMascotas/" + fileName);
                    FotoFile.SaveAs(path);
                    rutaFoto = "/FotosMascotas/" + fileName;
                }

                using (var con = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand("INSERT INTO Mascotas (DuenioId, Nombre, Especie, Raza, FechaNacimiento, Sexo, Color, Renian, Foto, Estado, FechaCreacion) VALUES (@DuenioId, @Nombre, @Especie, @Raza, @FechaNacimiento, @Sexo, @Color, @Renian, @Foto, @Estado, GETDATE())", con))
                {
                    cmd.Parameters.AddWithValue("@DuenioId", duenioId);
                    cmd.Parameters.AddWithValue("@Nombre", model.Nombre);
                    cmd.Parameters.AddWithValue("@Especie", model.Especie ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Raza", model.Raza ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@FechaNacimiento", model.FechaNacimiento ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Sexo", model.Sexo ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Color", model.Color ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Renian", model.Renian ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Foto", rutaFoto ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Estado", '1');
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                return RedirectToAction("Index");
            }
            return View(model);
        }



        // Editar
        public ActionResult Edit(int id)
        {
            Mascota mascota = null;
            using (var con = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand("SELECT * FROM Mascotas WHERE Id=@id", con))
            {
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        mascota = new Mascota
                        {
                            Id = (int)dr["Id"],
                            DuenioId = (int)dr["DuenioId"],
                            Nombre = dr["Nombre"].ToString(),
                            Especie = dr["Especie"].ToString(),
                            Raza = dr["Raza"].ToString(),
                            FechaNacimiento = dr["FechaNacimiento"] != DBNull.Value ? (DateTime?)dr["FechaNacimiento"] : null,
                            Sexo = dr["Sexo"].ToString(),
                            Color = dr["Color"].ToString(),
                            Renian = dr["Renian"].ToString(),
                            Foto = dr["Foto"].ToString(),
                            Estado = dr["Estado"].ToString()[0],
                            FechaCreacion = Convert.ToDateTime(dr["FechaCreacion"])
                        };
                    }
                }
            }
            if (mascota == null) return HttpNotFound();
            return View(mascota);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Mascota model)
        {
            if (ModelState.IsValid)
            {
                using (var con = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand("UPDATE Mascotas SET Nombre=@Nombre, Especie=@Especie, Raza=@Raza, FechaNacimiento=@FechaNacimiento, Sexo=@Sexo, Color=@Color, Renian=@Renian, Foto=@Foto WHERE Id=@Id", con))
                {
                    cmd.Parameters.AddWithValue("@Id", model.Id);
                    cmd.Parameters.AddWithValue("@Nombre", model.Nombre);
                    cmd.Parameters.AddWithValue("@Especie", model.Especie ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Raza", model.Raza ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@FechaNacimiento", model.FechaNacimiento ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Sexo", model.Sexo ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Color", model.Color ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Renian", model.Renian ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Foto", model.Foto ?? (object)DBNull.Value);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                return RedirectToAction("Index");
            }
            return View(model);
        }
    }
}