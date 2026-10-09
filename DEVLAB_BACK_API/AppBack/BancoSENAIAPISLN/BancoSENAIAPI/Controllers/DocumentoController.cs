using BancoSENAIAPI.Data;
using BancoSENAIAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BancoSENAIAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class DocumentoController : Controller
    {
        private readonly AppDbContext _context;

        public DocumentoController(AppDbContext context)
        {
            _context = context;
        }

        private readonly string _caminhoRaiz = Path.Combine
            (Directory.GetCurrentDirectory()
            , "ClienteArquivos");

        [HttpPost("upload/{CodigoCliente}")]
        public async Task<IActionResult> AnexarArquivo(int CodigoCliente, IFormFile arquivo)
        {

            if (arquivo == null || arquivo.Length == 0)
            {
                return BadRequest("nenhum arquivo foi enviado");
            }
            const long tamanhoBytes = 2 * (1024 * 1024);
            if (arquivo.Length > tamanhoBytes)
            {
                return BadRequest(new { mensagem = "O arquivo excede o limite permitido de 2 MB." });
            }

            string extensao = Path.GetExtension(arquivo.FileName).ToLowerInvariant();

            var extensoesPerm = new[] { ".pdf", ".jpg", ".png" };
            if (!extensoesPerm.Contains(extensao))
            {
                return BadRequest(new { mensagem = $"Extemsão {extensao} inválida. Apenas arquivos .pdf, .jpg e .png são permitidos." });
            }

            string pastaCliente = Path.Combine(_caminhoRaiz, CodigoCliente.ToString());

            if (!Directory.Exists(pastaCliente))
            {
                Directory.CreateDirectory(pastaCliente);
            }

            string nameOriginal = Path.GetFileNameWithoutExtension(arquivo.FileName);
            string novonome = $"{CodigoCliente}_{nameOriginal}_{Guid.NewGuid()}{extensao}";
            string caminhofinal = Path.Combine(pastaCliente, novonome);

            using (var stream = new FileStream(caminhofinal, FileMode.Create))
            {
                await arquivo.CopyToAsync(stream);
            }

            var documentosMetadados = new Models.DocumentoMetadado
            {
                Nome = nameOriginal,
                Extensao = extensao,
                Caminho = caminhofinal,
                CodigoCliente = CodigoCliente
            };

            _context.DocumentoMetadado.Add(documentosMetadados);
            await _context.SaveChangesAsync();

            return Ok(new { mensagem = "Documento anexado com sucesso", arquivoSalvo = novonome });
        }

        [HttpGet("listar/{codigoCliente}")]
        public async Task<IActionResult> ConsultarPorCodigo(int codigoCliente)
        {
            var documentos = await _context.DocumentoMetadado.Where(d => d.CodigoCliente == codigoCliente).ToListAsync();

            if (documentos == null)
                return NotFound(new { message = "Documento não encontrado." });

            return Ok(documentos);
        }

        [HttpGet("download/{id}")]
        public async Task<IActionResult> DownloadArquivo(int id)
        {
            var documento = await _context.DocumentoMetadado.FirstOrDefaultAsync(d => d.Id == id);

            if (documento == null)
            {
                return NotFound(new { mensagem = "Documento não encontrado." });
            }

            if (!System.IO.File.Exists(documento.Caminho))
            {
                return NotFound(new { mensagem = "O arquivo não foi encontrado no servidor." });
            }

            var fileInfo = new System.IO.FileInfo(documento.Caminho);
            long tamanhoBytes = 2 * (1024 * 1024);

            if (fileInfo.Length > tamanhoBytes)
            {
                return BadRequest(new { mensagem = "O arquivo excede o limite permitido de 2 MB para download." });
            }

            byte[] fileBytes = System.IO.File.ReadAllBytes(documento.Caminho);
            string nomeArquivo = $"{documento.Nome}{documento.Extensao}";

            return File(fileBytes, "application/octet-stream", nomeArquivo);
        }

        [HttpDelete("excluir/{id}")]
        public async Task<IActionResult> Excluir(int id)
        {
            var documento = await _context.DocumentoMetadado.FirstOrDefaultAsync(a => a.Id == id);

            if (documento == null)
            {
                return NotFound();
            }

            if (System.IO.File.Exists(documento.Caminho))
            {
                System.IO.File.Delete(documento.Caminho);
            }

            _context.DocumentoMetadado.Remove(documento);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Documento excluído com sucesso." });
        }
    }
}