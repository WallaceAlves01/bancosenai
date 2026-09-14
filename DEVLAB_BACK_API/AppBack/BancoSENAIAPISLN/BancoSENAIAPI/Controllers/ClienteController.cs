using BancoSENAIAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace BancoSENAIAPI.Controllers
{
    [ApiController]
    [Route("api/v3/[controller]")]
    public class ClienteController : ControllerBase
    {
        private static List<Cliente> _clientes = new List<Cliente>
        {
            new Cliente { CodigoCliente = 101, NomeCliente = "Douglas", CPF = 12827351, NumeroAgencia = 2001, SaldoTotal = 0,
                        Sexo = "Masculino", Endereco = "Rua de Deus", Cidade = "Xique-Xique", Estado = "Bahia" },

            new Cliente { CodigoCliente = 102, NomeCliente = "Jhon", CPF = 73557223, NumeroAgencia = 3001, SaldoTotal = 1,
                        Sexo = "Masculino", Endereco = "Highway to hell", Cidade = "Columbus", Estado = "Ohio" },
        };

        [HttpGet]
        public IActionResult ListarTodas()
        {
            return Ok(_clientes);
        }

        [HttpPost]
        public IActionResult Cadastrar([FromBody] Cliente novoCliente)
        {

            if (_clientes.Any(a => a.CodigoCliente == novoCliente.CodigoCliente))
                return BadRequest(new { message = "Este codigo já é utilizado por um cliente" });

            _clientes.Add(novoCliente);
            // Retorna Status 201 Created conforme boas práticas REST [6, 8]
            return Created("", novoCliente);
        }

        [HttpGet("{codigo}")]
        public IActionResult ConsultarPorCodigo(int codigo)
        {
            var cliente = _clientes.FirstOrDefault(a => a.CodigoCliente == codigo);

            if (cliente == null)
                return NotFound(new { message = "Cliente não encontrado" }); // Status 404 [6, 7]

            return Ok(cliente); // Status 200 OK [6, 7]
        }

        [HttpPut("{codigo}")]
        public IActionResult Alterar(int codigo, [FromBody] Cliente clienteAtualizado)
        {
            var clienteExistente = _clientes.FirstOrDefault(a => a.CodigoCliente == codigo);

            if (clienteExistente == null) return NotFound();

            clienteExistente.CodigoCliente = clienteAtualizado.CodigoCliente;
            clienteExistente.NomeCliente = clienteAtualizado.NomeCliente;
            clienteExistente.CPF = clienteAtualizado.CPF;
            clienteExistente.NumeroAgencia = clienteAtualizado.NumeroAgencia;
            clienteExistente.SaldoTotal = clienteAtualizado.SaldoTotal;
            clienteExistente.Sexo = clienteAtualizado.Sexo;
            clienteExistente.Endereco = clienteAtualizado.Endereco;
            clienteExistente.Cidade = clienteAtualizado.Cidade;
            clienteExistente.Estado = clienteAtualizado.Estado;

            // Retorna Status 204 No Content para atualizações bem-sucedidas [6, 9]
            return NoContent();
        }

        [HttpDelete("{codigo}")]
        public IActionResult Excluir(int codigo)
        {
            var cliente = _clientes.FirstOrDefault(a => a.CodigoCliente == codigo);

            if (cliente == null) return NotFound();

            _clientes.Remove(cliente);
            return Ok(new { message = "Cliente excluído com sucesso." }); // Status 200 [6]
        }
    }
}