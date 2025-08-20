using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftPets.Models
{
    public class TomasDosisIndicacionHistorialClinicoVM
    {
        public int Id { get; set; }
        public int DosisIndicacionHistorialClinicoId { get; set; }
        public DateTime FechaHoraProgramada { get; set; }
        public string Estado { get; set; }
        public string Observaciones { get; set; }
        public string NombreMascota { get; set; }
        public string Medicamento { get; set; }
        public string Motivo { get; set; }
    }
}