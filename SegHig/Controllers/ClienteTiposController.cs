using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SegHig.Data;
using SegHig.Data.Entities;
using Vereyon.Web;

namespace SegHig.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ClienteTiposController : Controller
    {
        private readonly DataContext _context;
        private readonly IFlashMessage _flashMessage;

        public ClienteTiposController(DataContext context, IFlashMessage flashMessage)
        {
            _context = context;
            _flashMessage = flashMessage;
        }

        // GET: ClienteTipos
        public async Task<IActionResult> Index()
        {
            return View(await _context.ClienteTipos.ToListAsync());
        }

        // GET: ClienteTipos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ClienteTipos == null)
            {
                return NotFound();
            }

            var ClienteTipo = await _context.ClienteTipos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ClienteTipo == null)
            {
                return NotFound();
            }

            return View(ClienteTipo);
        }

        // GET: ClienteTipos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ClienteTipos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClienteTipo ClienteTipo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ClienteTipo);
                try
                {
                    await _context.SaveChangesAsync();
                    _flashMessage.Info("Cliente creado.");
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicada"))
                    {
                        _flashMessage.Danger( "Ya existe un Tipo de Cliente con el mismo nombre.");
                    }
                    else
                    {
                        _flashMessage.Danger( dbUpdateException.InnerException.Message);
                    }
                }
                catch (Exception exception)
                {
                    _flashMessage.Danger( exception.Message);
                }

            }
            return View(ClienteTipo);
        }

        // GET: ClienteTipos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ClienteTipos == null)
            {
                return NotFound();
            }

            var ClienteTipo = await _context.ClienteTipos.FindAsync(id);
            if (ClienteTipo == null)
            {
                return NotFound();
            }
            return View(ClienteTipo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ClienteTipo ClienteTipo)
        {
            if (id != ClienteTipo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ClienteTipo);
                    await _context.SaveChangesAsync();
                    _flashMessage.Info("Tipo de Cliente actualizado.");
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicada"))
                    {
                        _flashMessage.Danger( "Ya existe un Tipo de Cliente con el mismo nombre.");
                    }
                    else
                    {
                        _flashMessage.Danger( dbUpdateException.InnerException.Message);
                    }
                }
                catch (Exception exception)
                {
                    _flashMessage.Danger( exception.Message);
                }
            }
            return View(ClienteTipo);
        }

        // GET: ClienteTipos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ClienteTipos == null)
            {
                return NotFound();
            }

            var ClienteTipo = await _context.ClienteTipos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ClienteTipo == null)
            {
                return NotFound();
            }

            return View(ClienteTipo);
        }

        // POST: ClienteTipos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ClienteTipos == null)
            {
                return Problem("Entity set 'DataContext.ClienteTipos'  is null.");
            }
            var ClienteTipo = await _context.ClienteTipos.FindAsync(id);
            if (ClienteTipo != null)
            {
                _context.ClienteTipos.Remove(ClienteTipo);
            }

            await _context.SaveChangesAsync();
            _flashMessage.Info("Tipo de Cliente borrado.");
            return RedirectToAction(nameof(Index));
        }

        private bool ClienteTipoExists(int id)
        {
            return _context.ClienteTipos.Any(e => e.Id == id);
        }
    }
}
