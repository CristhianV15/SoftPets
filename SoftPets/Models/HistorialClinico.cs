using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftPets.Models
{
    public class HistorialClinico
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Fecha de Inicio de Tratamiento")]
        public DateTime FechaInicioTratamiento { get; set; }

        [Display(Name = "Fecha de Fin de Tratamiento")]
        public DateTime? FechaFinTratamiento { get; set; }

        [Required]
        [Display(Name = "Fecha Posible de Fin de Tratamiento")]
        public DateTime FechaPosibleFinTratamiento { get; set; }

        [Required]
        [Display(Name = "Mascota")]
        public int MascotaId { get; set; }

      
        [NotMapped]
        [Display(Name = "Nombre del dueño")]
        public string NombreDuenio { get; set; }
        
        
        [Display(Name = "Nombre de mascota")]
        [NotMapped]
        public string NombreMascota { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Motivo")]
        public string Motivo { get; set; }

        [Required]
        [StringLength(15)]
        [Display(Name = "Tipo")]
        public string Tipo { get; set; }

        // Relaciones
        [ForeignKey("MascotaId")]
        public virtual Mascota Mascota { get; set; }
        

        //[Required]
        [Display(Name = "Dueño")]
        public int DuenioId { get; set; }

        [ForeignKey("DuenioId")]
        public virtual Duenio Duenio { get; set; }
        public virtual ICollection<DetalleHistorialClinico> DetallesHistorialClinico { get; set; }
    }
}