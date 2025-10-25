using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace PROYECTOMOVIE.Models
{
    
    public class Usuario : IdentityUser
    {

    
        
        //public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(30, ErrorMessage = "El nombre no puede exceder 50 caracteres")]
        public string? Nombre { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio")]
        [StringLength(50, ErrorMessage = "El apellido no puede exceder 50 caracteres")]
        public string? Apellido { get; set; }

        //[Required(ErrorMessage = "El email es obligatorio")]
        //[EmailAddress(ErrorMessage = "El formato del email no es válido")]
        //public string? Correo { get; set; }

        //[Required(ErrorMessage = "La contraseña es obligatoria")]
        //[StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        //[DataType(DataType.Password)]
        //public string? Contraseña { get; set; }


        //[Required(ErrorMessage = "La confirmación de contraseña es obligatoria")]
        //[Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
        //[DataType(DataType.Password)]
        //[Display(Name = "Confirmar Contraseña")]
        //public string? ConfirmarPassword { get; set; }

        public DateTime FechaRegistro { get; set; } = DateTime.Now;

                
        // Suscripción (relación 1:1)
        public virtual UsuarioSuscripcion? Subcripcion { get; set; }
        
        // Método de pago guardado
        public bool HasPaymentMethod { get; set; } = false;
        public virtual ICollection<UsuarioPelicula>? UsuarioPeliculas { get; set; }

        // Propiedad de navegación para suscripciones (1:N por si quieres historial)
        public virtual ICollection<UsuarioSuscripcion>? Suscripciones { get; set; }
    }
}