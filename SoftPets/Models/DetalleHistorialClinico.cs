using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftPets.Models
{
    public class DetalleHistorialClinico
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Historial Clínico")]
        public int HistorialClinicoId { get; set; }

        [Required]
        [Display(Name = "Fecha de Consulta")]
        public DateTime FechaConsulta { get; set; }

        [Required]
        [StringLength(450)]
        [Display(Name = "Observaciones")]
        public string Recomendaciones { get; set; }

        [Display(Name = "Próxima fecha de Consulta")]
        public DateTime? FechaFuturaConsulta { get; set; }

        [Display(Name = "Estado")]
        [StringLength(20)]
        public string Estado { get; set; }


        // Relaciones
        [ForeignKey("HistorialClinicoId")]
        public virtual HistorialClinico HistorialClinico { get; set; }

        public virtual ICollection<IndicacionHistorialClinico> IndicacionesHistorialClinico { get; set; }
    }
}