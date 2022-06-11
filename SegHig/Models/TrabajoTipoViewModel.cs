using System.ComponentModel.DataAnnotations;

namespace SegHig.Models
{
    public class TrabajoTipoViewModel
    {
        public int Id { get; set; }

        [Display(Name = "N°")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int Orden { get; set; }
        

        [Display(Name = "Nombre")]
        [MaxLength(50, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public String Name { get; set; }

        [Display(Name = "Activo")]
        public bool Active { get; set; }
        [Display(Name = "Tipo de Cliente")]

        public int ClienteId { get; set; }
    }
}
