using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SoftPets.Models
{
    public class Vacunacion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int MascotaId { get; set; }

        [Required]
        public int VacunaId { get; set; }

        [StringLength(50)]
        [Display(Name = "Dosis Aplicada")]
        public string DosisAplicada { get; set; }

        [Display(Name = "Fecha Aplicada")]
        public DateTime? FechaAplicada { get; set; }

        [Display(Name = "Fecha Programada")]
        public DateTime? FechaProgramada { get; set; }

        [StringLength(50)]
        [Display(Name = "Lote")]
        public string Lote { get; set; }

        [StringLength(255)]
        [Display(Name = "Observaciones")]
        public string Observaciones { get; set; }

        [Required, StringLength(20)]
        [Display(Name = "Estado")]
        public string Estado { get; set; } // 'Pendiente', 'Aplicada'

        [Required]
        [Display(Name = "Fecha de Creación")]
        public DateTime FechaCreacion { get; set; }

        [Display(Name = "Fecha de Actualización")]
        public DateTime? FechaActualizacion { get; set; }
    }
}
