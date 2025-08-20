using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftPets.Models
{
    public class ProximaConsultaVM
    {
        public int Id { get; set; }
        public int HistorialClinicoId { get; set; }
        public DateTime FechaFuturaConsulta { get; set; }
        public string Estado { get; set; }
        public string Tipo { get; set; }
        public string Motivo { get; set; }
        public string NombreMascota { get; set; }
    }
}