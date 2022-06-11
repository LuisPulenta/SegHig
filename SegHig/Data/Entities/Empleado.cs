using System.ComponentModel.DataAnnotations;

namespace SegHig.Data.Entities
{
    public class Empleado
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Documento")]
        [MaxLength(20, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string Document { get; set; }

        [Display(Name = "Nombres")]
        [MaxLength(50, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string FirstName { get; set; }

        [Display(Name = "Apellidos")]
        [MaxLength(50, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string LastName { get; set; }

        [Display(Name = "Cliente")]
        public Cliente Cliente { get; set; }
        public bool Active { get; set; }

        [Display(Name = "Empleado")]
        public string FullName => $"{FirstName} {LastName}";

        [Display(Name = "Empleado")]
        public string FullNameWithClienteYDocument => $"{Cliente} - {FirstName} {LastName} - {Document}";
    }
}
