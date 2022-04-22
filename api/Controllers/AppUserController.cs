using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AppUserController : ControllerBase
    {
        private readonly AppDbContext _context;
        public AppUserController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.AppUsers.ToListAsync();
            var user = _context.AppUsers.Find(-1);

            var b = 0;
            var a = 1 / b;

            return Ok(users);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _context.AppUsers.FirstOrDefaultAsync(u => u.Id == id);
            if(user == null) return NotFound();

            return Ok(user);
        }
    }
}
