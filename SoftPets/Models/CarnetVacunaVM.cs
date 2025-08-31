using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftPets.Models
{
    public class CarnetVacunaVM
    {
        public string NombreVacuna { get; set; }
        public string Tipo { get; set; }
        public string DosisAplicada { get; set; }
        public DateTime? FechaAplicada { get; set; }
        public string Lote { get; set; }
        public string Observaciones { get; set; }
    }
}