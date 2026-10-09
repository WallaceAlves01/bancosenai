using BancoSENAIAPI.Data;
using BancoSENAIAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BancoSENAIAPI.Controllers
{
    [ApiController]
    [Route("api/v3/[controller]")]
    [Authorize]
    public class ClienteController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ClienteController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> ListarTodas()
        {
            var cliente = await _context.Cliente.ToListAsync();
            return Ok(cliente); ;
        }

        [HttpPost]
        public async Task<IActionResult> Cadastrar([FromBody] Cliente novoCliente)
        {

            if (await _context.Cliente.AnyAsync(a => a.CodigoCliente == novoCliente.CodigoCliente))
                return BadRequest(new { message = "Este codigo já é utilizado por um cliente" });

            _context.Cliente.Add(novoCliente);
            await _context.SaveChangesAsync();
            // Retorna Status 201 Created conforme boas práticas REST [6, 8]
            return Created("", novoCliente);
        }

        [HttpGet("{codigo}")]
        public async Task<IActionResult> ConsultarPorCodigo(int codigo)
        {
            var cliente = await _context.Cliente.FirstOrDefaultAsync(a => a.CodigoCliente == codigo);

            if (cliente == null)
                return NotFound(new { message = "Cliente não encontrado" }); // Status 404 [6, 7]

            return Ok(cliente); // Status 200 OK [6, 7]
        }

        [HttpPut("{codigo}")]
        public async Task<IActionResult> Alterar(int codigo, [FromBody] Cliente clienteAtualizado)
        {
            var clienteExistente = await _context.Cliente.FirstOrDefaultAsync(a => a.CodigoCliente == codigo);

            if (clienteExistente == null) return NotFound();

            clienteExistente.CodigoCliente = clienteAtualizado.CodigoCliente;
            clienteExistente.NomeCliente = clienteAtualizado.NomeCliente;
            clienteExistente.CPF = clienteAtualizado.CPF;
            clienteExistente.NumeroAgencia = clienteAtualizado.NumeroAgencia;
            clienteExistente.SaldoTotal = clienteAtualizado.SaldoTotal;

            await _context.SaveChangesAsync();
            // Retorna Status 204 No Content para atualizações bem-sucedidas [6, 9]
            return NoContent();
        }

        [HttpDelete("{codigo}")]
        public async Task<IActionResult> Excluir(int codigo)
        {
            var cliente = await _context.Cliente.FirstOrDefaultAsync(a => a.CodigoCliente == codigo);

            if (cliente == null) return NotFound();

            _context.Cliente.Remove(cliente);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Cliente excluído com sucesso." }); // Status 200 [6]
        }
    }
}