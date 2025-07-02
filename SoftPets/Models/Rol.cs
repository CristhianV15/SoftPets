using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SoftPets.Models
{
    public class Rol
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del rol es obligatorio.")]
        [StringLength(50, ErrorMessage = "El rol no puede tener más de 50 caracteres.")]
        public string Nombre { get; set; }

        public char Estado { get; set; } //Por defecto es 1 al crearlo 

        // Relaciones
        //public virtual ICollection<Usuario> Usuarios { get; set; }
    }
}