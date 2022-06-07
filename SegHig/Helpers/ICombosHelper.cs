using Microsoft.AspNetCore.Mvc.Rendering;

namespace SegHig.Helpers
{
    public interface ICombosHelper
    {
        Task<IEnumerable<SelectListItem>> GetComboClienteTiposAsync();

        Task<IEnumerable<SelectListItem>> GetComboEmpresaTiposAsync();

        Task<IEnumerable<SelectListItem>> GetComboEmpresasAsync();

    }
}
