using BancoSENAIAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BancoSENAIAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class DocumentoController : Controller
    {
        private readonly string _caminhoRaiz = Path.Combine
            (Directory.GetCurrentDirectory()
            , "ClienteArquivos");

        private static List<Models.DocumentoMetadado> _documentoMetadado = new List<Models.DocumentoMetadado>();

        private static int _nextid = 1;

        [HttpPost("upload/{CodigoCliente}")]
        public async Task<IActionResult> AnexarArquivo(int CodigoCliente, IFormFile arquivo)
        {
            if (arquivo == null || arquivo.Length == 0)
            {
                return BadRequest("nenhum arquivo foi enviado");
            }

            string pastaCliente = Path.Combine(_caminhoRaiz, CodigoCliente.ToString());

            if (!Directory.Exists(pastaCliente))
            {
                Directory.CreateDirectory(pastaCliente);
            }

            string extensao = Path.Combine(arquivo.FileName);

            string nameOriginal = Path.GetFileNameWithoutExtension(arquivo.FileName);
            string novonome = $"{CodigoCliente}_{nameOriginal}_{Guid.NewGuid()}{extensao}";
            string caminhofinal = Path.Combine(pastaCliente, novonome);

            using (var stream = new FileStream(caminhofinal, FileMode.Create))
            {
                await arquivo.CopyToAsync(stream);
            }

            var documentosMetadados = new Models.DocumentoMetadado
            {
                Id = _nextid++,
                Nome = nameOriginal,
                Extensao = extensao,
                Caminho = caminhofinal,
                CodigoCliente = CodigoCliente
            };

            _documentoMetadado.Add(documentosMetadados);

            return Ok(new { mensagem = "Documento anexado com sucesso", arquivoSalvo = novonome });
        }

        [HttpGet("listar/{codigoCliente}")]
        public IActionResult ConsultarPorCodigo(int codigoCliente)
        {
            var documentos = _documentoMetadado.Where(d => d.CodigoCliente == codigoCliente).ToList();

            if (documentos == null)
                return NotFound(new { message = "Documento não encontrado." });

            return Ok(documentos);
        }

        [HttpGet("download/{id}")]
        public IActionResult DownloadArquivo(int id)
        {
            var documento = _documentoMetadado.FirstOrDefault(d => d.Id == id);

            if (documento == null)
            {
                return NotFound(new { mensagem = "Documento não encontrado." });
            }

            if (!System.IO.File.Exists(documento.Caminho))
            {
                return NotFound(new { mensagem = "O arquivo não foi encontrado no servidor." });
            }

            byte[] fileBytes = System.IO.File.ReadAllBytes(documento.Caminho);
            string nomeArquivo = $"{documento.Nome}{documento.Extensao}";

            return File(fileBytes, "application/octet-stream", nomeArquivo);
        }
        [HttpDelete("excluir/{id}")]
        public IActionResult Excluir(int id)
        {
            var documento = _documentoMetadado.FirstOrDefault(a => a.Id == id);

            if (documento == null)
            {
                return NotFound();
            }

            if (System.IO.File.Exists(documento.Caminho))
            {
                System.IO.File.Delete(documento.Caminho);
            }

            _documentoMetadado.Remove(documento);
            return Ok(new { message = "Documento excluído com sucesso." });
        }
    }
    
}



