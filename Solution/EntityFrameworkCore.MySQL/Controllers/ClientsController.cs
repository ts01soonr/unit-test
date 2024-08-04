using EntityFrameworkCore.MySQL.Data;
using EntityFrameworkCore.MySQL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.MySQL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        public ClientsController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        [HttpPost]
        public async Task<IActionResult> AddClient(Client client)
        {
            _appDbContext.Clients.Add(client);
            await _appDbContext.SaveChangesAsync();

            return Ok(client);
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var clients = await _appDbContext.Clients.ToListAsync();
            return Ok(clients);
        }
    }
}
