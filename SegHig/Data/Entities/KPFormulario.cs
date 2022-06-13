using System.ComponentModel.DataAnnotations;

namespace SegHig.Data.Entities
{
    public class KPFormulario
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "N°")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int Orden { get; set; }

        [Display(Name = "Nombre")]
        [MaxLength(50, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public String Name { get; set; }
        [Display(Name = "Tipo de Trabajo")]
        public KPTrabajoTipo KPTrabajoTipo { get; set; }
        [Display(Name = "Activo")]
        public bool Active { get; set; }
        [Display(Name = "Detalles")]
        public ICollection<KPFormularioDetalle> KPFormularioDetalles { get; set; }

    }
}