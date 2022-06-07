using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SegHig.Data;
using SegHig.Data.Entities;

namespace SegHig.Controllers
{
    public class ClienteTiposController : Controller
    {
        private readonly DataContext _context;
        private object dbUpdateException;

        public ClienteTiposController(DataContext context)
        {
            _context = context;
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
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                    {
                        ModelState.AddModelError(string.Empty, "Ya existe un tipo de Cliente con el mismo nombre.");
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
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                    {
                        ModelState.AddModelError(string.Empty, "Ya existe un tipo de Cliente con el mismo nombre.");
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
            return RedirectToAction(nameof(Index));
        }

        private bool ClienteTipoExists(int id)
        {
            return _context.ClienteTipos.Any(e => e.Id == id);
        }
    }
}
