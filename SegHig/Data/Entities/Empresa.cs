using System.ComponentModel.DataAnnotations;

namespace SegHig.Data.Entities
{
    public class Empresa
    {
        [Key]
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

        [Display(Name = "Tipo de Empresa")]
        public EmpresaTipo EmpresaTipo { get; set; }

        [Display(Name = "Activo")]
        public bool Active { get; set; }
        
        [Display(Name = "Usuario Alta")]
        public ICollection<User> Users { get; set; }
        public ICollection<EmpresaTipo> EmpresaTipos { get; set; }
        public ICollection<Cliente> Clientes { get; set; }
    }
}