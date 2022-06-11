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
    public class EmpresasController : Controller
    {
        private readonly DataContext _context;
        private readonly ICombosHelper _combosHelper;
        private readonly IFlashMessage _flashMessage;

        public EmpresasController(DataContext context, ICombosHelper combosHelper, IFlashMessage flashMessage)
        {
            _context = context;
            _combosHelper = combosHelper;
            _flashMessage = flashMessage;
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


        [NoDirectAccess]
        public async Task<IActionResult> Create()
        {
            EmpresaViewModel model = new EmpresaViewModel
            {
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
                    _flashMessage.Info("Empresa creada.");
                    return Json(new
                    {
                        isValid = true,
                        html = ModalHelper.RenderRazorViewToString(this, "_ViewAll", _context.Empresas.ToList())
                    });

                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicada"))
                    {
                        _flashMessage.Danger("Ya existe una Empresa con el mismo nombre.");
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
            model.EmpresaTipos = await _combosHelper.GetComboEmpresaTiposAsync();
            return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "Create", model) });
        }

        // GET: Empresas/Edit/5
        [NoDirectAccess]
        public async Task<IActionResult> Edit(int id)
        {
        
            Empresa empresa = await _context.Empresas
                .Include(t => t.EmpresaTipo)
                .FirstOrDefaultAsync(t => t.Id == id);
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
                    _flashMessage.Info("Empresa actualizada.");
                    return Json(new
                    {
                        isValid = true,
                        html = ModalHelper.RenderRazorViewToString(this, "_ViewAll", _context.Empresas
                        .Include(p => p.Users)
                        .ToList())
                    });

                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicada"))
                    {
                        _flashMessage.Danger("Ya existe una Empresa con el mismo nombre.");
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
            model.EmpresaTipos = await _combosHelper.GetComboEmpresaTiposAsync();
            return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "Edit", model) });
            //return RedirectToAction(nameof(Index));
        }

        // GET: Empresas/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            Empresa empresa = await _context.Empresas
                .Include(p => p.Users)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (empresa == null)
            {
                return NotFound();
            }

            _context.Empresas.Remove(empresa);
            await _context.SaveChangesAsync();
            _flashMessage.Info("Registro borrado.");
            return RedirectToAction(nameof(Index));
        }

    }
}