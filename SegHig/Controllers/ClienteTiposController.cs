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

        

        [NoDirectAccess]
        public async Task<IActionResult> Delete(int? id)
        {
            ClienteTipo ClienteTipo = await _context.ClienteTipos.FirstOrDefaultAsync(c => c.Id == id);
            try
            {
                _context.ClienteTipos.Remove(ClienteTipo);
                await _context.SaveChangesAsync();
                _flashMessage.Info("Registro borrado.");
            }
            catch
            {
                _flashMessage.Danger("No se puede borrar el Tipo de Cliente porque tiene registros relacionados.");
            }

            return RedirectToAction(nameof(Index));
        }

        [NoDirectAccess]
        public async Task<IActionResult> AddOrEdit(int id = 0)
        {
            if (id == 0)
            {
                return View(new ClienteTipo());
            }
            else
            {
                ClienteTipo ClienteTipo = await _context.ClienteTipos.FindAsync(id);
                if (ClienteTipo == null)
                {
                    return NotFound();
                }

                return View(ClienteTipo);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(int id, ClienteTipo ClienteTipo)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (id == 0) //Insert
                    {
                        _context.Add(ClienteTipo);
                        await _context.SaveChangesAsync();
                        _flashMessage.Info("Registro creado.");
                    }
                    else //Update
                    {
                        _context.Update(ClienteTipo);
                        await _context.SaveChangesAsync();
                        _flashMessage.Info("Registro actualizado.");
                    }
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicada"))
                    {
                        _flashMessage.Danger("Ya existe un Tipo de Cliente con el mismo nombre.");
                    }
                    else
                    {
                        _flashMessage.Danger(dbUpdateException.InnerException.Message);
                    }
                    return View(ClienteTipo);
                }
                catch (Exception exception)
                {
                    _flashMessage.Danger(exception.Message);
                    return View(ClienteTipo);
                }

                return Json(new { isValid = true, html = ModalHelper.RenderRazorViewToString(this, "_ViewAll", _context.ClienteTipos.ToList()) });

            }

            return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "AddOrEdit", ClienteTipo) });
        }

    }
}
