using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SoftPets.Models
{
    public class Tendencia
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int MascotaId { get; set; }

        public DateTime? Fecha { get; set; }

        public decimal? Peso { get; set; }

        public decimal? Temperatura { get; set; }

        [StringLength(255)]
        public string Otros { get; set; }

        [Required]
        public char Estado { get; set; }

        [Required]
        public DateTime FechaCreacion { get; set; }

        public DateTime? FechaActualizacion { get; set; }

        // Relaciones
        [ForeignKey("MascotaId")]
        public virtual Mascota Mascota { get; set; }
    }
}