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
    public class VacunasController : Controller
    {
            private string connectionString = ConfigurationManager.ConnectionStrings["ConexionLocal"].ConnectionString;

            // GET: Vacunas
            public ActionResult Index()
            {
                List<Vacuna> vacunas = new List<Vacuna>();

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("VacunasSelect", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Id", DBNull.Value);

                        con.Open();
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            vacunas.Add(new Vacuna
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Nombre = reader["Nombre"].ToString(),
                                Descripcion = reader["Descripcion"].ToString(),
                                Lote = reader["Lote"].ToString(),
                                Estado = Convert.ToChar(reader["Estado"]),
                                FechaCreacion = Convert.ToDateTime(reader["FechaCreacion"]),
                                FechaActualizacion = reader["FechaActualizacion"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["FechaActualizacion"])
                            });
                        }
                    }
                }
                return View(vacunas);
            }

            // GET: Vacunas/Details/5
            public ActionResult Details(int id)
            {
                Vacuna vacuna = null;

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("VacunasSelect", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Id", id);

                        con.Open();
                        SqlDataReader reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            vacuna = new Vacuna
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Nombre = reader["Nombre"].ToString(),
                                Descripcion = reader["Descripcion"].ToString(),
                                Lote = reader["Lote"].ToString(),
                                Estado = Convert.ToChar(reader["Estado"]),
                                FechaCreacion = Convert.ToDateTime(reader["FechaCreacion"]),
                                FechaActualizacion = reader["FechaActualizacion"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["FechaActualizacion"])
                            };
                        }
                    }
                }

                if (vacuna == null)
                {
                    return HttpNotFound();
                }

                return View(vacuna);
            }

            // GET: Vacunas/Create
            public ActionResult Create()
            {
                return View();
            }

            // POST: Vacunas/Create
            [HttpPost]
            [ValidateAntiForgeryToken]
            public ActionResult Create(Vacuna model)
            {
                if (ModelState.IsValid)
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand("VacunasInsert", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@Nombre", model.Nombre);
                            cmd.Parameters.AddWithValue("@Descripcion", (object)model.Descripcion ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@Lote", (object)model.Lote ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@Estado", model.Estado);

                            con.Open();
                            cmd.ExecuteNonQuery();
                        }
                    }
                    return RedirectToAction("Index");
                }
                return View(model);
            }

            // GET: Vacunas/Edit/5
            public ActionResult Edit(int id)
            {
                Vacuna vacuna = null;

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("VacunasSelect", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Id", id);

                        con.Open();
                        SqlDataReader reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            vacuna = new Vacuna
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Nombre = reader["Nombre"].ToString(),
                                Descripcion = reader["Descripcion"].ToString(),
                                Lote = reader["Lote"].ToString(),
                                Estado = Convert.ToChar(reader["Estado"]),
                                FechaCreacion = Convert.ToDateTime(reader["FechaCreacion"]),
                                FechaActualizacion = reader["FechaActualizacion"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["FechaActualizacion"])
                            };
                        }
                    }
                }

                if (vacuna == null)
                {
                    return HttpNotFound();
                }

                return View(vacuna);
            }

            // POST: Vacunas/Edit/5
            [HttpPost]
            [ValidateAntiForgeryToken]
            public ActionResult Edit(Vacuna model)
            {
                if (ModelState.IsValid)
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand("VacunasUpdate", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@Id", model.Id);
                            cmd.Parameters.AddWithValue("@Nombre", model.Nombre);
                            cmd.Parameters.AddWithValue("@Descripcion", (object)model.Descripcion ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@Lote", (object)model.Lote ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@Estado", model.Estado);

                            con.Open();
                            cmd.ExecuteNonQuery();
                        }
                    }
                    return RedirectToAction("Index");
                }
                return View(model);
            }

            // GET: Vacunas/Delete/5
            public ActionResult Delete(int id)
            {
                Vacuna vacuna = null;

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("VacunasSelect", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Id", id);

                        con.Open();
                        SqlDataReader reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            vacuna = new Vacuna
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Nombre = reader["Nombre"].ToString(),
                                Descripcion = reader["Descripcion"].ToString(),
                                Lote = reader["Lote"].ToString(),
                                Estado = Convert.ToChar(reader["Estado"]),
                                FechaCreacion = Convert.ToDateTime(reader["FechaCreacion"]),
                                FechaActualizacion = reader["FechaActualizacion"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["FechaActualizacion"])
                            };
                        }
                    }
                }

                if (vacuna == null)
                {
                    return HttpNotFound();
                }

                return View(vacuna);
            }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            char estadoActual = '1';
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("VacunasSelect", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", id);

                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        estadoActual = Convert.ToChar(reader["Estado"]);
                    }
                }
            }

            char nuevoEstado = estadoActual == '1' ? '0' : '1';

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("VacunasDeactivate", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@NuevoEstado", nuevoEstado);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            return RedirectToAction("Index");
        }

    }
}
