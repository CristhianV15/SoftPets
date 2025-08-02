using System.Web.Mvc;
using System.Data.SqlClient;
using System.Configuration;

namespace SoftPets.Filters
{
    public class RequiereDatosCompletosAttribute : ActionFilterAttribute
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["ConexionLocal"].ConnectionString;

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var session = filterContext.HttpContext.Session;
            int? rolId = session["RolId"] as int?;
            int? usuarioId = session["UsuarioId"] as int?;
            int? duenioId = session["DuenioId"] as int?;

            // Solo aplica para veterinario o dueño
            if (rolId == 2 && usuarioId != null)
            {
                // Dueño: validar datos completos
                bool tieneDatos = false;
                using (var con = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand("SELECT COUNT(*) FROM Duenios WHERE UsuarioId=@usuarioId AND Nombres IS NOT NULL AND Nombres <> ''", con))
                {
                    cmd.Parameters.AddWithValue("@usuarioId", usuarioId);
                    con.Open();
                    int count = (int)cmd.ExecuteScalar();
                    tieneDatos = count > 0;
                }
                if (!tieneDatos && filterContext.ActionDescriptor.ActionName != "CompletarDatos")
                {
                    filterContext.Result = new RedirectToRouteResult(
                        new System.Web.Routing.RouteValueDictionary(
                            new { controller = "Duenios", action = "CompletarDatos" }
                        )
                    );
                    return;
                }

                // Validar que tenga al menos una mascota
                bool tieneMascota = false;
                using (var con = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand("SELECT COUNT(*) FROM Mascotas WHERE DuenioId=@duenioId AND Estado='1'", con))
                {
                    cmd.Parameters.AddWithValue("@duenioId", usuarioId);
                    con.Open();
                    int count = (int)cmd.ExecuteScalar();
                    tieneMascota = count > 0;
                }
                // Solo restringe acceso a otros controladores si no está en MascotasController o en CompletarDatos
                var controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
                var actionName = filterContext.ActionDescriptor.ActionName;
                if (!tieneMascota && controllerName != "Mascotas" && actionName != "CompletarDatos")
                {
                    filterContext.Result = new RedirectToRouteResult(
                        new System.Web.Routing.RouteValueDictionary(
                            new { controller = "Mascotas", action = "Index" }
                        )
                    );
                    return;
                }
            }
            else if (rolId == 3 && usuarioId != null)
            {
                // Veterinario: validar datos completos
                bool tieneDatos = false;
                using (var con = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand("SELECT COUNT(*) FROM Veterinarios WHERE UsuarioId=@usuarioId AND Nombre IS NOT NULL AND Nombre <> ''", con))
                {
                    cmd.Parameters.AddWithValue("@usuarioId", usuarioId);
                    con.Open();
                    int count = (int)cmd.ExecuteScalar();
                    tieneDatos = count > 0;
                }
                if (!tieneDatos && filterContext.ActionDescriptor.ActionName != "CompletarDatos")
                {
                    filterContext.Result = new RedirectToRouteResult(
                        new System.Web.Routing.RouteValueDictionary(
                            new { controller = "Veterinarios", action = "CompletarDatos" }
                        )
                    );
                    return;
                }
            }
            base.OnActionExecuting(filterContext);
        }
    }
}