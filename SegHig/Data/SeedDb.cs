using SegHig.Data.Entities;
using SegHig.Enums;
using SegHig.Helpers;

namespace SegHig.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;

        public SeedDb(DataContext context, IUserHelper userHelper)
        {
            _context = context;
            _userHelper = userHelper;
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();
            await CheckClienteTiposAsync();
            await CheckEmpresaTiposAsync();
            await CheckRolesAsync();
            await CheckUserAsync("1010", "Luis", "Núñez", "luis@yopmail.com", "156 814 963", "Espora 2052", UserType.Admin,true);
            

        }

        private async Task CheckRolesAsync()
        {
            await _userHelper.CheckRoleAsync(UserType.Admin.ToString());
            await _userHelper.CheckRoleAsync(UserType.User.ToString());
        }

        private async Task<User> CheckUserAsync(
        string document,
        string firstName,
        string lastName,
        string email,
        string phone,
        string address,
        UserType userType,
        bool active)
        {
            User user = await _userHelper.GetUserAsync(email);
            if (user == null)
            {
                user = new User
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    UserName = email,
                    PhoneNumber = phone,
                    Address = address,
                    Document = document,
                    UserType = userType,
                    Active=active,
                };

                await _userHelper.AddUserAsync(user, "123456");
                await _userHelper.AddUserToRoleAsync(user, userType.ToString());

                string token = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                await _userHelper.ConfirmEmailAsync(user, token);

            }

            return user;
        }

        private async Task CheckClienteTiposAsync()
        {
            if (!_context.ClienteTipos.Any())
            {
                _context.ClienteTipos.Add(new ClienteTipo { Name = "Telefonía", Active=true });
                _context.ClienteTipos.Add(new ClienteTipo { Name = "Cable", Active = true });
                _context.ClienteTipos.Add(new ClienteTipo { Name = "Energía", Active = true });
            }

            await _context.SaveChangesAsync();
        }

        private async Task CheckEmpresaTiposAsync()
        {
            if (!_context.EmpresaTipos.Any())
            {
                   _context.EmpresaTipos.Add(new EmpresaTipo { Name = "Servicios", Active=true });
                   _context.EmpresaTipos.Add(new EmpresaTipo { Name = "Bienes", Active = true });
                   _context.EmpresaTipos.Add(new EmpresaTipo { Name = "Pymes", Active = true });
            }
            await _context.SaveChangesAsync();
        }
    }
}