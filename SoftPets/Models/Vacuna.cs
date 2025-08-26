using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SoftPets.Models
{
    public class Vacuna
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Nombre { get; set; }

        [StringLength(255)]
        public string Descripcion { get; set; }

        [StringLength(50)]
        public string Lote { get; set; }

        [Required, StringLength(20)]
        public string Estado { get; set; } // 'Activo', 'Inactivo'

        [Required]
        public DateTime FechaCreacion { get; set; }

        public DateTime? FechaActualizacion { get; set; }

        [Required, StringLength(20)]
        public string Tipo { get; set; } // 'Vacuna' o 'Pastilla'

        public int? Frecuencia { get; set; }
        [StringLength(10)]
        public string UnidadFrecuencia { get; set; }
        public int? Duracion { get; set; }
        [StringLength(10)]
        public string UnidadDuracion { get; set; }
        [StringLength(50)]
        public string RangoDosis { get; set; }
    }
}
