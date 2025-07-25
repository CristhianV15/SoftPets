using SoftPets.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace SoftPets.Controllers
{
    public class MascotasController : Controller
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["ConexionLocal"].ConnectionString;

        // Listar mascotas del dueño logueado
        public ActionResult Index()
        {
            // Solo dueños pueden ver sus mascotas
            int? rolId = Session["RolId"] as int?;
            int? duenioId = Session["DuenioId"] as int?;

            if (rolId != 2 || duenioId == null)
            {
                // Redirige o muestra error si no es dueño
                return RedirectToAction("Index", "Home");
            }

            var lista = new List<Mascota>();
            using (var con = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand("SELECT * FROM Mascotas WHERE DuenioId=@duenioId AND Estado='1'", con))
            {
                cmd.Parameters.AddWithValue("@duenioId", duenioId.Value);
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
                            FechaCreacion = Convert.ToDateTime(dr["FechaCreacion"])
                        });
                    }
                }
            }
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
        public ActionResult Create(Mascota model)
        {
            if (ModelState.IsValid)
            {
                int duenioId = (int)Session["DuenioId"];
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
                    cmd.Parameters.AddWithValue("@Foto", model.Foto ?? (object)DBNull.Value);
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