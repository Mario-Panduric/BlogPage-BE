using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication5.Data;
using WebApplication5.Entities;
using WebApplication5.DTOs;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using WebApplication5.Authorization;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using WebApplication5.Validators;

namespace WebApplication5.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly PasswordHasher<User> _passwordHasher;
        private readonly TokenGenerator _tokenGenerator;
        private readonly RegisterUserDtoValidator _registerUserValidator;

        public UsersController(DataContext context, IMapper mapper, PasswordHasher<User> passwordHasher, TokenGenerator tokenGenerator, RegisterUserDtoValidator registerUserValidator)
        {
            _context = context;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            _tokenGenerator = tokenGenerator;
            _registerUserValidator = registerUserValidator;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            var users = await _context.Users.Where(c => c.isActive).ToListAsync();
            return this.Ok(this._mapper.Map<List<UserDto>>(users));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(c => c.Id == id && c.isActive);

            if (user == null)
            {
                return NotFound();
            }

            return _mapper.Map<UserDto>(user);
        }
        [HttpPut]
        public async Task<IActionResult> EditUser(int id, UserDto updatedUser)
        {
            var user = await _context.Users.FirstOrDefaultAsync(c => c.Id == id && c.isActive);

            if (user == null)
            {
                return NotFound();
            }

            _mapper.Map(updatedUser, user);
            user.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost]
        [Route("Register")]
        public async Task<ActionResult<User>> RegisterUser(RegisterUserDto userData)
        {
            var result = _registerUserValidator.Validate(userData);
            if (result.IsValid)
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == userData.UserName);

                if (user != null)
                {
                    return Conflict("Username already exist.");
                }

                user = await _context.Users.FirstOrDefaultAsync(x => x.Email == userData.Email);

                if (user != null)
                {
                    return Conflict("Email already exist.");
                }
                var newUser = new User()
                {
                    UserName = userData.UserName,
                    Email = userData.Email,
                    UserPassword = userData.UserPassword
                };
                newUser.UserPassword = _passwordHasher.HashPassword(newUser, newUser.UserPassword);
                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();
                return CreatedAtAction("GetUser", new { id = newUser.Id }, newUser);
            }
            else
            {
                List<string> errors = new List<string>();
                foreach (var error in result.Errors)
                {
                    errors.Add(error.ToString());
                }
                return BadRequest(errors);
            }
            

        }

        [HttpPost]
        [Route("Login")]
        public async Task<ActionResult<User>> LoginUser(LoginUserDto credentials)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == credentials.UserName && x.isActive);
            
            if (user != null)
            {
                var passwordResult = _passwordHasher.VerifyHashedPassword(user, user.UserPassword, credentials.UserPassword);
                if (passwordResult == PasswordVerificationResult.Success)
                {
                    var token = _tokenGenerator.Create(user.Id, user.UserName, user.Email);
                    HttpContext.Response.Cookies.Append("token", token,
                        new CookieOptions
                        {
                            Expires = DateTime.Now.AddHours(10),
                            HttpOnly = true,
                            Secure = true,
                            IsEssential = true,
                            SameSite = SameSiteMode.None,
                        });
                    return Ok( new { token });
                }
            }

            return NoContent();
        }

        [HttpGet("getLoggedUser")]
        public IActionResult VerifyToken()
        {
            if (Request.Cookies.TryGetValue("token", out var token))
            {
                var handler = new JwtSecurityTokenHandler();
                try
                {
                    var jwtToken = handler.ReadJwtToken(token);

                    if (jwtToken.ValidTo < DateTime.UtcNow)
                    {
                        return Unauthorized(new { authenticated = false, message = "Token has expired" });
                    }

                    return Ok(jwtToken.Claims.Take(3).ToList());
                }
                catch
                {
                    return Unauthorized(new { authenticated = false, message = "Invalid token" });
                }
            }

            return Unauthorized(new { authenticated = false, message = "Token not found" });
        }

        [HttpPost]
        [Route("Logout")]
        [Authorize]
        public IActionResult LogoutUser()
        {
            HttpContext.Response.Cookies.Delete("token",
                        new CookieOptions
                        {
                            Expires = DateTime.Now.AddMinutes(-10),
                            HttpOnly = true,
                            Secure = true,
                            IsEssential = true,
                            SameSite = SameSiteMode.None,
                        });
            return NoContent();
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(c => c.Id == id && c.isActive);
            if (user == null)
            {
                return NotFound();
            }

            user.isActive = false;
            user.DeletedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
