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

        public UsersController(DataContext context, IMapper mapper, PasswordHasher<User> passwordHasher, TokenGenerator tokenGenerator)
        {
            _context = context;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            _tokenGenerator = tokenGenerator;
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

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
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

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Route("Register")]
        public async Task<ActionResult<User>> RegisterUser(User user)
        {
            user.UserPassword = _passwordHasher.HashPassword(user, user.UserPassword);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
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
                    var token = _tokenGenerator.Create(user.Id);
                    HttpContext.Response.Cookies.Append("token", token,
                        new CookieOptions
                        {
                            Expires = DateTime.Now.AddMinutes(10),
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

        [HttpGet("verify")]
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

                    return Ok(new { authenticated = true });
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
            Response.Cookies.Delete("token");
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
