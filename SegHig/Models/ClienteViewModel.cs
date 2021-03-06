using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace SegHig.Models
{
    public class ClienteViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Nombre")]
        [MaxLength(50, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public String Name { get; set; }

        [Display(Name = "Dirección")]
        [MaxLength(200, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string Address { get; set; }

        [Display(Name = "Teléfono")]
        [MaxLength(15, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string Phone { get; set; }

        [Display(Name = "Activo")]
        public bool Active { get; set; }
        [Display(Name = "Tipo de Cliente")]

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un Tipo de Cliente.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public int ClienteTipoId { get; set; }

        public IEnumerable<SelectListItem> ClienteTipos { get; set; }
    }
}
