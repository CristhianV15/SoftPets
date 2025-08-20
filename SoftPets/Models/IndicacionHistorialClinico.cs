using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftPets.Models
{
    public class IndicacionHistorialClinico
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Detalle Historial Clínico")]
        public int DetalleHistorialClinicoId { get; set; }

        
        [StringLength(100)]
        [Display(Name = "Medicamento")]
        public string Medicamento { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Indicación")]
        public string Indicacion { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Estado")]
        public string Estado { get; set; }

        // Relaciones
        [ForeignKey("DetalleHistorialClinicoId")]
        public virtual DetalleHistorialClinico DetalleHistorialClinico { get; set; }

        public virtual ICollection<DosisIndicacionHistorialClinico> DosisIndicacionesHistorialClinico { get; set; }
    }
}