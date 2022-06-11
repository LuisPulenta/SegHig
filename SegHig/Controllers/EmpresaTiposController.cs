using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SegHig.Data;
using SegHig.Data.Entities;
using SegHig.Helpers;
using Vereyon.Web;
using static SegHig.Helpers.ModalHelper;

namespace SegHig.Controllers
{
    [Authorize(Roles = "Admin")]
    public class EmpresaTiposController : Controller
    {
        private readonly DataContext _context;
        private readonly IFlashMessage _flashMessage;

        public EmpresaTiposController(DataContext context, IFlashMessage flashMessage)
        {
            _context = context;
            _flashMessage = flashMessage;
        }

        // GET: EmpresaTipos
        public async Task<IActionResult> Index()
        {
            return View(await _context.EmpresaTipos.ToListAsync());
        }

        // GET: EmpresaTipos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.EmpresaTipos == null)
            {
                return NotFound();
            }

            var empresaTipo = await _context.EmpresaTipos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (empresaTipo == null)
            {
                return NotFound();
            }

            return View(empresaTipo);
        }

        
        [NoDirectAccess]
        public async Task<IActionResult> Delete(int? id)
        {
            EmpresaTipo empresaTipo = await _context.EmpresaTipos.FirstOrDefaultAsync(c => c.Id == id);
            try
            {
                _context.EmpresaTipos.Remove(empresaTipo);
                await _context.SaveChangesAsync();
                _flashMessage.Info("Registro borrado.");
            }
            catch
            {
                _flashMessage.Danger("No se puede borrar el Tipo de Empresa porque tiene registros relacionados.");
            }

            return RedirectToAction(nameof(Index));
        }

        [NoDirectAccess]
        public async Task<IActionResult> AddOrEdit(int id = 0)
        {
            if (id == 0)
            {
                return View(new EmpresaTipo());
            }
            else
            {
                EmpresaTipo empresaTipo = await _context.EmpresaTipos.FindAsync(id);
                if (empresaTipo == null)
                {
                    return NotFound();
                }

                return View(empresaTipo);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(int id, EmpresaTipo empresaTipo)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (id == 0) //Insert
                    {
                        _context.Add(empresaTipo);
                        await _context.SaveChangesAsync();
                        _flashMessage.Info("Registro creado.");
                    }
                    else //Update
                    {
                        _context.Update(empresaTipo);
                        await _context.SaveChangesAsync();
                        _flashMessage.Info("Registro actualizado.");
                    }
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicada"))
                    {
                        _flashMessage.Danger("Ya existe un Tipo de Empresa con el mismo nombre.");
                    }
                    else
                    {
                        _flashMessage.Danger(dbUpdateException.InnerException.Message);
                    }
                    return View(empresaTipo);
                }
                catch (Exception exception)
                {
                    _flashMessage.Danger(exception.Message);
                    return View(empresaTipo);
                }

                return Json(new { isValid = true, html = ModalHelper.RenderRazorViewToString(this, "_ViewAll", _context.EmpresaTipos.ToList()) });

            }

            return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "AddOrEdit", empresaTipo) });
        }


    }
}
