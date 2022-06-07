using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SegHig.Commons;
using SegHig.Data;
using SegHig.Data.Entities;
using SegHig.Enums;
using SegHig.Helpers;
using SegHig.Models;

namespace Shooping.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private readonly ICombosHelper _combosHelper;
        private readonly IMailHelper _mailHelper;

        public UsersController(DataContext context, IUserHelper userHelper, ICombosHelper combosHelper, IMailHelper mailHelper)
        {
            _context = context;
            _userHelper = userHelper;
            _combosHelper = combosHelper;
            _mailHelper = mailHelper;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Users
                .Include(u => u.Empresa)
                .Where(t=>t.UserType==UserType.Admin)
                .ToListAsync());
        }

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
                    ModelState.AddModelError(string.Empty, "Este correo ya está siendo usado por otro usuario.");
                    model.UserType = UserType.Admin;
                    model.Empresas = await _combosHelper.GetComboEmpresasAsync();
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
                    $"<h1>Shopping - Confirmación de cuenta</h1>" +
                    $"Para habilitar el usuario, por favor hacer clic en el siguiente enlace: " +
                    $"</hr></br><p><a href = \"{tokenLink}\">Confirmar Email</a></p>");
                if (response.IsSuccess)
                {
                    ViewBag.Message = "Las instrucciones para habilitar el Administrador han sido enviadas al correo.";
                    return View(model);
                }

                
                return RedirectToAction("Index", "Users");
            }
            model.EmpresaId = null;
            model.UserType = UserType.Admin;
            model.Empresas = await _combosHelper.GetComboEmpresasAsync();
            return View(model);
        }
    }
}