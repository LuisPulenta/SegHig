using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SegHig.Data;
using SegHig.Data.Entities;
using SegHig.Helpers;
using SegHig.Models;
using Vereyon.Web;

namespace SegHig.Controllers
{
    [Authorize(Roles = "User")]
    public class ClientesController : Controller
    {
        private readonly DataContext _context;
        private readonly ICombosHelper _combosHelper;
        private readonly IUserHelper _userHelper;
        private readonly IFlashMessage _flashMessage;

        public ClientesController(DataContext context, ICombosHelper combosHelper,IUserHelper userHelper, IFlashMessage flashMessage)
        {
            _context = context;
            _combosHelper = combosHelper;
            _userHelper = userHelper;
            _flashMessage = flashMessage;
        }

        // GET: Clientes
        public async Task<IActionResult> Index()
        {

            User user = await _userHelper.GetUserAsync(User.Identity.Name);
            return View(await _context.Clientes
                .Include(t => t.ClienteTipo)
                .Include(t => t.Empresa)
                .Where(t => t.Empresa == user.Empresa)
                .ToListAsync());
        }

        // GET: Clientes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Clientes == null)
            {
                return NotFound();
            }

            var ClienteTipo = await _context.Clientes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ClienteTipo == null)
            {
                return NotFound();
            }

            return View(ClienteTipo);
        }

        // GET: Clientes/Create


        public async Task<IActionResult> Create()
        {
            ClienteViewModel model = new ClienteViewModel
            {
                Id = 1,
                ClienteTipos = await _combosHelper.GetComboClienteTiposAsync(),
            };
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(ClienteViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _userHelper.GetUserAsync(User.Identity.Name);
                Cliente Cliente = new()
                {
                    Name = model.Name,
                    Active = true,
                    Address = model.Address,
                    Phone = model.Phone,
                    Empresa=user.Empresa,
                    ClienteTipo = await _context.ClienteTipos.FindAsync(model.ClienteTipoId),
                };
                try
                {
                    _context.Add(Cliente);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicada"))
                    {
                        ModelState.AddModelError(string.Empty, "Ya existe una Cliente con el mismo nombre.");
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
            model.ClienteTipos = await _combosHelper.GetComboClienteTiposAsync();
            return View(model);
        }

        // GET: Clientes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Clientes == null)
            {
                return NotFound();
            }

            Cliente Cliente = await _context.Clientes
                .Include(t => t.ClienteTipo)
                .FirstOrDefaultAsync(t => t.Id == id);
            if (Cliente == null)
            {
                return NotFound();
            }
            ClienteViewModel model = new()
            {
                Id = Cliente.Id,
                Name = Cliente.Name,
                Active = Cliente.Active,
                Address = Cliente.Address,
                Phone = Cliente.Phone,
                ClienteTipoId = Cliente.ClienteTipo.Id,
                ClienteTipos = await _combosHelper.GetComboClienteTiposAsync(),
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ClienteViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Cliente Cliente = await _context.Clientes.FindAsync(model.Id);

                    Cliente.Active = model.Active;
                    Cliente.Name = model.Name;
                    Cliente.Address = model.Address;
                    Cliente.Phone = model.Phone;
                    Cliente.ClienteTipo = await _context.ClienteTipos.FindAsync(model.ClienteTipoId);
                    _context.Update(Cliente);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicada"))
                    {
                        ModelState.AddModelError(string.Empty, "Ya existe una Cliente con el mismo nombre.");
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
            model.ClienteTipos = await _combosHelper.GetComboClienteTiposAsync();
            return View(model);
            //return RedirectToAction(nameof(Index));
        }

        // GET: Clientes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Clientes == null)
            {
                return NotFound();
            }

            var ClienteTipo = await _context.Clientes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ClienteTipo == null)
            {
                return NotFound();
            }

            return View(ClienteTipo);
        }

        // POST: Clientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Clientes == null)
            {
                return Problem("Entity set 'DataContext.Clientes'  is null.");
            }
            var ClienteTipo = await _context.Clientes.FindAsync(id);
            if (ClienteTipo != null)
            {
                _context.Clientes.Remove(ClienteTipo);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClienteTipoExists(int id)
        {
            return _context.Clientes.Any(e => e.Id == id);
        }
    }
}

