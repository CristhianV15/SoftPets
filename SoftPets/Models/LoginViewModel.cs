using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SoftPets.Models
{
    public class LoginViewModel
    {
        [Required]
        public string Nombre { get; set; }


        [Display(Name = "Contraseña")]
        [Required]
        [DataType(DataType.Password)]
        public string Contrasenia { get; set; }
    }
}