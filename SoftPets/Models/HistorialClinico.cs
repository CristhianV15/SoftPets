using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SoftPets.Models
{
    public class HistorialClinico
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int MascotaId { get; set; }

        public DateTime? FechaConsulta { get; set; }

        [Required]
        public int VeterinarioId { get; set; } // FK a Usuario.Id (veterinario)

        [StringLength(255)]
        public string Diagnostico { get; set; }

        [StringLength(255)]
        public string Tratamiento { get; set; }

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

        [ForeignKey("VeterinarioId")]
        public virtual Usuario Veterinario { get; set; }
    }
}