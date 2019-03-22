using BuildTrackerApi.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace BuildTrackerApi.Services
{
    public interface IUserService
    {
        Task<User> Authenticate(string username, string password);
        IEnumerable<User> GetAll();
        User GetById(int id);

        Task<User> GetByUserName(string UserName);
        Task<User> Create(User user, string password);
        void Update(User user, string password = null);
        void Delete(int id);
        void ChangeRole(User userParam, Role role);

        Task<bool> ChangePassword(User userParam, string oldPassword, string newPassword);
    }

    public class UserService : IUserService
    {
        private BuildTrackerContext _context;
        private readonly UserManager<User> _userManager;

        public UserService(BuildTrackerContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
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

        public IEnumerable<User> GetAll()
        {
            return _context.Users;
        }

        public User GetById(int id)
        {
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

        public void Update(User userParam, string password = null)
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

            
            _userManager.UpdateAsync(user).Wait();
          
        }

        public void ChangeRole(User userParam, Role role)
        {
            var user = _context.Users.Find(userParam.Id);

            if (user == null)
                throw new AppException("User not found");

            user.Role = role;

            _userManager.UpdateAsync(user).Wait();
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

       

        public void Delete(int id)
        {
            var user = _context.Users.Find(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
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
