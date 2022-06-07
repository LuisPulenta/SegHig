using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SegHig.Data;
using SegHig.Data.Entities;

namespace SegHig.Controllers
{
    [Authorize(Roles = "Admin")]
    public class EmpresaTiposController : Controller
    {
        private readonly DataContext _context;
        private object dbUpdateException;

        public EmpresaTiposController(DataContext context)
        {
            _context = context;
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

        // GET: EmpresaTipos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: EmpresaTipos/Create
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmpresaTipo empresaTipo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(empresaTipo);
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicada"))
                    {
                        ModelState.AddModelError(string.Empty, "Ya existe un Tipo de Empresa con el mismo nombre.");
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
            return View(empresaTipo);
        }

        // GET: EmpresaTipos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.EmpresaTipos == null)
            {
                return NotFound();
            }

            var empresaTipo = await _context.EmpresaTipos.FindAsync(id);
            if (empresaTipo == null)
            {
                return NotFound();
            }
            return View(empresaTipo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EmpresaTipo empresaTipo)
        {
            if (id != empresaTipo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(empresaTipo);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicada"))
                    {
                        ModelState.AddModelError(string.Empty, "Ya existe un Tipo de Empresa con el mismo nombre.");
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
            return View(empresaTipo);
        }

        // GET: EmpresaTipos/Delete/5
        public async Task<IActionResult> Delete(int? id)
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

        // POST: EmpresaTipos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.EmpresaTipos == null)
            {
                return Problem("Entity set 'DataContext.EmpresaTipos'  is null.");
            }
            var empresaTipo = await _context.EmpresaTipos.FindAsync(id);
            if (empresaTipo != null)
            {
                _context.EmpresaTipos.Remove(empresaTipo);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmpresaTipoExists(int id)
        {
            return _context.EmpresaTipos.Any(e => e.Id == id);
        }
    }
}
