using System.ComponentModel.DataAnnotations;
namespace PROYECTOMOVIE.Models;
public class PerfilVistaUsuario
{
    // Información personal
    public string? Id { get; set; }

    [Required(ErrorMessage = "El nombre es requerido")]
    [Display(Name = "Nombre")]
    public string? Nombre { get; set; }

    [Required(ErrorMessage = "El apellido es requerido")]

    public string? Apellido { get; set; }

    [Required(ErrorMessage = "El correo electrónico es requerido")]
    [EmailAddress(ErrorMessage = "El formato del correo no es válido")]
    [Display(Name = "Correo Electrónico")]
    public string? Email { get; set; }

    [Display(Name = "Nombre de Usuario")]
    public string? NombredeUsuario { get; set; }


    // Cambio de contraseña
    [Display(Name = "Contraseña Actual")]
    [DataType(DataType.Password)]
    public string? CurrentPassword { get; set; }

    [Display(Name = "Nueva Contraseña")]
    [DataType(DataType.Password)]
    [StringLength(100, ErrorMessage = "La {0} debe tener al menos {2} caracteres.", MinimumLength = 6)]
    public string? NewPassword { get; set; }

    [Display(Name = "Confirmar Nueva Contraseña")]
    [DataType(DataType.Password)]
    [Compare("NewPassword", ErrorMessage = "Las contraseñas no coinciden.")]
    public string? ConfirmPassword { get; set; }

    // Información de auditoría
    [Display(Name = "Fecha de Registro")]
    public DateTime FechaRegistro { get; set; }

    [Display(Name = "Último Acceso")]
    public DateTime? LastLogin { get; set; }

    public string? SuccessMessage { get; set; }
    public string? ErrorMessage { get; set; }
}
