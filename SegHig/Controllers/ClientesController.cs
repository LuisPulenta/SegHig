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
                .Include(t => t.TrabajoTipos)
                .ThenInclude(t => t.Formularios)
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
                .Include(t => t.TrabajoTipos)
                .ThenInclude(t => t.Formularios)
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
                Active = true,
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
                    _flashMessage.Info("Cliente creado.");
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicada"))
                    {
                        _flashMessage.Danger( "Ya existe un Cliente con el mismo nombre.");
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
                    _flashMessage.Info("Cliente actualizado.");
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicada"))
                    {
                        _flashMessage.Danger( "Ya existe un Cliente con el mismo nombre.");
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
            var cliente = await _context.Clientes
                .Include(t => t.TrabajoTipos)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cliente != null)
            {
                _context.Clientes.Remove(cliente);
            }

            await _context.SaveChangesAsync();
            _flashMessage.Info("Cliente borrado.");
            return RedirectToAction(nameof(Index));
        }

        private bool ClienteTipoExists(int id)
        {
            return _context.Clientes.Any(e => e.Id == id);
        }

        // GET: Clientes/AddTipoTrabajo
        public async Task<IActionResult> AddTipoTrabajo(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Cliente cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }
            TrabajoTipoViewModel model = new()
            {
                ClienteId=cliente.Id,
                Active=true,
            };
            return View(model);
        }

        // POST: Countries/AddState
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTipoTrabajo(TrabajoTipoViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    TrabajoTipo trabajoTipo = new()
                    {
                        Name = model.Name,
                        Active=true,
                        Cliente = await _context.Clientes.FindAsync(model.ClienteId),
                    };
                    _context.Add(trabajoTipo);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Details), new { Id = model.ClienteId });
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicada"))
                    {
                        ModelState.AddModelError(string.Empty, "Ya existe un Tipo de Trabajo con el mismo nombre en este Cliente.");
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
            return View(model);
        }

        // GET: Clientes/TrabajoTipo/5
        public async Task<IActionResult> EditTrabajoTipo(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            TrabajoTipo trabajoTipo = await _context.TrabajoTipos
                .Include(s => s.Cliente)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (trabajoTipo == null)
            {
                return NotFound();
            }

            TrabajoTipoViewModel model = new()
            {
                ClienteId = trabajoTipo.Cliente.Id,
                Id = trabajoTipo.Id,
                Name = trabajoTipo.Name
            };

            return View(model);
        }

        // POST: Clientes/TrabajoTipo/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTrabajoTipo(int id, TrabajoTipoViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    TrabajoTipo trabajoTipo = new()
                    {
                        Id = model.Id,
                        Name = model.Name,
                    };

                    _context.Update(trabajoTipo);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Details), new { Id = model.ClienteId });
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicada"))
                    {
                        ModelState.AddModelError(string.Empty, "Ya existe un Tipo de Trabajo con el mismo nombre en este Cliente.");
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
            return View(model);
        }

        // GET: Clientes/DetailsTrabajoTipo/5
        public async Task<IActionResult> DetailsTrabajoTipo(int? id)
        {
            if (id == null || _context.TrabajoTipos == null)
            {
                return NotFound();
            }

            var trabajoTipo = await _context.TrabajoTipos
                .Include(c => c.Cliente)
                .Include(c => c.Formularios)
                .ThenInclude(c => c.FormularioDetalles)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (trabajoTipo == null)
            {
                return NotFound();
            }

            return View(trabajoTipo);
        }

        // GET: Clientes/DeleteTrabajoTipo/5
        public async Task<IActionResult> DeleteTrabajoTipo(int? id)
        {
            if (id == null || _context.TrabajoTipos == null)
            {
                return NotFound();
            }

            TrabajoTipo trabajoTipo = await _context.TrabajoTipos
                .Include(c => c.Cliente)
                .Include(c => c.Formularios)
                .ThenInclude(c => c.FormularioDetalles)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (trabajoTipo == null)
            {
                return NotFound();
            }

            return View(trabajoTipo);
        }

        // POST: Clientes/DeleteTrabajoTipo/5
        [HttpPost, ActionName("DeleteTrabajoTipo")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTrabajoTipoConfirmed(int id)
        {
            if (_context.TrabajoTipos == null)
            {
                return Problem("Entity set 'DataContext.TrabajoTipos'  is null.");
            }
            TrabajoTipo trabajoTipo = await _context.TrabajoTipos
                .Include(c => c.Cliente)
               .Include(c => c.Formularios)
                .ThenInclude(c => c.FormularioDetalles)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (trabajoTipo != null)
            {
                _context.TrabajoTipos.Remove(trabajoTipo);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { Id = trabajoTipo.Cliente.Id });
        }


        // GET: Clientes/AddFormulario
        public async Task<IActionResult> AddFormulario(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            TrabajoTipo trabajoTipo = await _context.TrabajoTipos.FindAsync(id);
            if (trabajoTipo == null)
            {
                return NotFound();
            }
            FormularioViewModel model = new()
            {
                TrabajoTipoId = trabajoTipo.Id,
                Active = true,
            };
            return View(model);
        }

        // POST: Countries/AddFormulario
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddFormulario(FormularioViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Formulario formulario = new()
                    {
                        Name = model.Name,
                        Active = true,
                        TrabajoTipo = await _context.TrabajoTipos.FindAsync(model.TrabajoTipoId),
                    };
                    _context.Add(formulario);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(DetailsTrabajoTipo), new { Id = model.TrabajoTipoId });
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicada"))
                    {
                        ModelState.AddModelError(string.Empty, "Ya existe un Formulario con el mismo nombre en este Tipo de Trabajo.");
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
            return View(model);
        }

        // GET: Clientes/Formulario/5
        public async Task<IActionResult> EditFormulario(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Formulario formulario = await _context.Formularios
                .Include(s => s.TrabajoTipo)
                .Include(s => s.FormularioDetalles)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (formulario == null)
            {
                return NotFound();
            }

            FormularioViewModel model = new()
            {
                TrabajoTipoId = formulario.TrabajoTipo.Id,
                Id = formulario.Id,
                Name = formulario.Name
            };

            return View(model);
        }

        // POST: Clientes/Formulario/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditFormulario(int id, FormularioViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Formulario formulario = new()
                    {
                        Id = model.Id,
                        Name = model.Name,
                    };

                    _context.Update(formulario);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(DetailsTrabajoTipo), new { Id = model.TrabajoTipoId });
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicada"))
                    {
                        ModelState.AddModelError(string.Empty, "Ya existe un Formulario con el mismo nombre en este Tipo de Trabajo.");
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
            return View(model);
        }

        // GET: Clientes/DetailsFormulario/5
        public async Task<IActionResult> DetailsFormulario(int? id)
        {
            if (id == null || _context.Formularios == null)
            {
                return NotFound();
            }

            var formulario = await _context.Formularios
                .Include(c => c.TrabajoTipo)
                .Include(c => c.FormularioDetalles)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (formulario == null)
            {
                return NotFound();
            }

            return View(formulario);
        }

        // GET: Clientes/DeleteFormulario/5
        public async Task<IActionResult> DeleteFormulario(int? id)
        {
            if (id == null || _context.Formularios == null)
            {
                return NotFound();
            }

            Formulario formulario = await _context.Formularios
                .Include(c => c.TrabajoTipo)
                .Include(c => c.FormularioDetalles)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (formulario == null)
            {
                return NotFound();
            }

            return View(formulario);
        }

        // POST: Clientes/DeleteFormulario/5
        [HttpPost, ActionName("DeleteFormulario")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteFormularioConfirmed(int id)
        {
            if (_context.TrabajoTipos == null)
            {
                return Problem("Entity set 'DataContext.Formularios'  is null.");
            }
            Formulario formulario = await _context.Formularios
                 .Include(c => c.TrabajoTipo)
                .Include(c => c.FormularioDetalles)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (formulario != null)
            {
                _context.Formularios.Remove(formulario);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(DetailsTrabajoTipo), new { Id = formulario.TrabajoTipo.Id });
        }



    }
}

