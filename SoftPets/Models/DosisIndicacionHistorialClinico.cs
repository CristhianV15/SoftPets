using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftPets.Models
{
    public class DosisIndicacionHistorialClinico
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Indicación Historial Clínico")]
        public int IndicacionesHistorialesClinicosId { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Dosis")]
        public string Dosis { get; set; }

        [Required]
        [Display(Name = "Intervalo Cantidad")]
        public int IntervaloCantidad { get; set; }

        [Required]
        [StringLength(10)]
        [Display(Name = "Intervalo Tipo")]
        public string IntervaloTipo { get; set; }

        [Display(Name = "Cantidad Total de Dosis")]
        public int? CantidadTotalDosis { get; set; }

        [Required]
        [Display(Name = "Fecha de Inicio de Dosis")]
        public DateTime FechaInicioDosis { get; set; }

        [Display(Name = "Fecha de Fin de Dosis")]
        public DateTime? FechaFinDosis { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Estado de Alerta")]
        public string EstadoAlerta { get; set; }

        // Relaciones
        [ForeignKey("IndicacionesHistorialesClinicosId")]
        public virtual IndicacionHistorialClinico IndicacionHistorialClinico { get; set; }
    }
}