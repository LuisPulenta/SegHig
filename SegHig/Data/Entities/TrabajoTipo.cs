using System.ComponentModel.DataAnnotations;

namespace SegHig.Data.Entities
{
    public class TrabajoTipo
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Nombre")]
        [MaxLength(50, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public String Name { get; set; }
        public Cliente Cliente { get; set; }
        [Display(Name = "Activo")]
        public bool Active { get; set; }
        public ICollection<Formulario> Formularios { get; set; }

    }
}