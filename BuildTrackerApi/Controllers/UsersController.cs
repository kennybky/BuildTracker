using AutoMapper;
using BuildTrackerApi.Helpers;
using BuildTrackerApi.Models;
using BuildTrackerApi.Services;
using BuildTrackerApi.Services.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BuildTrackerApi.Models.Dtos;

namespace BuildTrackerApi.Controllers
{
    [Authorize]
    [DenyNotConfirmed]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private IUserService _userService;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;
        private readonly IAuthorizationService _authorizationService;

        public UsersController(
            IUserService userService,
            IMapper mapper,
            IOptions<AppSettings> appSettings,
            IAuthorizationService authorizationService)
        {
            _userService = userService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
            _authorizationService = authorizationService;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody]UserDto userDto)
        {
            var user = await _userService.Authenticate(userDto.UserName, userDto.Password);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role.ToString()),
                    new Claim("AccountConfirmed", user.AccountConfirmed.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            userDto = _mapper.Map<UserDto>(user);
            // return basic user info (without password) and token to store client side
            return Ok(new
            {
              User = userDto,
                Token = tokenString,
                Expires = DateTime.UtcNow.AddDays(7)
            });
        }

       
        [Authorize(Roles = "ADMIN")]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]UserDto userDto)
        {
            // map dto to entity
            var user = _mapper.Map<User>(userDto);

            try
            {
                // save 
              await _userService.Create(user, userDto.Password);
                return Ok();
            }
            catch (Exception ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }
        [AllowNotConfirmed]
        [HttpGet("self")]
        public IActionResult Self()
        {
            var id = int.Parse(HttpContext.User.Identity.Name);
            var user = _userService.GetById(id);
            return Ok(_mapper.Map<UserDto>(user));
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var users = _userService.GetAll();
            var userDtos = _mapper.Map<IList<UserDto>>(users);
            return Ok(userDtos);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var user = _userService.GetById(id);
            var userDto = _mapper.Map<UserDto>(user);
            return Ok(userDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody]UserDto userDto)
        {
            // map dto to entity and set id
            var user = _mapper.Map<User>(userDto);
            user.Id = id;
            var allowed = await _authorizationService.AuthorizeAsync(HttpContext.User, user, Operations.Update);
            if (!allowed.Succeeded)
            {
                return Forbid();
            }

            try
            {
                // save 
               await  _userService.Update(user);
                return Ok();
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("password/{id}")]
        public async Task<IActionResult> ChangePassword(int id, [FromForm]string oldPassword, [FromForm]string newPassword)
        {
           
            var user = _userService.GetById(id);
            if(user == null)
            {
                return NotFound(new { message = "User Not found" });
            }

            if (HttpContext.User.Identity.Name != id.ToString())
            {
                return Forbid();
            }

            try
            {
                // save 
               await _userService.ChangePassword(user, oldPassword, newPassword);
                return Ok();
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [AllowNotConfirmed]
        [HttpPut("confirm/{id}")]
        public async Task<IActionResult> ConfirmAccount(int id, [FromBody]UserDto userDto)
        {
           
            // map dto to entity and set id
            var user = _mapper.Map<User>(userDto);
            user.Id = id;
            var allowed = await _authorizationService.AuthorizeAsync(HttpContext.User, user, Operations.Update);
            if (!allowed.Succeeded)
            {
                return Forbid();
            }
            try
            {
                // Update and track 
               user = await _userService.Update(user);
               await _userService.ConfirmAccount(user, userDto.Password);
                return Ok();
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "ADMIN, PROJECT_MANAGER, DEVELOPER")]
        [HttpPut("role/{id}")]
        public IActionResult ChangeRole(int id, [FromBody] Role role)
        {
            // map dto to entity and set id
            var user = _userService.GetById(id);
            var requester = _userService.GetById(int.Parse(HttpContext.User.Identity.Name));
            if (user == null || requester == null)
            {
                return NotFound(new { message = "User Not found" });
            }

            var allowed = (user.Role < requester.Role) && (role <= requester.Role);

            if(!allowed)
            {
                return Forbid();
            }

            try
            {
                // save 
                _userService.ChangeRole(user, role);
                return Ok();
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")]
        public IActionResult Delete(int id)
        {
            _userService.Delete(id);
            return Ok();
        }
    }
}
