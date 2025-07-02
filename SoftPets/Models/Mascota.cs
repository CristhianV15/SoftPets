using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SoftPets.Models
{
    public class Mascota
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int DuenioId { get; set; }

        [StringLength(100)]
        public string Nombre { get; set; }

        [StringLength(30)]
        public string Especie { get; set; }

        [StringLength(100)]
        public string Raza { get; set; }

        public DateTime? FechaNacimiento { get; set; }

        [StringLength(10)]
        public string Sexo { get; set; }

        [StringLength(30)]
        public string Color { get; set; }

        [StringLength(50)]
        public string Renian { get; set; }

        [StringLength(255)]
        public string QR { get; set; }

        [StringLength(255)]
        public string PublicViewSettings { get; set; }

        [StringLength(75)]
        public string Foto { get; set; }

        [Required]
        public char Estado { get; set; }

        [Required]
        public DateTime FechaCreacion { get; set; }

        public DateTime? FechaActualizacion { get; set; }

        // Relaciones
        [ForeignKey("DuenioId")]
        public virtual Duenio Duenio { get; set; }

        public virtual ICollection<Vacunacion> Vacunaciones { get; set; }
        public virtual ICollection<HistorialClinico> HistorialesClinicos { get; set; }
        public virtual ICollection<Tendencia> Tendencias { get; set; }
        public virtual ICollection<QRPublico> QRPublicos { get; set; }
    }   






}