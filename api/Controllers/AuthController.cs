using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.UserDtos;
using api.Interfaces;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ITokenService _token;
        public AuthController(AppDbContext context, ITokenService token)
        {
            _token = token;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto register)
        {
            if (await UserExists(register.UserName)) return BadRequest();

            //create secured password hash and salt for user and save user to database
            CreatePasswordHash(register.Password, out byte[] passwordSalt, out byte[] passwordHash);
            AppUser user = new AppUser {
                UserName = register.UserName.ToLower(),
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };

            await _context.AppUsers.AddAsync(user);
            await _context.SaveChangesAsync();

            return Ok(new UserDto {
                UserName = register.UserName,
                Token = _token.CreateToken(user)
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto login)
        {
            AppUser user = await _context.AppUsers.SingleOrDefaultAsync(u => u.UserName == login.UserName.ToLower());
            if (user == null) return Unauthorized("Invalid username");

            if(!VerifyPassword(login.Password, user.PasswordSalt, user.PasswordHash)) return BadRequest("Invalid Password");

            return Ok(new UserDto {
                UserName = login.UserName,
                Token = _token.CreateToken(user)
            });
        }

        public async Task<bool> UserExists(string username) 
        {
            return await _context.AppUsers.AnyAsync(u => u.UserName.ToLower() == username.ToLower());
        }

        private void CreatePasswordHash(string password, out byte[] passwordSalt, out byte[] passwordHash)
        {
            using var hmac = new HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        private bool VerifyPassword(string password, byte[] passwordSalt, byte[] passwordHash)
        {
            using var hmac = new HMACSHA512(passwordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != passwordHash[i]) return false;
            }
            return true;
        }
    }
}
