using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SegHig.Data;
using SegHig.Data.Entities;
using SegHig.Helpers;
using SegHig.Models;

namespace SegHig.Controllers
{
    [Authorize(Roles = "Admin")]
    public class EmpresasController : Controller
    {
        private readonly DataContext _context;
        private readonly ICombosHelper _combosHelper;
        private object dbUpdateException;

        public EmpresasController(DataContext context,ICombosHelper combosHelper)
        {
            _context = context;
            _combosHelper = combosHelper;
        }

        // GET: Empresas
        public async Task<IActionResult> Index()
        {
            return View(await _context.Empresas
                .Include(t => t.EmpresaTipo)
                .Include(t => t.Users)
                .ToListAsync());
        }

        // GET: Empresas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Empresas == null)
            {
                return NotFound();
            }

            var empresaTipo = await _context.Empresas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (empresaTipo == null)
            {
                return NotFound();
            }

            return View(empresaTipo);
        }

        // GET: Empresas/Create


        public async Task<IActionResult> Create()
        {
            EmpresaViewModel model = new EmpresaViewModel
            {
                Id = 1,
                EmpresaTipos = await _combosHelper.GetComboEmpresaTiposAsync(),
            };
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(EmpresaViewModel model)
        {
            if (ModelState.IsValid)
            {
                Empresa empresa = new()
                {
                    Name = model.Name,
                    Active = true,
                    Address = model.Address,
                    Phone = model.Phone,
                    EmpresaTipo = await _context.EmpresaTipos.FindAsync(model.EmpresaTipoId),
                };
                try
                {
                    _context.Add(empresa);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicada"))
                    {
                        ModelState.AddModelError(string.Empty, "Ya existe una Empresa con el mismo nombre.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, dbUpdateException.InnerException.Message);
                    }
                }
                catch (Exception exception)
                {
                    ModelState.AddModelError(string.Empty, exception.Message);
                }

            }
            model.EmpresaTipos = await _combosHelper.GetComboEmpresaTiposAsync();
            return View(model);
        }

        // GET: Empresas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Empresas == null)
            {
                return NotFound();
            }

            Empresa empresa = await _context.Empresas
                .Include(t=>t.EmpresaTipo)
                .FirstOrDefaultAsync(t=>t.Id == id);
            if (empresa == null)
            {
                return NotFound();
            }
            EmpresaViewModel model = new()
            {
                Id = empresa.Id,
                Name = empresa.Name,
                Active = empresa.Active,
                Address = empresa.Address,
                Phone = empresa.Phone,
                EmpresaTipoId = empresa.EmpresaTipo.Id,
                EmpresaTipos = await _combosHelper.GetComboEmpresaTiposAsync(),
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EmpresaViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Empresa empresa = await _context.Empresas.FindAsync(model.Id);

                    empresa.Active = model.Active;
                    empresa.Name = model.Name;
                    empresa.Address = model.Address;
                    empresa.Phone = model.Phone;
                    empresa.EmpresaTipo = await _context.EmpresaTipos.FindAsync(model.EmpresaTipoId);
                    _context.Update(empresa);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicada"))
                    {
                        ModelState.AddModelError(string.Empty, "Ya existe una Empresa con el mismo nombre.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, dbUpdateException.InnerException.Message);
                    }
                }
                catch (Exception exception)
                {
                    ModelState.AddModelError(string.Empty, exception.Message);
                }
                
            }
            model.EmpresaTipos = await _combosHelper.GetComboEmpresaTiposAsync();
            return View(model);
            //return RedirectToAction(nameof(Index));
        }

        // GET: Empresas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Empresas == null)
            {
                return NotFound();
            }

            var empresaTipo = await _context.Empresas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (empresaTipo == null)
            {
                return NotFound();
            }

            return View(empresaTipo);
        }

        // POST: Empresas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Empresas == null)
            {
                return Problem("Entity set 'DataContext.Empresas'  is null.");
            }
            var empresaTipo = await _context.Empresas.FindAsync(id);
            if (empresaTipo != null)
            {
                _context.Empresas.Remove(empresaTipo);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmpresaTipoExists(int id)
        {
            return _context.Empresas.Any(e => e.Id == id);
        }
    }
}
