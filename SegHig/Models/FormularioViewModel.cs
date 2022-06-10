using System.ComponentModel.DataAnnotations;

namespace SegHig.Models
{
    public class FormularioViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Nombre")]
        [MaxLength(50, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public String Name { get; set; }

        [Display(Name = "Activo")]
        public bool Active { get; set; }
        [Display(Name = "Tipo de Trabajo")]

        public int TrabajoTipoId { get; set; }
    }
}
