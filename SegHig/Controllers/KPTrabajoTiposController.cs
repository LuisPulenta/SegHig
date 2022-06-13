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
    [Authorize(Roles = "Admin")]
    public class KPTrabajoTiposController : Controller
    {
        private readonly DataContext _context;
        private readonly ICombosHelper _combosHelper;
        private readonly IUserHelper _userHelper;
        private readonly IFlashMessage _flashMessage;

        public KPTrabajoTiposController(DataContext context, ICombosHelper combosHelper,IUserHelper userHelper, IFlashMessage flashMessage)
        {
            _context = context;
            _combosHelper = combosHelper;
            _userHelper = userHelper;
            _flashMessage = flashMessage;
        }

        // GET: KPTrabajoTipos
        public async Task<IActionResult> Index()
        {

            User user = await _userHelper.GetUserAsync(User.Identity.Name);
            return View(await _context.KPTrabajoTipos
                .Include(t => t.KPFormularios)
                .ToListAsync());
        }

        // GET: Clientes/AddTipoTrabajo
        [NoDirectAccess]
        public async Task<IActionResult> AddTipoTrabajo()
        {
            KPTrabajoTipo model = new()
            {
                Active = true,
            };
            return View(model);
        }

        // POST: Countries/AddTipoTrabajo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTipoTrabajo(KPTrabajoTipo model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    KPTrabajoTipo trabajoTipo = new()
                    {
                        Orden = model.Orden,
                        Name = model.Name,
                        Active = true,
                    };
                    _context.Add(trabajoTipo);
                    await _context.SaveChangesAsync();
                    _flashMessage.Info("Tipo de Trabajo creado.");
                    return Json(new { isValid = true, html = ModalHelper.RenderRazorViewToString(this, "_ViewAllKPTrabajoTipos", trabajoTipo) });


                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicada"))
                    {
                        _flashMessage.Danger("Ya existe un Tipo de Trabajo con el mismo nombre.");
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

            var a = 1;

            KPTrabajoTipo trabajoTipo = await _context.KPTrabajoTipos
                .FirstOrDefaultAsync(s => s.Id == id);
            if (trabajoTipo == null)
            {
                return NotFound();
            }

            KPTrabajoTipo model = new()
            {
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
        public async Task<IActionResult> EditTrabajoTipo(int id, KPTrabajoTipo model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            var b = 1;

            if (ModelState.IsValid)
            {
                try
                {
                    KPTrabajoTipo trabajoTipo = new()
                    {
                        Id = model.Id,
                        Orden = model.Orden,
                        Name = model.Name,
                        Active = model.Active,
                    };

                    _context.Update(trabajoTipo);
                    await _context.SaveChangesAsync();
                    _flashMessage.Info("Tipo de Trabajo actualizado.");
                    return Json(new { isValid = true, html = ModalHelper.RenderRazorViewToString(this, "_ViewAllKPTrabajoTipos", trabajoTipo) });
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicada"))
                    {
                        _flashMessage.Danger("Ya existe un Tipo de Trabajo con el mismo nombre.");
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

            var trabajoTipo = await _context.KPTrabajoTipos
                .Include(c => c.KPFormularios)
                .ThenInclude(c => c.KPFormularioDetalles)
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
            KPTrabajoTipo trabajoTipo = await _context.KPTrabajoTipos
                .Include(s => s.KPFormularios)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (trabajoTipo == null)
            {
                return NotFound();
            }

            try
            {
                _context.KPTrabajoTipos.Remove(trabajoTipo);
                await _context.SaveChangesAsync();
                _flashMessage.Info("Tipo de Trabajo borrado.");
            }
            catch
            {
                _flashMessage.Danger("No se puede borrar el Tipo de Trabajo porque tiene registros relacionados.");
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> OnOff(int id)
        {
            KPTrabajoTipo kpTrabajoTipo = await _context.KPTrabajoTipos.FindAsync(id);
            if (kpTrabajoTipo == null)
            {
                return NotFound();
            }

            kpTrabajoTipo.Active = !kpTrabajoTipo.Active;

            _context.Update(kpTrabajoTipo);
            await _context.SaveChangesAsync();
            if (kpTrabajoTipo.Active)
            {
                _flashMessage.Info("Tipo de Trabajo activado.");
            }
            else
            {
                _flashMessage.Info("Tipo de Trabajo desactivado.");
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Clientes/AddFormulario
        [NoDirectAccess]
        public async Task<IActionResult> AddFormulario(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            KPTrabajoTipo trabajoTipo = await _context.KPTrabajoTipos.FindAsync(id);
            if (trabajoTipo == null)
            {
                return NotFound();
            }
            KPFormularioViewModel model = new()
            {
                KPTrabajoTipoId = trabajoTipo.Id,
                Active = true,
            };
            return View(model);
        }

        // POST: Countries/AddFormulario
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddFormulario(KPFormularioViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    KPFormulario formulario = new()
                    {
                        Orden = model.Orden,
                        Name = model.Name,
                        Active = true,
                        KPTrabajoTipo = await _context.KPTrabajoTipos.FindAsync(model.KPTrabajoTipoId),
                    };
                    _context.Add(formulario);
                    await _context.SaveChangesAsync();
                    KPTrabajoTipo trabajoTipo = await _context.KPTrabajoTipos
                        .Include(c => c.KPFormularios)
                        .ThenInclude(s => s.KPFormularioDetalles)
                        .FirstOrDefaultAsync(c => c.Id == model.KPTrabajoTipoId);
                    _flashMessage.Info("Formulario creado.");
                    return Json(new { isValid = true, html = ModalHelper.RenderRazorViewToString(this, "_ViewAllKPFormularios", trabajoTipo) });
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

            KPFormulario formulario = await _context.KPFormularios
                .Include(s => s.KPTrabajoTipo)
                .Include(s => s.KPFormularioDetalles)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (formulario == null)
            {
                return NotFound();
            }

            KPFormularioViewModel model = new()
            {
                KPTrabajoTipoId = formulario.KPTrabajoTipo.Id,
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
        public async Task<IActionResult> EditFormulario(int id, KPFormularioViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    KPFormulario formulario = new()
                    {
                        Id = model.Id,
                        Orden = model.Orden,
                        Name = model.Name,
                        Active = model.Active,
                    };

                    _context.Update(formulario);
                    await _context.SaveChangesAsync();
                    KPTrabajoTipo trabajoTipo = await _context.KPTrabajoTipos
                       .Include(c => c.KPFormularios)
                       .ThenInclude(s => s.KPFormularioDetalles)
                       .FirstOrDefaultAsync(c => c.Id == model.KPTrabajoTipoId);
                    _flashMessage.Info("Formulario actualizado.");
                    return Json(new { isValid = true, html = ModalHelper.RenderRazorViewToString(this, "_ViewAllKPFormularios", trabajoTipo) });
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
            if (id == null || _context.KPFormularios == null)
            {
                return NotFound();
            }

            var formulario = await _context.KPFormularios
                .Include(c => c.KPTrabajoTipo)
                .Include(c => c.KPFormularioDetalles)
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
            KPFormulario formulario = await _context.KPFormularios
                .Include(s => s.KPTrabajoTipo)
                .Include(s => s.KPFormularioDetalles)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (formulario == null)
            {
                return NotFound();
            }

            try
            {
                _context.KPFormularios.Remove(formulario);
                await _context.SaveChangesAsync();
                _flashMessage.Info("Formulario borrado.");
            }
            catch
            {
                _flashMessage.Danger("No se puede borrar el Formulario porque tiene registros relacionados.");
            }
            return RedirectToAction(nameof(DetailsTrabajoTipo), new { Id = formulario.KPTrabajoTipo.Id });
        }

        public async Task<IActionResult> OnOffFormulario(int id)
        {
            KPFormulario formulario = await _context.KPFormularios
                .Include(t => t.KPTrabajoTipo)
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
            return RedirectToAction(nameof(DetailsTrabajoTipo), new { Id = formulario.KPTrabajoTipo.Id });
        }

        // GET: Clientes/AddFormularioDetalle

        [NoDirectAccess]
        public async Task<IActionResult> AddFormularioDetalle(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            KPFormulario formulario = await _context.KPFormularios.FindAsync(id);
            if (formulario == null)
            {
                return NotFound();
            }
            KPFormularioDetalleViewModel model = new()
            {
                KPFormularioId = formulario.Id,
                Active = true,
                Ponderacion = 0,
            };
            return View(model);
        }

        // POST: Countries/AddFormularioDetalle
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddFormularioDetalle(KPFormularioDetalleViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    KPFormularioDetalle formularioDetalle = new()
                    {
                        Orden = model.Orden,
                        Description = model.Description,
                        Active = true,
                        Ponderacion = model.Ponderacion,
                        KPFormulario = await _context.KPFormularios.FindAsync(model.KPFormularioId),
                    };
                    _context.Add(formularioDetalle);
                    await _context.SaveChangesAsync();
                    KPFormulario formulario = await _context.KPFormularios
                       .Include(c => c.KPTrabajoTipo)
                       .Include(c => c.KPFormularioDetalles)
                       .FirstOrDefaultAsync(c => c.Id == model.KPFormularioId);
                    _flashMessage.Info("Detalle creado.");
                    return Json(new { isValid = true, html = ModalHelper.RenderRazorViewToString(this, "_ViewAllKPFormularioDetalles", formulario) });
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

            KPFormularioDetalle formularioDetalle = await _context.KPFormularioDetalles
                .Include(s => s.KPFormulario)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (formularioDetalle == null)
            {
                return NotFound();
            }

            KPFormularioDetalleViewModel model = new()
            {
                KPFormularioId = formularioDetalle.KPFormulario.Id,
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
        public async Task<IActionResult> EditFormularioDetalle(int id, KPFormularioDetalleViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    KPFormularioDetalle formularioDetalle = new()
                    {
                        Id = model.Id,
                        Orden = model.Orden,
                        Description = model.Description,
                        Ponderacion = model.Ponderacion,
                        Active = model.Active,
                    };

                    _context.Update(formularioDetalle);
                    await _context.SaveChangesAsync();
                    KPFormulario formulario = await _context.KPFormularios
                       .Include(c => c.KPTrabajoTipo)
                       .Include(s => s.KPFormularioDetalles)
                       .FirstOrDefaultAsync(c => c.Id == model.KPFormularioId);
                    _flashMessage.Info("Formulario actualizado.");
                    return Json(new { isValid = true, html = ModalHelper.RenderRazorViewToString(this, "_ViewAllKPFormularioDetalles", formulario) });
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

            var formularioDetalle = await _context.KPFormularioDetalles
                .Include(c => c.KPFormulario)
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
            KPFormularioDetalle formularioDetalle = await _context.KPFormularioDetalles
                .Include(s => s.KPFormulario)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (formularioDetalle == null)
            {
                return NotFound();
            }

            try
            {
                _context.KPFormularioDetalles.Remove(formularioDetalle);
                await _context.SaveChangesAsync();
                _flashMessage.Info("Detalle borrado.");
            }
            catch
            {
                _flashMessage.Danger("No se puede borrar el Detalle porque tiene registros relacionados.");
            }
            return RedirectToAction(nameof(DetailsFormulario), new { Id = formularioDetalle.KPFormulario.Id });
        }

        public async Task<IActionResult> OnOffFormularioDetalle(int id)
        {
            KPFormularioDetalle formularioDetalle = await _context.KPFormularioDetalles
                .Include(t => t.KPFormulario)
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
            return RedirectToAction(nameof(DetailsFormulario), new { Id = formularioDetalle.KPFormulario.Id });
        }


    }
}