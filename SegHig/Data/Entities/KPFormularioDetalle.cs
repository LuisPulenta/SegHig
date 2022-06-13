using System.ComponentModel.DataAnnotations;

namespace SegHig.Data.Entities
{
    public class KPFormularioDetalle
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "N°")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int Orden { get; set; }
        [Display(Name = "Descripción")]
        [MaxLength(300, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public String Description { get; set; }
        [Display(Name = "Ponderación")]
        public int Ponderacion { get; set; }
        [Display(Name = "Formulario")]
        public KPFormulario KPFormulario { get; set; }
        [Display(Name = "Activo")]
        public bool Active { get; set; }

    }
}