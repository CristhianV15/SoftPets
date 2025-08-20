using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SoftPets.Models
{
    public class TomasDosisIndicacionHistorialClinico
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Dosis Receta")]
        public int DosisIndicacionHistorialClinicoId { get; set; }

        [Required]
        [Display(Name = "Fecha y Hora Programada")]
        public DateTime FechaHoraProgramada { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Estado")]
        public string Estado { get; set; }

        [StringLength(200)]
        [Display(Name = "Observaciones")]
        public string Observaciones { get; set; }

        [ForeignKey("DosisIndicacionHistorialClinicoId")]
        public virtual DosisIndicacionHistorialClinico DosisIndicacionHistorialClinico { get; set; }
    }
}