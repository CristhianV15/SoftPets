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

        public DateTime? Fecha { get; set; }

        [Required]
        public int VeterinarioId { get; set; } // FK a Usuario.Id (veterinario)

        [StringLength(255)]
        public string Observaciones { get; set; }

        [Required]
        public char Estado { get; set; }

        [Required]
        public DateTime FechaCreacion { get; set; }

        public DateTime? FechaActualizacion { get; set; }

        // Relaciones
        [ForeignKey("MascotaId")]
        public virtual Mascota Mascota { get; set; }

        [ForeignKey("VacunaId")]
        public virtual Vacuna Vacuna { get; set; }

        [ForeignKey("VeterinarioId")]
        public virtual Usuario Veterinario { get; set; }
    }
}