using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SoftPets.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede tener más de 100 caracteres.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El email es obligatorio.")]
        [StringLength(100, ErrorMessage = "El email no puede tener más de 100 caracteres.")]
        [EmailAddress(ErrorMessage = "Ingrese un email válido.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [StringLength(255, ErrorMessage = "La contraseña no puede tener más de 255 caracteres.")]
        public string Contrasenia { get; set; }

        [Required]
        [Display(Name = "Rol")]
        public int RolId { get; set; }

        [Required]
        public char Estado { get; set; } // '1' = Activo, '0' = Inactivo

        [Required]
        [Display(Name = "Fecha de Creación")]
        public DateTime FechaCreacion { get; set; }

        [Display(Name = "Fecha de Actualización")]
        public DateTime? FechaActualizacion { get; set; }

        // Relaciones
        [ForeignKey("RolId")]
        public virtual Rol Rol { get; set; }

        // 1:1 con Duenios (si es cliente/dueño)
        public virtual Duenio Duenio { get; set; }

        // 1:1 con Veterinarios (si es veterinario)
        public virtual Veterinario Veterinario { get; set; }

        // Para auditoría: Un usuario veterinario puede haber realizado muchas vacunaciones o historiales
        public virtual ICollection<Vacunacion> VacunacionesRealizadas { get; set; }
        public virtual ICollection<HistorialClinico> HistorialesClinicosRealizados { get; set; }
    }
}