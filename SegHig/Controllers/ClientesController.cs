using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SegHig.Data;
using SegHig.Data.Entities;
using SegHig.Helpers;
using SegHig.Models;
using Vereyon.Web;
using static SegHig.Helpers.ModalHelper;

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
                .Include(t => t.Empleados)
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
                .Include(t => t.Empleados)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ClienteTipo == null)
            {
                return NotFound();
            }

            return View(ClienteTipo);
        }

        // GET: Clientes/Create


        [NoDirectAccess]
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
                    return Json(new
                    {
                        isValid = true,
                        html = ModalHelper.RenderRazorViewToString(this, "_ViewAllClientes", _context.Empresas.ToList())
                    });
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
            return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "Create", model) });
        }

        // GET: Clientes/Edit/5
        [NoDirectAccess]
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
                    return Json(new
                    {
                        isValid = true,
                        html = ModalHelper.RenderRazorViewToString(this, "_ViewAllClientes", _context.Empresas
                        .Include(p => p.Users)
                        .ToList())
                    });
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
            return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "Edit", model) });
        }

        [NoDirectAccess]
        public async Task<IActionResult> Delete(int? id)
        {
            Cliente cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Id == id);
            try
            {
                _context.Clientes.Remove(cliente);
                await _context.SaveChangesAsync();
                _flashMessage.Info("Cliente borrado.");
            }
            catch
            {
                _flashMessage.Danger("No se puede borrar el Cliente porque tiene registros relacionados.");
            }

            return RedirectToAction(nameof(Index));
        }

        [NoDirectAccess]
        public async Task<IActionResult> AddOrEdit(int id = 0)
        {
            if (id == 0)
            {
                return View(new Cliente());
            }
            else
            {
                Cliente cliente = await _context.Clientes.FindAsync(id);
                if (cliente == null)
                {
                    return NotFound();
                }

                return View(cliente);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(int id, Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (id == 0) //Insert
                    {
                        _context.Add(cliente);
                        await _context.SaveChangesAsync();
                        _flashMessage.Info("Cliente creado.");
                    }
                    else //Update
                    {
                        _context.Update(cliente);
                        await _context.SaveChangesAsync();
                        _flashMessage.Info("Cliente actualizado.");
                    }
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicada"))
                    {
                        _flashMessage.Danger("Ya existe un Cliente con el mismo nombre.");
                    }
                    else
                    {
                        _flashMessage.Danger(dbUpdateException.InnerException.Message);
                    }
                    return View(cliente);
                }
                catch (Exception exception)
                {
                    _flashMessage.Danger(exception.Message);
                    return View(cliente);
                }

                return Json(new { isValid = true, html = ModalHelper.RenderRazorViewToString(this, "_ViewAllClientes", _context.Clientes.Include(c => c.TrabajoTipos).ToList()) });

            }

            return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "AddOrEdit", cliente) });
        }


        private bool ClienteTipoExists(int id)
        {
            return _context.Clientes.Any(e => e.Id == id);
        }

        // GET: Clientes/AddTipoTrabajo
        [NoDirectAccess]
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
                        Orden = model.Orden,
                        Name = model.Name,
                        Active=true,
                        Cliente = await _context.Clientes.FindAsync(model.ClienteId),
                    };
                    _context.Add(trabajoTipo);
                    await _context.SaveChangesAsync();
                    Cliente cliente = await _context.Clientes
                        .Include(c => c.TrabajoTipos)
                        .ThenInclude(s => s.Formularios)
                        .FirstOrDefaultAsync(c => c.Id == model.ClienteId);
                    _flashMessage.Info("Tipo de Trabajo creado.");
                    return Json(new { isValid = true, html = ModalHelper.RenderRazorViewToString(this, "_ViewAllTrabajoTipos", trabajoTipo) });


                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicada"))
                    {
                        _flashMessage.Danger("Ya existe un Tipo de Trabajo con el mismo nombre en este Cliente.");
                    }
                    else
                    {
                        _flashMessage.Danger(dbUpdateException.InnerException.Message);
                    }
                }
                catch (Exception exception)
                {
                    _flashMessage.Danger(exception.Message);
                }
            }
            return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "AddTipoTrabajo", model) });
        }

        // GET: Clientes/TrabajoTipo/5
        [NoDirectAccess]
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
                Orden = trabajoTipo.Orden,
                Name = trabajoTipo.Name,
                Active = trabajoTipo.Active
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
                        Orden = model.Orden,
                        Name = model.Name,
                        Active = model.Active,
                    };

                    _context.Update(trabajoTipo);
                    await _context.SaveChangesAsync();
                    Cliente cliente = await _context.Clientes
                       .Include(c => c.TrabajoTipos)
                       .ThenInclude(s => s.Formularios)
                       .FirstOrDefaultAsync(c => c.Id == model.ClienteId);
                    _flashMessage.Info("Tipo de Trabajo actualizado.");
                    return Json(new { isValid = true, html = ModalHelper.RenderRazorViewToString(this, "_ViewAllTrabajoTipos", trabajoTipo) });
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicada"))
                    {
                        _flashMessage.Danger("Ya existe un Tipo de Trabajo con el mismo nombre en este Cliente.");
                    }
                    else
                    {
                        _flashMessage.Danger(dbUpdateException.InnerException.Message);
                    }
                }
                catch (Exception exception)
                {
                    _flashMessage.Danger(exception.Message);
                }
            }
            return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "EditTrabajoTipo", model) });
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

        [NoDirectAccess]
        public async Task<IActionResult> DeleteTrabajoTipo(int id)
        {
            TrabajoTipo trabajoTipo = await _context.TrabajoTipos
                .Include(s => s.Cliente)
                .Include(s => s.Formularios)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (trabajoTipo == null)
            {
                return NotFound();
            }

            try
            {
                _context.TrabajoTipos.Remove(trabajoTipo);
                await _context.SaveChangesAsync();
                _flashMessage.Info("Tipo de Trabajo borrado.");
            }
            catch
            {
                _flashMessage.Danger("No se puede borrar el Tipo de Trabajo porque tiene registros relacionados.");
            }

            return RedirectToAction(nameof(Details), new { Id = trabajoTipo.Cliente.Id });
        }



        // GET: Clientes/AddFormulario
        [NoDirectAccess]
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
                        Orden = model.Orden,
                        Name = model.Name,
                        Active = true,
                        TrabajoTipo = await _context.TrabajoTipos.FindAsync(model.TrabajoTipoId),
                    };
                    _context.Add(formulario);
                    await _context.SaveChangesAsync();
                    TrabajoTipo trabajoTipo = await _context.TrabajoTipos
                        .Include(c => c.Cliente)
                        .Include(c => c.Formularios)
                        .ThenInclude(s => s.FormularioDetalles)
                        .FirstOrDefaultAsync(c => c.Id == model.TrabajoTipoId);
                    _flashMessage.Info("Formulario creado.");
                    return Json(new { isValid = true, html = ModalHelper.RenderRazorViewToString(this, "_ViewAllFormularios", trabajoTipo) });
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicada"))
                    {
                        _flashMessage.Info("Ya existe un Formulario con el mismo nombre en este Tipo de Trabajo.");
                    }
                    else
                    {
                        _flashMessage.Info(dbUpdateException.InnerException.Message);
                    }
                }
                catch (Exception exception)
                {
                    _flashMessage.Info(string.Empty, exception.Message);
                }
            }
            return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "AddFormulario", model) });
        }

        // GET: Clientes/Formulario/5
        [NoDirectAccess]
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
                Orden = formulario.Orden,
                Name = formulario.Name,
                Active = formulario.Active
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
                        Orden = model.Orden,
                        Name = model.Name,
                        Active = model.Active,
                    };

                    _context.Update(formulario);
                    await _context.SaveChangesAsync();
                    TrabajoTipo trabajoTipo = await _context.TrabajoTipos
                       .Include(c => c.Cliente)
                       .Include(c => c.Formularios)
                       .ThenInclude(s => s.FormularioDetalles)
                       .FirstOrDefaultAsync(c => c.Id == model.TrabajoTipoId);
                    _flashMessage.Info("Formulario actualizado.");
                    return Json(new { isValid = true, html = ModalHelper.RenderRazorViewToString(this, "_ViewAllFormularios", trabajoTipo) });
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicada"))
                    {
                        _flashMessage.Danger("Ya existe un Formulario con el mismo nombre en este Tipo de Trabajo.");
                    }
                    else
                    {
                        _flashMessage.Danger(dbUpdateException.InnerException.Message);
                    }
                }
                catch (Exception exception)
                {
                    _flashMessage.Danger(exception.Message);
                }
            }
            return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "EditFormulario", model) });
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

        [NoDirectAccess]
        public async Task<IActionResult> DeleteFormulario(int id)
        {
            Formulario formulario= await _context.Formularios
                .Include(s => s.TrabajoTipo)
                .Include(s => s.FormularioDetalles)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (formulario == null)
            {
                return NotFound();
            }

            try
            {
                _context.Formularios.Remove(formulario);
                await _context.SaveChangesAsync();
                _flashMessage.Info("Formulario borrado.");
            }
            catch
            {
                _flashMessage.Danger("No se puede borrar el Formulario porque tiene registros relacionados.");
            }
            return RedirectToAction(nameof(DetailsTrabajoTipo), new { Id = formulario.TrabajoTipo.Id });
        }
 

        // GET: Clientes/AddFormularioDetalle

        [NoDirectAccess]
        public async Task<IActionResult> AddFormularioDetalle(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Formulario formulario = await _context.Formularios.FindAsync(id);
            if (formulario == null)
            {
                return NotFound();
            }
            FormularioDetalleViewModel model = new()
            {
                FormularioId = formulario.Id,
                Active = true,
                Ponderacion=0,
            };
            return View(model);
        }

        // POST: Countries/AddFormularioDetalle
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddFormularioDetalle(FormularioDetalleViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    FormularioDetalle formularioDetalle = new()
                    {
                        Orden = model.Orden,
                        Description = model.Description,
                        Active = true,
                        Ponderacion=model.Ponderacion,
                        Formulario = await _context.Formularios.FindAsync(model.FormularioId),
                    };
                    _context.Add(formularioDetalle);
                    await _context.SaveChangesAsync();
                    Formulario formulario = await _context.Formularios
                       .Include(c => c.TrabajoTipo)
                       .Include(c => c.FormularioDetalles)
                       .FirstOrDefaultAsync(c => c.Id == model.FormularioId);
                    _flashMessage.Info("Detalle creado.");
                    return Json(new { isValid = true, html = ModalHelper.RenderRazorViewToString(this, "_ViewAllFormularioDetalles", formulario) });
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicada"))
                    {
                        _flashMessage.Danger("Ya existe un Detalle con el mismo nombre en este Formulario.");
                    }
                    else
                    {
                        _flashMessage.Danger(dbUpdateException.InnerException.Message);
                    }
                }
                catch (Exception exception)
                {
                    _flashMessage.Danger(exception.Message);
                }
            }
            return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "AddFormularioDetalle", model) });
        }

        // GET: Clientes/FormularioDetalle/5

        [NoDirectAccess]
        public async Task<IActionResult> EditFormularioDetalle(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            FormularioDetalle formularioDetalle = await _context.FormularioDetalles
                .Include(s => s.Formulario)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (formularioDetalle == null)
            {
                return NotFound();
            }

            FormularioDetalleViewModel model = new()
            {
                FormularioId = formularioDetalle.Formulario.Id,
                Id = formularioDetalle.Id,
                Orden = formularioDetalle.Orden,
                Description = formularioDetalle.Description,
                Ponderacion = formularioDetalle.Ponderacion,
                Active = formularioDetalle.Active,

            };

            return View(model);
        }

        // POST: Clientes/FormularioDetalle/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditFormularioDetalle(int id, FormularioDetalleViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    FormularioDetalle formularioDetalle = new()
                    {
                        Id = model.Id,
                        Orden = model.Orden,
                        Description = model.Description,
                        Ponderacion = model.Ponderacion,
                        Active =model.Active,
                    };

                    _context.Update(formularioDetalle);
                    await _context.SaveChangesAsync();
                    Formulario formulario = await _context.Formularios
                       .Include(c => c.TrabajoTipo)
                       .Include(s => s.FormularioDetalles)
                       .FirstOrDefaultAsync(c => c.Id == model.FormularioId);
                    _flashMessage.Info("Formulario actualizado.");
                    return Json(new { isValid = true, html = ModalHelper.RenderRazorViewToString(this, "_ViewAllFormularioDetalles", formulario) });
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicada"))
                    {
                        _flashMessage.Danger("Ya existe un Detalle con la misma descripción en este Formulario.");
                    }
                    else
                    {
                        _flashMessage.Danger(dbUpdateException.InnerException.Message);
                    }
                }
                catch (Exception exception)
                {
                    _flashMessage.Danger(exception.Message);
                }
            }
            return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "EditFormularioDetalle", model) });
        }

        // GET: Clientes/DetailsFormularioDetalle/5
        public async Task<IActionResult> DetailsFormularioDetalle(int? id)
        {
            if (id == null || _context.FormularioDetalles == null)
            {
                return NotFound();
            }

            var formularioDetalle = await _context.FormularioDetalles
                .Include(c => c.Formulario)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (formularioDetalle == null)
            {
                return NotFound();
            }

            return View(formularioDetalle);
        }

        [NoDirectAccess]
        public async Task<IActionResult> DeleteFormularioDetalle(int id)
        {
            FormularioDetalle formularioDetalle = await _context.FormularioDetalles
                .Include(s => s.Formulario)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (formularioDetalle == null)
            {
                return NotFound();
            }

            try
            {
                _context.FormularioDetalles.Remove(formularioDetalle);
                await _context.SaveChangesAsync();
                _flashMessage.Info("Detalle borrado.");
            }
            catch
            {
                _flashMessage.Danger("No se puede borrar el Detalle porque tiene registros relacionados.");
            }
            return RedirectToAction(nameof(DetailsFormulario), new { Id = formularioDetalle.Formulario.Id });
        }


        public async Task<IActionResult> OnOff(int id)
        {
            Cliente cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }

            cliente.Active = !cliente.Active;

            _context.Update(cliente);
            await _context.SaveChangesAsync();
            if (cliente.Active)
            {
                _flashMessage.Info("Cliente activado.");
            }
            else
            {
                _flashMessage.Info("Cliente desactivado.");
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> OnOffTrabajoTipo(int id)
        {
            TrabajoTipo trabajoTipo = await _context.TrabajoTipos
                .Include(t=> t.Cliente)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (trabajoTipo == null)
            {
                return NotFound();
            }

            trabajoTipo.Active = !trabajoTipo.Active;

            _context.Update(trabajoTipo);
            await _context.SaveChangesAsync();
            if (trabajoTipo.Active)
            {
                _flashMessage.Info("Tipo de Trabajo activado.");
            }
            else
            {
                _flashMessage.Info("Tipo de Trabajo desactivado.");
            }
            return RedirectToAction(nameof(Details), new { Id = trabajoTipo.Cliente.Id });
        }

        public async Task<IActionResult> OnOffFormulario(int id)
        {
            Formulario formulario = await _context.Formularios
                .Include(t => t.TrabajoTipo)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (formulario == null)
            {
                return NotFound();
            }

            formulario.Active = !formulario.Active;

            _context.Update(formulario);
            await _context.SaveChangesAsync();
            if (formulario.Active)
            {
                _flashMessage.Info("Formulario activado.");
            }
            else
            {
                _flashMessage.Info("Formulario desactivado.");
            }
            return RedirectToAction(nameof(DetailsTrabajoTipo), new { Id = formulario.TrabajoTipo.Id });
        }

        public async Task<IActionResult> OnOffFormularioDetalle(int id)
        {
            FormularioDetalle formularioDetalle = await _context.FormularioDetalles
                .Include(t => t.Formulario)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (formularioDetalle == null)
            {
                return NotFound();
            }

            formularioDetalle.Active = !formularioDetalle.Active;

            _context.Update(formularioDetalle);
            await _context.SaveChangesAsync();
            if (formularioDetalle.Active)
            {
                _flashMessage.Info("Detalle activado.");
            }
            else
            {
                _flashMessage.Info("Detalle desactivado.");
            }
            return RedirectToAction(nameof(DetailsFormulario), new { Id = formularioDetalle.Formulario.Id });
        }

    }
}

