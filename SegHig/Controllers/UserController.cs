using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SegHig.Commons;
using SegHig.Data;
using SegHig.Data.Entities;
using SegHig.Enums;
using SegHig.Helpers;
using SegHig.Models;
using Vereyon.Web;
using static SegHig.Helpers.ModalHelper;

namespace Shooping.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private readonly ICombosHelper _combosHelper;
        private readonly IMailHelper _mailHelper;
        private readonly IFlashMessage _flashMessage;

        public UsersController(DataContext context, IUserHelper userHelper, ICombosHelper combosHelper, IMailHelper mailHelper, IFlashMessage flashMessage)
        {
            _context = context;
            _userHelper = userHelper;
            _combosHelper = combosHelper;
            _mailHelper = mailHelper;
            _flashMessage = flashMessage;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Users
                .Include(u => u.Empresa)
                .Where(t=>t.UserType==UserType.Admin)
                .ToListAsync());
        }

        [NoDirectAccess]
        public async Task<IActionResult> Create()
        {
            AddUserViewModel model = new AddUserViewModel
            {
                Id = "1",
                EmpresaId=1,
                Empresas = await _combosHelper.GetComboEmpresasAsync(),
                Active =true,
                UserType = UserType.Admin,
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddUserViewModel model)
        {
            model.Active = true;
            if (ModelState.IsValid)
            {
                model.EmpresaId = null;
                User user = await _userHelper.AddUserAsync(model);
                if (user == null)
                {
                    _flashMessage.Danger( "Este correo ya está siendo usado por otro usuario.");
                    model.UserType = UserType.Admin;
                    model.Empresas = await _combosHelper.GetComboEmpresasAsync();
                    model.Active = true;
                    return View(model);
                }
                string myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                string tokenLink = Url.Action("ConfirmEmail", "Account", new
                {
                    userid = user.Id,
                    token = myToken
                }, protocol: HttpContext.Request.Scheme);

                Response response = _mailHelper.SendMail(
                    $"{model.FirstName} {model.LastName}",
                    model.Username,
                    "SegHig - Confirmación de cuenta",
                    $"<h1>SegHig - Confirmación de cuenta</h1>" +
                    $"Para habilitar el usuario, por favor hacer clic en el siguiente enlace: " +
                    $"</hr></br><p><a href = \"{tokenLink}\">Confirmar Email</a></p>");
                if (response.IsSuccess)
                {
                    _flashMessage.Info( "Las instrucciones para habilitar el Administrador han sido enviadas al correo.");
                    return Json(new
                    {
                        isValid = true,
                        html = ModalHelper.RenderRazorViewToString(this, "_ViewAll", _context.Users
               .Where(c => c.UserType == UserType.Admin)
               .ToList())
                    });
                }
                _flashMessage.Danger(response.Message);
            }
            model.EmpresaId = null;
            model.UserType = UserType.Admin;
            model.Empresas = await _combosHelper.GetComboEmpresasAsync();
            return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "Create", model) });
        }

        [NoDirectAccess]
        public async Task<IActionResult> EditUser(string id)
        {
            User user = await _userHelper.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            EditUserViewModel model = new()
            {
                Address = user.Address,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Empresas = await _combosHelper.GetComboEmpresasAsync(),
                EmpresaId = 1,
                Id = user.Id,
                Document = user.Document,
                Active = user.Active
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _userHelper.GetUserByIdAsync(model.Id);
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Address = model.Address;
                user.PhoneNumber = model.PhoneNumber;
                //user.Empresa = await _context.Empresas.FindAsync(model.EmpresaId);
                user.Document = model.Document;
                user.Active = model.Active;

                await _userHelper.UpdateUserAsync(user);
                _flashMessage.Info("Administrador actualizado.");
                return Json(new
                {
                    isValid = true,
                    html = ModalHelper.RenderRazorViewToString(this, "_ViewAll", _context.Users
                    .ToList())
                });

            }

            model.Empresas = await _combosHelper.GetComboEmpresasAsync();
            return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "EditUser", model) });
        }

        public async Task<IActionResult> OnOff(string id)
        {
            User user = await _userHelper.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            user.Active = !user.Active;

            await _userHelper.UpdateUserAsync(user);
            return RedirectToAction("Index", "Users");
        }

        public async Task<IActionResult> DeleteUser(string? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(e => e.Empresa)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: ClienteTipos/Delete/5
        [HttpPost, ActionName("DeleteUser")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'DataContext.Users'  is null.");
            }
            var user = await _context.Users.FirstOrDefaultAsync(m => m.Id == id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }

            await _context.SaveChangesAsync();
            _flashMessage.Info("Administrador borrado.");
            return RedirectToAction(nameof(Index));
        }
    }
}