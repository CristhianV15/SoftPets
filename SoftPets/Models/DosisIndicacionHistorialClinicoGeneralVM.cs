using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftPets.Models
{
    public class DosisIndicacionHistorialClinicoGeneralVM
    {

        public int Id { get; set; }
        public string Dosis { get; set; }
        public int IntervaloCantidad { get; set; }
        public string IntervaloTipo { get; set; }
        public int? CantidadTotalDosis { get; set; }
        public DateTime FechaInicioDosis { get; set; }
        public DateTime? FechaFinDosis { get; set; }
        public string EstadoAlerta { get; set; }
        public string Medicamento { get; set; }
        public string Indicacion { get; set; }
        public string NombreMascota { get; set; }
        public string Motivo { get; set; }

        public int IndicacionesHistorialesClinicosId { get; set; }
        public int DetallesHistorialesClinicosId { get; set; }
    }
}