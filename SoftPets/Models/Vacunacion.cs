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

            [StringLength(50)]
            public string DosisAplicada { get; set; }

            public DateTime? FechaAplicada { get; set; }
            public DateTime? FechaProgramada { get; set; }

            [StringLength(50)]
            public string Lote { get; set; }

            [StringLength(255)]
            public string Observaciones { get; set; }

            [Required, StringLength(20)]
            public string Estado { get; set; } // 'Pendiente', 'Aplicada'

            [Required]
            public DateTime FechaCreacion { get; set; }

            public DateTime? FechaActualizacion { get; set; }
        }
    }
