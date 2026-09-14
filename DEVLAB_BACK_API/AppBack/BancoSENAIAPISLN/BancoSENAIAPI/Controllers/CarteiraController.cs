using BancoSENAIAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace BancoSENAIAPI.Controllers
{
    [ApiController]
    [Route("api/v2/[controller]")]
    public class CarteiraController : ControllerBase
    {
        private static List<Carteira> _carteiras = new List<Carteira>
        {
            new Carteira { NumeroCarteira = 1, NomeCarteira = "DuduCard", ApetiteCarteira = 1000000 },
            new Carteira { NumeroCarteira = 2, NomeCarteira = "NicoCard", ApetiteCarteira = 1100000  },
            new Carteira { NumeroCarteira = 4, NomeCarteira = "WallCard", ApetiteCarteira = 1111000  }
        };

        [HttpGet]
        public IActionResult ListarTodas()
        {
            return Ok(_carteiras);
        }

        [HttpPost]
        public IActionResult Cadastrar([FromBody] Carteira novaCarteira)
        {

            if (_carteiras.Any(a => a.NumeroCarteira == novaCarteira.NumeroCarteira))
                return BadRequest(new { message = "Este número de carteira já existe." });
            if (novaCarteira.ApetiteCarteira < 0)
                return BadRequest(new { message = "O valor do apetitecarteira deve ser maior ou igual a zero" });

            _carteiras.Add(novaCarteira);
            // Retorna Status 201 Created conforme boas práticas REST [6, 8]
            return Created("", novaCarteira);
        }

        [HttpGet("{codigo}")]
        public IActionResult ConsultarPorCodigo(int codigo)
        {
            var carteira = _carteiras.FirstOrDefault(a => a.NumeroCarteira == codigo);

            if (carteira == null)
                return NotFound(new { message = "Carteira não encontrada." }); // Status 404 [6, 7]

            return Ok(carteira); // Status 200 OK [6, 7]
        }

        [HttpPut("{codigo}")]
        public IActionResult Alterar(int codigo, [FromBody] Carteira carteiraAtualizada)
        {
            var carteiraExistente = _carteiras.FirstOrDefault(a => a.NumeroCarteira == codigo);

            if (carteiraExistente == null) return NotFound();

            carteiraExistente.NomeCarteira = carteiraAtualizada.NomeCarteira;
            carteiraExistente.ApetiteCarteira = carteiraAtualizada.ApetiteCarteira;

            // Retorna Status 204 No Content para atualizações bem-sucedidas [6, 9]
            return NoContent();
        }

        [HttpDelete("{codigo}")]
        public IActionResult Excluir(int codigo)
        {
            var carteira = _carteiras.FirstOrDefault(a => a.NumeroCarteira == codigo);

            if (carteira == null) return NotFound();

            _carteiras.Remove(carteira);
            return Ok(new { message = "Carteira excluída com sucesso." }); // Status 200 [6]
        }
    }
}