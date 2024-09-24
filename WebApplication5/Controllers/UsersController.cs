using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication5.Data;
using WebApplication5.Entities;
using WebApplication5.DTOs;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace WebApplication5.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly PasswordHasher<User> _passwordHasher;

        public UsersController(DataContext context, IMapper mapper, PasswordHasher<User> passwordHasher)
        {
            _context = context;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
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
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == credentials.UserName);
            
            if (user != null)
            {
                var passwordResult = _passwordHasher.VerifyHashedPassword(user, user.UserPassword, credentials.UserPassword);
                if (passwordResult == PasswordVerificationResult.Success)
                {          
                    return Ok(_mapper.Map<User>(user));
                }
            }

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
