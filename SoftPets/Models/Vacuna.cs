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

        [Required(ErrorMessage = "El nombre de la vacuna es obligatorio.")]
        [StringLength(100)]
        public string Nombre { get; set; }

        [Display(Name = "Descripción")]
        [StringLength(255)]
        public string Descripcion { get; set; }

        [StringLength(50)]
        public string Lote { get; set; }

        [Required]
        public char Estado { get; set; } // '1' = Activo, '0' = Inactivo

        [Display(Name = "F. Creación")]
        [Required]
        public DateTime FechaCreacion { get; set; }

        [Display(Name = "F. Actualización")]
        public DateTime? FechaActualizacion { get; set; }

        // Relaciones
        public virtual ICollection<Vacunacion> Vacunaciones { get; set; }
    }
}
