using Microsoft.AspNetCore.Identity;
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

namespace SegHig.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly DataContext _context;
        private readonly ICombosHelper _combosHelper;
        private readonly IMailHelper _mailHelper;
        private readonly IFlashMessage _flashMessage;

        public AccountController(IUserHelper userHelper, DataContext context, ICombosHelper combosHelper, IMailHelper mailHelper ,IFlashMessage flashMessage)
        {
            _userHelper = userHelper;
            _context = context;
            _combosHelper = combosHelper;
            _mailHelper = mailHelper;
            _flashMessage = flashMessage;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Users
                .Include(u => u.Empresa)
                .Where(t => t.UserType == UserType.User)
                .ToListAsync());
        }

        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(new LoginViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _userHelper.GetUserAsync(model.Username);

                if (user.Active == false)
                {
                    {
                        _flashMessage.Danger("Este usuario no está activo.");
                        return RedirectToAction("Login", "Account");
                    }
                }


                Microsoft.AspNetCore.Identity.SignInResult result = await _userHelper.LoginAsync(model);
                
                
                
                
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }

                if (result.IsLockedOut)
                {
                    _flashMessage.Danger( "Ha superado el máximo número de intentos, su cuenta está bloqueada, intente de nuevo en 5 minutos.");
                }
                else if (result.IsNotAllowed)
                {
                    _flashMessage.Danger( "El usuario no ha sido habilitado, debes de seguir las instrucciones del correo enviado para poder habilitar el usuario.");
                }
                else
                {
                    _flashMessage.Danger( "Email o contraseña incorrectos.");
                }

            }

            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _userHelper.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult NotAuthorized()
        {
            return View();
        }

        [NoDirectAccess]
        public async Task<IActionResult> Register()
        {
            AddUserViewModel model = new AddUserViewModel
            {
                Id = "1",
                Empresas = await _combosHelper.GetComboEmpresasAsync(),
                UserType = UserType.User,
                Active=true,
            };
            return View(model);
        }


        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(AddUserViewModel model)
        {
            model.Active = true;
            if (ModelState.IsValid)
            {
                
                User user = await _userHelper.AddUserAsync(model);
                if (user == null)
                {
                    _flashMessage.Danger( "Este correo ya está siendo usado por otro usuario.");
                    model.Empresas = await _combosHelper.GetComboEmpresasAsync();
                    model.UserType = UserType.User;
                    model.Active = true;
                    return View(model);
                };
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
                    _flashMessage.Info( "Las instrucciones para habilitar su cuenta han sido enviadas al correo.");
                    return Json(new
                    {
                        isValid = true,
                        html = ModalHelper.RenderRazorViewToString(this, "_ViewAll", _context.Users
               .Where(c => c.UserType == UserType.Admin)
               .ToList())
                    });
                }
                _flashMessage.Danger( response.Message);
            }
            model.EmpresaId = null;
            model.UserType = UserType.Admin;
            model.Empresas = await _combosHelper.GetComboEmpresasAsync();
            return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "Register", model) });
        }

        [NoDirectAccess]
        public async Task<IActionResult> ChangeUser()
        {
            User user = await _userHelper.GetUserAsync(User.Identity.Name);
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
                EmpresaId = user.Empresa.Id,
                Id = user.Id,
                Document = user.Document,
                Active=user.Active
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeUser(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _userHelper.GetUserAsync(User.Identity.Name);
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Address = model.Address;
                user.PhoneNumber = model.PhoneNumber;
                user.Empresa = await _context.Empresas.FindAsync(model.EmpresaId);
                user.Document = model.Document;
                user.Active = model.Active;

                await _userHelper.UpdateUserAsync(user);
                _flashMessage.Info("Usuario actualizado.");
                return Json(new
                {
                    isValid = true,
                    html = ModalHelper.RenderRazorViewToString(this, "_ViewAll", _context.Users
                    .ToList())
                });
            }

            model.Empresas = await _combosHelper.GetComboEmpresasAsync();
            return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "ChangeUser", model) });
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
                EmpresaId = user.Empresa.Id,
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
                user.Empresa = await _context.Empresas.FindAsync(model.EmpresaId);
                user.Document = model.Document;
                user.Active = model.Active;

                await _userHelper.UpdateUserAsync(user);
                _flashMessage.Info("Usuario actualizado.");
                return Json(new
                {
                    isValid = true,
                    html = ModalHelper.RenderRazorViewToString(this, "_ViewAll", _context.Users
              .Where(c => c.UserType == UserType.Admin)
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
                return RedirectToAction("Index", "Account");
        }

        public async Task<IActionResult> DeleteUser(string id)
        {
            User user = await _context.Users
                .FirstOrDefaultAsync(p => p.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            _flashMessage.Info("Registro borrado.");
            return RedirectToAction(nameof(Index));
        }



        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {

                if (model.OldPassword == model.NewPassword)
                {
                    _flashMessage.Danger( "Debes ingresar una contraseña diferente a la actual");
                    return View(model);
                }

                var user = await _userHelper.GetUserAsync(User.Identity.Name);
                if (user != null)
                {
                    var result = await _userHelper.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("ChangeUser");
                    }
                    else
                    {
                        _flashMessage.Danger( "Password incorrecto");
                    }
                }
                else
                {
                    _flashMessage.Danger( "User no found.");
                }
            }

            return View(model);
        }
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return NotFound();
            }

            User user = await _userHelper.GetUserByIdAsync(new String(userId));
            if (user == null)
            {
                return NotFound();
            }

            IdentityResult result = await _userHelper.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                return NotFound();
            }
            return View();
        }
        public IActionResult RecoverPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RecoverPassword(RecoverPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _userHelper.GetUserAsync(model.Email);
                if (user == null)
                {
                    _flashMessage.Danger( "El email no corresponde a ningún usuario registrado.");
                    return View(model);
                }

                string myToken = await _userHelper.GeneratePasswordResetTokenAsync(user);
                string link = Url.Action(
                    "ResetPassword",
                    "Account",
                    new { token = myToken }, protocol: HttpContext.Request.Scheme);
                _mailHelper.SendMail(
                    $"{user.FullName}",
                    model.Email,
                    "SegHig - Recuperación de Contraseña",
                    $"<h1>SegHig - Recuperación de Contraseña</h1>" +
                    $"Para recuperar la contraseña haga clic en el siguiente enlace:" +
                    $"<p><a href = \"{link}\">Reset Password</a></p>");
                _flashMessage.Info( "Las instrucciones para recuperar la contraseña han sido enviadas a su correo.");
                return View();
            }

            return View(model);
        }

        public IActionResult ResetPassword(string token)
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            User user = await _userHelper.GetUserAsync(model.UserName);
            if (user != null)
            {
                IdentityResult result = await _userHelper.ResetPasswordAsync(user, model.Token, model.Password);
                if (result.Succeeded)
                {
                    _flashMessage.Info( "Contraseña cambiada con éxito.");
                    return View();
                }

                _flashMessage.Info( "Error cambiando la contraseña.");
                return View(model);
            }

            _flashMessage.Info( "Usuario no encontrado.");
            return View(model);
        }

    }
}