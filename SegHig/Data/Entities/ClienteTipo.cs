using System.ComponentModel.DataAnnotations;

namespace SegHig.Data.Entities
{
    public class ClienteTipo
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Tipo de Cliente")]
        [MaxLength(50, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public String Name { get; set; }
        [Display(Name = "Activo")]
        public bool Active { get; set; }
        public ICollection<Cliente> Clientes { get; set; }
    }
}
