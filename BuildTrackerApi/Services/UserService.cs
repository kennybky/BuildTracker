using BuildTrackerApi.Helpers;
using BuildTrackerApi.Models;
using BuildTrackerApi.Models.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BuildTrackerApi.Services
{
    public interface IUserService
    {
        Task<User> Authenticate(string username, string password);

        Task<IEnumerable<Claim>> GetUserClaims(User user);
        IEnumerable<User> GetAll();
        User GetById(int? id);

        Task<User> GetByUserName(string UserName);
        Task<User> Create(User user, string password);
        Task<User> Update(User user);
        User Delete(int id);
        Task<User> AddRole(User userParam, Role role);

        Task<bool> IsInRole(User user, Role role);

     
        Task<bool> ChangePassword(User userParam, string oldPassword, string newPassword);

        Task ConfirmAccount(User user, string password);
    }

    public class UserService : IUserService
    {
        private BuildTrackerContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        public UserService(BuildTrackerContext context, UserManager<User> userManager, RoleManager<AppRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<User> Authenticate(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return null;

            var user = _context.Users.SingleOrDefault(x => x.UserName == username);

            // check if username exists
            if (user == null)
                return null;

            if(await _userManager.IsLockedOutAsync(user))
            {
                throw new AppException("This account is temporarily locked out", HttpStatusCode.Forbidden);
            }
    
            var result = await _userManager.CheckPasswordAsync(user, password);

            if(result)
            {
                return user;
            } else
            {
                await _userManager.AccessFailedAsync(user);
                return null;
            }
           
            // check if password is correct
            //if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            //    return null;

            // authentication successful
        }

        Task<bool> IUserService.IsInRole(User user, Role role)
        {
            return _userManager.IsInRoleAsync(user, role.ToString());
        }

        public IEnumerable<User> GetAll()
        {
            return _context.Users;
        }

        async Task<IEnumerable<Claim>> IUserService.GetUserClaims(User user)
        {
            IList<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Id.ToString()),
                new Claim(ClaimTypes.GivenName, user.FirstName),
                new Claim(ClaimTypes.Surname, user.LastName)
            };
            IList<Claim> userClaims = await _userManager.GetClaimsAsync(user);
            foreach (var claim in userClaims)
            {
                claims.Add(claim);
            }
            IEnumerable<string> userRoles = await _userManager.GetRolesAsync(user);
            bool supportsRoleClaim = _roleManager.SupportsRoleClaims;
            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
                if (supportsRoleClaim)
                {
                    IList<Claim> roleClaims = await _roleManager.GetClaimsAsync(new AppRole(role));
                    foreach (var claim in roleClaims)
                    {
                        claims.Add(claim);
                    }
                }
            }
            claims.Add(new Claim(CustomClaimTypes.AccountConfirmed, user.AccountConfirmed.ToString()));
            return claims;
        }

        public User GetById(int? id)
        {
            if(id== null)
            {
                return null;
            }
            return _context.Users.Find(id);
        }

        public async Task<User> GetByUserName(string UserName)
        {
            return await _userManager.FindByNameAsync(UserName);
        }

        public async Task<User> Create(User user, string password)
        {
            // validation
            if (string.IsNullOrWhiteSpace(password))
                throw new AppException("Password is required");

            if (_context.Users.Any(x => x.UserName == user.UserName))
                throw new AppException("Username \"" + user.UserName + "\" is already taken");

           var result =  await  _userManager.CreateAsync(user, password);

            //byte[] passwordHash, passwordSalt;
            //CreatePasswordHash(password, out passwordHash, out passwordSalt);

            //user.PasswordHash = passwordHash;
            //user.PasswordSalt = passwordSalt;

            //_context.Users.Add(user);
            // _context.SaveChanges();

            if (result.Succeeded)
            {
                return user;
            } else
            {
                string message = "";
                foreach (var error in result.Errors)
                {
                    message += error.Description + "\n";
                }
                throw new AppException(message.Trim(), HttpStatusCode.BadRequest);
            }
        }

        public async Task<User> Update(User userParam)
        {
            var user = _context.Users.Find(userParam.Id);

            if (user == null)
                throw new AppException("User not found");

            if (userParam.UserName != user.UserName)
            {
                // username has changed so check if the new username is already taken
                if (_context.Users.Any(x => x.UserName == userParam.UserName))
                    throw new AppException("Username " + userParam.UserName + " is already taken");
            }

            // update user properties
            user.FirstName = userParam.FirstName;
            user.LastName = userParam.LastName;
            user.UserName = userParam.UserName;
            user.Email = userParam.Email;
            user.PhoneNumber = userParam.PhoneNumber;


            await _userManager.UpdateAsync(user);
            return user;
        }

        public async Task ConfirmAccount(User user, string password)
        {

            if (user == null)
                throw new AppException("User not found");

            else if(user.AccountConfirmed)
                throw new AppException("Account Already Confirmed", HttpStatusCode.Forbidden);


            string token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var result = await _userManager.ResetPasswordAsync(user, token, password);

            if (result.Succeeded) {
              user.AccountConfirmed = true;
                result = await _userManager.UpdateAsync(user);
            }
            if (!result.Succeeded)
            {
                throw new AppException("There was an error completing this request", HttpStatusCode.BadRequest);
            }
        }


        public async Task<User> AddRole(User userParam, Role role)
        {
            var user = _context.Users.Find(userParam.Id);

            if (user == null)
                throw new AppException("User not found", HttpStatusCode.NotFound);


            var result = await _userManager.AddToRoleAsync(user, role.ToString());

            if (!result.Succeeded)
            {
                throw new AppException("Can't update Role");
            }
            //TODO: implement role
            //user.Role = role;

            await _userManager.UpdateAsync(user);
            return user;
        }

        public async Task<bool> ChangePassword(User userParam, string oldPassword, string newPassword)
        {
            var user = _context.Users.Find(userParam.Id);

            if (user == null)
                throw new AppException("User not found");

            var valid = await _userManager.CheckPasswordAsync(user, oldPassword);
            if(valid)
            {
                var result = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
                return result.Succeeded;
            }
            return false;
        }

       

        public User Delete(int id)
        {
            var user = _context.Users.Find(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
            return user;
        }

        // private helper methods

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");

            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
            if (storedHash.Length != 64) throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
            if (storedSalt.Length != 128) throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");

            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }

            return true;
        }

    }
}
