using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SoftPets.Models
{
    public class QRPublico
    {
        [Key, Column(Order = 0)]
        public int MascotaId { get; set; }

        [Key, Column(Order = 1)]
        [StringLength(100)]
        public string Campo { get; set; }

        [Required]
        public bool Visible { get; set; }

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