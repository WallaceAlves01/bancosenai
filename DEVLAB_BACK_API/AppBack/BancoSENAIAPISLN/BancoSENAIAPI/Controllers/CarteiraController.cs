using BancoSENAIAPI.Models;
using Microsoft.AspNetCore.Mvc;
using BancoSENAIAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace BancoSENAIAPI.Controllers
{
    [ApiController]
    [Route("api/v2/[controller]")]
    [Authorize]
    public class CarteiraController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CarteiraController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> ListarTodas()
        {
            var carteiras = await _context.Carteira.ToListAsync();
            return Ok(carteiras);
        }

        [HttpPost]
        public async Task<IActionResult> Cadastrar([FromBody] Carteira novaCarteira)
        {

            if (await _context.Carteira.AnyAsync(a => a.NumeroCarteira == novaCarteira.NumeroCarteira))
                return BadRequest(new { message = "Este número de carteira já existe." });
            if (novaCarteira.ApetiteCarteira < 0)
                return BadRequest(new { message = "O valor do apetitecarteira deve ser maior ou igual a zero" });

            _context.Carteira.Add(novaCarteira);

            await _context.SaveChangesAsync();
            // Retorna Status 201 Created conforme boas práticas REST [6, 8]
            return Created("", novaCarteira);
        }

        [HttpGet("{codigo}")]
        public async Task<IActionResult> ConsultarPorCodigo(int codigo)
        {
            var carteira = await _context.Carteira.FirstOrDefaultAsync(a => a.NumeroCarteira == codigo);

            if (carteira == null)
                return NotFound(new { message = "Carteira não encontrada." }); // Status 404 [6, 7]

            return Ok(carteira); // Status 200 OK [6, 7]
        }

        [HttpPut("{codigo}")]
        public async Task<IActionResult> Alterar(int codigo, [FromBody] Carteira carteiraAtualizada)
        {
            var carteiraExistente = await _context.Carteira.FirstOrDefaultAsync(a => a.NumeroCarteira == codigo);

            if (carteiraExistente == null) return NotFound();

            carteiraExistente.NomeCarteira = carteiraAtualizada.NomeCarteira;
            carteiraExistente.ApetiteCarteira = carteiraAtualizada.ApetiteCarteira;

            await _context.SaveChangesAsync();

            // Retorna Status 204 No Content para atualizações bem-sucedidas [6, 9]
            return NoContent();
        }

        [HttpDelete("{codigo}")]
        public async Task<IActionResult> Excluir(int codigo)
        {
            var carteira = await _context.Carteira.FirstOrDefaultAsync(a => a.NumeroCarteira == codigo);

            if (carteira == null) return NotFound();

            _context.Carteira.Remove(carteira);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Carteira excluída com sucesso." }); // Status 200 [6]
        }
    }
}